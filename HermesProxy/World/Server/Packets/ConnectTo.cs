using System;
using System.Linq;
using System.Security.Cryptography;
using Framework.Constants;
using Framework.Cryptography;
using Framework.IO;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class ConnectTo : ServerPacket
{
	public class ConnectPayload
	{
		public SocketAddress Where;

		public ushort Port;

		public byte[] Signature = new byte[256];
	}

	public struct SocketAddress
	{
		public AddressType Type;

		public byte[] IPv4;

		public byte[] IPv6;

		public string NameSocket;
	}

	public enum AddressType
	{
		IPv4 = 1,
		IPv6 = 2,
		NamedSocket = 3
	}

	public ulong Key;

	public ConnectToSerial Serial;

	public ConnectPayload Payload;

	public byte Con;

	public ConnectTo()
		: base(Opcode.SMSG_CONNECT_TO)
	{
		this.Payload = new ConnectPayload();
	}

	protected override void Write()
	{
		ByteBuffer whereBuffer = new ByteBuffer();
		whereBuffer.WriteUInt8((byte)this.Payload.Where.Type);
		switch (this.Payload.Where.Type)
		{
		case AddressType.IPv4:
			whereBuffer.WriteBytes(this.Payload.Where.IPv4);
			break;
		case AddressType.IPv6:
			whereBuffer.WriteBytes(this.Payload.Where.IPv6);
			break;
		case AddressType.NamedSocket:
			whereBuffer.WriteString(this.Payload.Where.NameSocket);
			break;
		}
		Sha256 hash = new Sha256();
		hash.Process(whereBuffer.GetData(), (int)whereBuffer.GetSize());
		hash.Process((uint)this.Payload.Where.Type);
		hash.Finish(BitConverter.GetBytes(this.Payload.Port));
		this.Payload.Signature = RsaCrypt.RSA.SignHash(hash.Digest, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1).Reverse().ToArray();
		base._worldPacket.WriteBytes(this.Payload.Signature, (uint)this.Payload.Signature.Length);
		base._worldPacket.WriteBytes(whereBuffer);
		base._worldPacket.WriteUInt16(this.Payload.Port);
		base._worldPacket.WriteUInt32((uint)this.Serial);
		base._worldPacket.WriteUInt8(this.Con);
		base._worldPacket.WriteUInt64(this.Key);
	}
}
