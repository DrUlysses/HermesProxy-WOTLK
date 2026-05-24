using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Bgs.Protocol;
using BNetServer.Services;
using Framework.Constants;
using Framework.IO;
using Framework.Logging;
using Framework.Networking;
using Google.Protobuf;

namespace BNetServer.Networking;

public class BnetTcpSession : SSLSocket, BnetServices.INetwork
{
	private readonly BnetServices.ServiceManager _handlerManager;

	private List<byte> _currentBuffer = new List<byte>();

	public BnetTcpSession(Socket socket)
		: base(socket)
	{
		_handlerManager = new BnetServices.ServiceManager("BnetTcp", this, null);
	}

	public override void Accept()
	{
		string ipAddress = base.GetRemoteIpEndPoint().ToString();
		Log.Print(LogType.Server, "Accepting connection from " + ipAddress + ".", "BnetTcpSession.cs");
		AsyncHandshake(BnetServerCertificate.Certificate);
	}

	public override bool Update()
	{
		if (!base.Update())
		{
			return false;
		}
		return true;
	}

	public override async Task ReadHandler(byte[] data, int receivedLength)
	{
		if (IsOpen())
		{
			Log.Print(LogType.Debug, $"BnetTcp received {receivedLength} bytes: {BitConverter.ToString(data, 0, Math.Min(receivedLength, 16))}", "BnetTcpSession.cs");
			_currentBuffer.AddRange(data.Take(receivedLength));
			await ProcessCurrentBuffer();
			await AsyncRead();
		}
	}

	private Task ProcessCurrentBuffer()
	{
		while (_currentBuffer.Count > 2)
		{
			byte[] headerLengthBuffer = _currentBuffer.Take(2).ToArray();
			ushort headerLength = (ushort)IPAddress.HostToNetworkOrder(BitConverter.ToInt16(headerLengthBuffer));
			if (_currentBuffer.Count < 2 + headerLength)
			{
				return Task.CompletedTask;
			}
			byte[] headerBuffer = _currentBuffer.Skip(2).Take(headerLength).ToArray();
			Header header = new Header();
			header.MergeFrom(headerBuffer);
			int payloadLength = (int)header.Size;
			if (_currentBuffer.Count < 2 + headerLength + payloadLength)
			{
				return Task.CompletedTask;
			}
			byte[] payloadBuffer = _currentBuffer.Skip(2).Skip(headerLength).Take(payloadLength)
				.ToArray();
			_currentBuffer.RemoveRange(0, 2 + headerLength + (int)header.Size);
			CodedInputStream stream = new CodedInputStream(payloadBuffer);
			if (header.ServiceId != 254 && header.ServiceHash != 0)
			{
				_handlerManager.Invoke(header.ServiceId, (OriginalHash)header.ServiceHash, header.MethodId, header.Token, stream);
			}
		}
		return Task.CompletedTask;
	}

	public void SendRpcMessage(uint serviceId, OriginalHash service, uint methodId, uint token, BattlenetRpcErrorCode status, IMessage? message)
	{
		Header header = new Header();
		header.Token = token;
		header.Status = (uint)status;
		header.ServiceId = serviceId;
		header.ServiceHash = (uint)service;
		header.MethodId = methodId;
		if (message != null)
		{
			header.Size = (uint)message.CalculateSize();
		}
		ByteBuffer buffer = new ByteBuffer();
		buffer.WriteBytes(GetHeaderSize(header), 2u);
		buffer.WriteBytes(header.ToByteArray());
		if (message != null)
		{
			buffer.WriteBytes(message.ToByteArray());
		}
		AsyncWrite(buffer.GetData());
	}

	public byte[] GetHeaderSize(Header header)
	{
		ushort size = (ushort)header.CalculateSize();
		byte[] bytes = new byte[2]
		{
			(byte)((size >> 8) & 0xFF),
			(byte)(size & 0xFF)
		};
		byte[] headerSizeBytes = BitConverter.GetBytes((ushort)header.CalculateSize());
		Array.Reverse(headerSizeBytes);
		return bytes;
	}

	IPEndPoint BnetServices.INetwork.GetRemoteIpEndPoint()
	{
		return base.GetRemoteIpEndPoint();
	}
}
