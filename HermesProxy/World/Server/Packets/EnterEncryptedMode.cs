using System;
using System.Linq;
using System.Security.Cryptography;
using Framework.Cryptography;
using Framework.Logging;
using HermesProxy.World.Enums;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;

namespace HermesProxy.World.Server.Packets;

internal class EnterEncryptedMode : ServerPacket
{
	private byte[] EncryptionKey;

	private bool Enabled;

	private static byte[] EnableEncryptionSeedRSA = new byte[16]
	{
		144, 156, 208, 80, 90, 44, 20, 221, 92, 44,
		192, 100, 20, 243, 254, 201
	};

	private static byte[] EnableEncryptionSeedEd25519 = new byte[32]
	{
		102, 190, 41, 121, 239, 242, 213, 181, 97, 83,
		246, 95, 69, 174, 129, 203, 50, 236, 148, 236,
		117, 179, 95, 68, 106, 99, 67, 103, 23, 32,
		68, 52
	};

	private static byte[] EnableEncryptionContext = new byte[16]
	{
		167, 31, 182, 155, 201, 124, 221, 150, 233, 187,
		184, 33, 57, 141, 90, 212
	};

	private static byte[] Ed25519PrivateKey = new byte[32]
	{
		8, 189, 199, 163, 204, 195, 79, 63, 106, 11,
		255, 207, 49, 193, 182, 151, 105, 30, 114, 154,
		10, 171, 44, 119, 195, 111, 138, 231, 90, 154,
		167, 201
	};

	public EnterEncryptedMode(byte[] encryptionKey, bool enabled)
		: base(Opcode.SMSG_ENTER_ENCRYPTED_MODE)
	{
		EncryptionKey = encryptionKey;
		Enabled = enabled;
	}

	protected override void Write()
	{
		if (ModernVersion.ExpansionVersion >= 3)
		{
			WriteEd25519();
		}
		else
		{
			WriteRSA();
		}
	}

	private void WriteRSA()
	{
		var hash = new HmacSha256(EncryptionKey);
		hash.Process(BitConverter.GetBytes(Enabled), 1);
		hash.Finish(EnableEncryptionSeedRSA, 16);
		_worldPacket.WriteBytes(RsaCrypt.RSA.SignHash(hash.Digest, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1).Reverse().ToArray());
		_worldPacket.WriteBit(Enabled);
		_worldPacket.FlushBits();
	}

	private void WriteEd25519()
	{
		var hash = new HmacSha256(EncryptionKey);
		hash.Process(BitConverter.GetBytes(Enabled), 1);
		hash.Finish(EnableEncryptionSeedRSA, 16);
		var toSign = hash.Digest;
		Log.Print(LogType.Debug, "EnterEncryptedMode Ed25519: toSign=" + BitConverter.ToString(toSign, 0, 16) + "...", "Packets\\AuthenticationPackets.cs");
		var privateKeyParams = new Ed25519PrivateKeyParameters(Ed25519PrivateKey, 0);
		var signer = new Ed25519ctxSigner(EnableEncryptionContext);
		signer.Init(forSigning: true, privateKeyParams);
		signer.BlockUpdate(toSign, 0, toSign.Length);
		var signature = signer.GenerateSignature();
		Log.Print(LogType.Debug, $"EnterEncryptedMode Ed25519: signature={BitConverter.ToString(signature, 0, 16)}... ({signature.Length} bytes)", "Packets\\AuthenticationPackets.cs");
		_worldPacket.WriteBytes(signature);
		_worldPacket.WriteBit(Enabled);
		_worldPacket.FlushBits();
	}
}
