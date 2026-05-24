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

	private List<byte> _currentBuffer = new();

	public BnetTcpSession(Socket socket)
		: base(socket)
	{
		_handlerManager = new BnetServices.ServiceManager("BnetTcp", this, null);
	}

	public override void Accept()
	{
		var ipAddress = base.GetRemoteIpEndPoint().ToString();
		Log.Print(LogType.Server, "Accepting connection from " + ipAddress + ".", "BnetTcpSession.cs");
		AsyncHandshake(BnetServerCertificate.Certificate).Wait();
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
			var headerLengthBuffer = _currentBuffer.Take(2).ToArray();
			var headerLength = (ushort)IPAddress.HostToNetworkOrder(BitConverter.ToInt16(headerLengthBuffer));
			if (_currentBuffer.Count < 2 + headerLength)
			{
				return Task.CompletedTask;
			}
			var headerBuffer = _currentBuffer.Skip(2).Take(headerLength).ToArray();
			var header = new Header();
			header.MergeFrom(headerBuffer);
			var payloadLength = (int)header.Size;
			if (_currentBuffer.Count < 2 + headerLength + payloadLength)
			{
				return Task.CompletedTask;
			}
			var payloadBuffer = _currentBuffer.Skip(2).Skip(headerLength).Take(payloadLength)
				.ToArray();
			_currentBuffer.RemoveRange(0, 2 + headerLength + (int)header.Size);
			var stream = new CodedInputStream(payloadBuffer);
			if (header.ServiceId != 254 && header.ServiceHash != 0)
			{
				_handlerManager.Invoke(header.ServiceId, (OriginalHash)header.ServiceHash, header.MethodId, header.Token, stream);
			}
		}
		return Task.CompletedTask;
	}

	public void SendRpcMessage(uint serviceId, OriginalHash service, uint methodId, uint token, BattlenetRpcErrorCode status, IMessage? message)
	{
		var header = new Header
		{
			Token = token,
			Status = (uint)status,
			ServiceId = serviceId,
			ServiceHash = (uint)service,
			MethodId = methodId
		};
		if (message != null)
		{
			header.Size = (uint)message.CalculateSize();
		}
		var buffer = new ByteBuffer();
		buffer.WriteBytes(GetHeaderSize(header), 2u);
		buffer.WriteBytes(header.ToByteArray());
		if (message != null)
		{
			buffer.WriteBytes(message.ToByteArray());
		}
		AsyncWrite(buffer.GetData()).Wait();
	}

	private byte[] GetHeaderSize(Header header)
	{
		var size = (ushort)header.CalculateSize();
		var bytes = new byte[2]
		{
			(byte)((size >> 8) & 0xFF),
			(byte)(size & 0xFF)
		};
		var headerSizeBytes = BitConverter.GetBytes((ushort)header.CalculateSize());
		Array.Reverse(headerSizeBytes);
		return bytes;
	}

	IPEndPoint BnetServices.INetwork.GetRemoteIpEndPoint()
	{
		return base.GetRemoteIpEndPoint();
	}
}
