using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Framework;
using Framework.Constants;
using Framework.Cryptography;
using Framework.IO;
using Framework.Logging;
using Framework.Networking;
using HermesProxy.Enums;
using HashAlgorithm = Framework.Cryptography.HashAlgorithm;

namespace HermesProxy.Auth;

public class AuthClient
{
	private static readonly Action<ByteBuffer> _debugTraceBreakpointHandler = delegate
	{
	};

	private GlobalSessionData _globalSession;

	private Socket _clientSocket;

	private TaskCompletionSource<AuthResult> _response;

	private TaskCompletionSource _hasRealmlist;

	private bool _realmlistRequestIsPending;

	private byte[] _passwordHash;

	private BigInteger _key;

	private byte[] _m2;

	private string _username;

	private string _locale;

	public AuthClient(GlobalSessionData globalSession)
	{
		_globalSession = globalSession;
	}

	public GlobalSessionData GetSession()
	{
		return _globalSession;
	}

	public AuthResult ConnectToAuthServer(string username, string password, string locale)
	{
		_username = username;
		_locale = locale;
		_response = new TaskCompletionSource<AuthResult>();
		_hasRealmlist = new TaskCompletionSource();
		_realmlistRequestIsPending = false;
		string authstring = _username + ":" + password;
		_passwordHash = HashAlgorithm.SHA1.Hash(Encoding.ASCII.GetBytes(authstring.ToUpper()));
		try
		{
			IPAddress serverIpAddress = NetworkUtils.ResolveOrDirectIPv4(Settings.ServerAddress);
			Log.PrintNet(LogType.Network, LogNetDir.P2S, $"Connecting to auth server... (realmlist addr: {Settings.ServerAddress}:{Settings.ServerPort}) (resolved as: {serverIpAddress}:{Settings.ServerPort})", "Auth\\AuthClient.cs");
			_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			IPEndPoint endPoint = new IPEndPoint(serverIpAddress, Settings.ServerPort);
			_clientSocket.BeginConnect(endPoint, ConnectCallback, null);
		}
		catch (Exception ex)
		{
			Log.PrintNet(LogType.Error, LogNetDir.P2S, "Socket Error: " + ex.Message, "Auth\\AuthClient.cs");
			_response.SetResult(AuthResult.FAIL_INTERNAL_ERROR);
		}
		_response.Task.Wait();
		return _response.Task.Result;
	}

	public AuthResult Reconnect()
	{
		_response = new TaskCompletionSource<AuthResult>();
		_hasRealmlist = new TaskCompletionSource();
		_realmlistRequestIsPending = false;
		try
		{
			IPAddress serverIpAddress = NetworkUtils.ResolveOrDirectIPv4(Settings.ServerAddress);
			Log.PrintNet(LogType.Network, LogNetDir.P2S, $"Reconnecting to auth server... (realmlist addr: {Settings.ServerAddress}:{Settings.ServerPort}) (resolved as: {serverIpAddress}:{Settings.ServerPort})", "Auth\\AuthClient.cs");
			_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			IPEndPoint endPoint = new IPEndPoint(serverIpAddress, Settings.ServerPort);
			_clientSocket.BeginConnect(endPoint, ConnectCallback, null);
		}
		catch (Exception ex)
		{
			Log.PrintNet(LogType.Error, LogNetDir.P2S, "Socket Error: " + ex.Message, "Auth\\AuthClient.cs");
			_response.SetResult(AuthResult.FAIL_INTERNAL_ERROR);
		}
		_response.Task.Wait();
		return _response.Task.Result;
	}

	private void SetAuthResponse(AuthResult response)
	{
		_response.TrySetResult(response);
	}

	public void Disconnect()
	{
		if (IsConnected())
		{
			_clientSocket.Shutdown(SocketShutdown.Both);
			_clientSocket.Disconnect(reuseSocket: false);
		}
	}

	public bool IsConnected()
	{
		return _clientSocket != null && _clientSocket.Connected;
	}

	public byte[] GetSessionKey()
	{
		return _key.ToCleanByteArray();
	}

	private void ConnectCallback(IAsyncResult AR)
	{
		try
		{
			_clientSocket.EndConnect(AR);
			_clientSocket.ReceiveBufferSize = 65535;
			byte[] buffer = new byte[_clientSocket.ReceiveBufferSize];
			_clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, buffer);
			SendLogonChallenge(reconnect: false);
		}
		catch (Exception ex)
		{
			Log.Print(LogType.Error, "Connect Error: " + ex.Message, "Auth\\AuthClient.cs");
			SetAuthResponse(AuthResult.FAIL_INTERNAL_ERROR);
		}
	}

	private void ReconnectCallback(IAsyncResult AR)
	{
		try
		{
			_clientSocket.EndConnect(AR);
			_clientSocket.ReceiveBufferSize = 65535;
			byte[] buffer = new byte[_clientSocket.ReceiveBufferSize];
			_clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, buffer);
			SendLogonChallenge(reconnect: true);
		}
		catch (Exception ex)
		{
			Log.PrintNet(LogType.Error, LogNetDir.P2S, "Connect Error: " + ex.Message, "Auth\\AuthClient.cs");
			SetAuthResponse(AuthResult.FAIL_INTERNAL_ERROR);
		}
	}

	private void ReceiveCallback(IAsyncResult AR)
	{
		try
		{
			int received = _clientSocket.EndReceive(AR);
			if (received == 0)
			{
				SetAuthResponse(AuthResult.FAIL_INTERNAL_ERROR);
				Log.PrintNet(LogType.Error, LogNetDir.S2P, "Socket Closed By Server", "Auth\\AuthClient.cs");
				return;
			}
			byte[] oldBuffer = (byte[])AR.AsyncState;
			HandlePacket(oldBuffer, received);
			byte[] newBuffer = new byte[_clientSocket.ReceiveBufferSize];
			_clientSocket.BeginReceive(newBuffer, 0, newBuffer.Length, SocketFlags.None, ReceiveCallback, newBuffer);
		}
		catch (Exception ex)
		{
			Log.Print(LogType.Error, "Packet Read Error: " + ex.Message, "Auth\\AuthClient.cs");
			SetAuthResponse(AuthResult.FAIL_INTERNAL_ERROR);
		}
	}

	private void SendCallback(IAsyncResult AR)
	{
		try
		{
			_clientSocket.EndSend(AR);
		}
		catch (Exception ex)
		{
			Log.PrintNet(LogType.Error, LogNetDir.P2S, "Packet Send Error: " + ex.Message, "Auth\\AuthClient.cs");
			SetAuthResponse(AuthResult.FAIL_INTERNAL_ERROR);
		}
	}

	private void SendPacket(ByteBuffer packet)
	{
		try
		{
			_clientSocket.BeginSend(packet.GetData(), 0, (int)packet.GetSize(), SocketFlags.None, SendCallback, null);
		}
		catch (Exception ex)
		{
			Log.PrintNet(LogType.Error, LogNetDir.P2S, "Packet Write Error: " + ex.Message, "Auth\\AuthClient.cs");
			SetAuthResponse(AuthResult.FAIL_INTERNAL_ERROR);
		}
	}

	private void HandlePacket(byte[] buffer, int size)
	{
		ByteBuffer packet = new ByteBuffer(buffer);
		AuthCommand opcode = (AuthCommand)packet.ReadUInt8();
		Log.PrintNet(LogType.Debug, LogNetDir.S2P, $"Received opcode {opcode} size {size}.", "Auth\\AuthClient.cs");
		switch (opcode)
		{
		case AuthCommand.LOGON_CHALLENGE:
			HandleLogonChallenge(packet);
			return;
		case AuthCommand.LOGON_PROOF:
			HandleLogonProof(packet);
			return;
		case AuthCommand.RECONNECT_CHALLENGE:
			HandleReconnectChallenge(packet);
			return;
		case AuthCommand.RECONNECT_PROOF:
			HandleReconnectProof(packet);
			return;
		case AuthCommand.REALM_LIST:
			HandleRealmList(packet);
			return;
		}
		Log.PrintNet(LogType.Error, LogNetDir.S2P, $"No handler for opcode {opcode}!", "Auth\\AuthClient.cs");
		SetAuthResponse(AuthResult.FAIL_INTERNAL_ERROR);
	}

	private void SendLogonChallenge(bool reconnect)
	{
		ByteBuffer buffer = new ByteBuffer();
		buffer.WriteUInt8((byte)(reconnect ? 2 : 0));
		buffer.WriteUInt8((byte)((LegacyVersion.ExpansionVersion > 1) ? 8u : 3u));
		buffer.WriteUInt16((ushort)(_username.Length + 30));
		buffer.WriteBytes(Encoding.ASCII.GetBytes("WoW"));
		buffer.WriteUInt8(0);
		buffer.WriteUInt8(LegacyVersion.ExpansionVersion);
		buffer.WriteUInt8(LegacyVersion.MajorVersion);
		buffer.WriteUInt8(LegacyVersion.MinorVersion);
		buffer.WriteUInt16((ushort)Settings.ServerBuild);
		buffer.WriteBytes(Encoding.ASCII.GetBytes(Settings.ReportedPlatform.Reverse()));
		buffer.WriteUInt8(0);
		buffer.WriteBytes(Encoding.ASCII.GetBytes(Settings.ReportedOS.Reverse()));
		buffer.WriteUInt8(0);
		buffer.WriteBytes(Encoding.ASCII.GetBytes(_locale.Reverse()));
		buffer.WriteUInt32(60u);
		buffer.WriteUInt32(16777343u);
		buffer.WriteUInt8((byte)_username.Length);
		buffer.WriteBytes(Encoding.ASCII.GetBytes(_username));
		SendPacket(buffer);
	}

	private void HandleLogonChallenge(ByteBuffer packet)
	{
		byte unk2 = packet.ReadUInt8();
		AuthResult error = (AuthResult)packet.ReadUInt8();
		if (error != AuthResult.SUCCESS)
		{
			Log.Print(LogType.Error, $"Login failed. Reason: {error}", "Auth\\AuthClient.cs");
			SetAuthResponse(error);
			return;
		}
		byte[] challenge_B = packet.ReadBytes(32u);
		byte challenge_gLen = packet.ReadUInt8();
		byte[] challenge_g = packet.ReadBytes(1u);
		byte challenge_nLen = packet.ReadUInt8();
		byte[] challenge_N = packet.ReadBytes(32u);
		byte[] challenge_salt = packet.ReadBytes(32u);
		byte[] challenge_version = packet.ReadBytes(16u);
		byte challenge_securityFlags = packet.ReadUInt8();
		BigInteger k = new BigInteger(3);
		BigInteger B = challenge_B.ToBigInteger();
		BigInteger g = challenge_g.ToBigInteger();
		BigInteger N = challenge_N.ToBigInteger();
		BigInteger salt = challenge_salt.ToBigInteger();
		BigInteger versionChallenge = challenge_version.ToBigInteger();
		BigInteger x = HashAlgorithm.SHA1.Hash(challenge_salt, _passwordHash).ToBigInteger();
		RandomNumberGenerator rand = RandomNumberGenerator.Create();
		BigInteger a;
		BigInteger A;
		do
		{
			byte[] randBytes = new byte[19];
			rand.GetBytes(randBytes);
			a = randBytes.ToBigInteger();
			A = g.ModPow(a, N);
		}
		while (A.ModPow(1, N) == 0L);
		BigInteger u = HashAlgorithm.SHA1.Hash(A.ToCleanByteArray(), B.ToCleanByteArray()).ToBigInteger();
		BigInteger S = ((B + k * (N - g.ModPow(x, N))) % N).ModPow(a + u * x, N);
		byte[] sData = S.ToCleanByteArray();
		if (sData.Length < 32)
		{
			byte[] tmpBuffer = new byte[32];
			Buffer.BlockCopy(sData, 0, tmpBuffer, 32 - sData.Length, sData.Length);
			sData = tmpBuffer;
		}
		byte[] keyData = new byte[40];
		byte[] temp = new byte[16];
		for (int i = 0; i < 16; i++)
		{
			temp[i] = sData[i * 2];
		}
		byte[] keyHash = HashAlgorithm.SHA1.Hash(temp);
		for (int j = 0; j < 20; j++)
		{
			keyData[j * 2] = keyHash[j];
		}
		for (int l = 0; l < 16; l++)
		{
			temp[l] = sData[l * 2 + 1];
		}
		keyHash = HashAlgorithm.SHA1.Hash(temp);
		for (int m = 0; m < 20; m++)
		{
			keyData[m * 2 + 1] = keyHash[m];
		}
		_key = keyData.ToBigInteger();
		byte[] gNHash = new byte[20];
		byte[] nHash = HashAlgorithm.SHA1.Hash(N.ToCleanByteArray());
		for (int n = 0; n < 20; n++)
		{
			gNHash[n] = nHash[n];
		}
		byte[] gHash = HashAlgorithm.SHA1.Hash(g.ToCleanByteArray());
		for (int num = 0; num < 20; num++)
		{
			gNHash[num] ^= gHash[num];
		}
		byte[] userHash = HashAlgorithm.SHA1.Hash(Encoding.ASCII.GetBytes(_username.ToUpper()));
		byte[] m1Hash = HashAlgorithm.SHA1.Hash(gNHash, userHash, challenge_salt, A.ToCleanByteArray(), B.ToCleanByteArray(), _key.ToCleanByteArray());
		_m2 = HashAlgorithm.SHA1.Hash(A.ToCleanByteArray(), m1Hash, keyData);
		SendLogonProof(A.ToCleanByteArray(), m1Hash, new byte[20]);
	}

	private void SendLogonProof(byte[] A, byte[] M1, byte[] crc)
	{
		ByteBuffer buffer = new ByteBuffer();
		buffer.WriteUInt8(1);
		buffer.WriteBytes(A);
		buffer.WriteBytes(M1);
		buffer.WriteBytes(crc);
		buffer.WriteUInt8(0);
		buffer.WriteUInt8(0);
		_debugTraceBreakpointHandler(buffer);
		SendPacket(buffer);
	}

	private void HandleLogonProof(ByteBuffer packet)
	{
		AuthResult error = (AuthResult)packet.ReadUInt8();
		if (error != AuthResult.SUCCESS)
		{
			Log.Print(LogType.Error, $"Login failed. Reason: {error}", "Auth\\AuthClient.cs");
			SetAuthResponse(error);
			return;
		}
		byte[] M2 = packet.ReadBytes(20u);
		uint accountFlags = 0u;
		uint surveyId = 0u;
		ushort loginFlags = 0;
		if (Settings.ServerBuild < ClientVersionBuild.V2_0_3_6299)
		{
			surveyId = packet.ReadUInt32();
		}
		else if (Settings.ServerBuild < ClientVersionBuild.V2_4_0_8089)
		{
			surveyId = packet.ReadUInt32();
			loginFlags = packet.ReadUInt16();
		}
		else
		{
			accountFlags = packet.ReadUInt32();
			surveyId = packet.ReadUInt32();
			loginFlags = packet.ReadUInt16();
		}
		bool equal = _m2 != null && _m2.Length == 20;
		int i = 0;
		while (equal && i < _m2.Length && (equal = _m2[i] == M2[i]))
		{
			i++;
		}
		if (!equal)
		{
			Log.Print(LogType.Error, "Authentication failed!", "Auth\\AuthClient.cs");
			SetAuthResponse(AuthResult.FAIL_INTERNAL_ERROR);
		}
		else
		{
			Log.Print(LogType.Network, "Authentication succeeded!", "Auth\\AuthClient.cs");
			SetAuthResponse(AuthResult.SUCCESS);
		}
	}

	public void HandleReconnectChallenge(ByteBuffer packet)
	{
		packet.ReadUInt8();
		byte[] reconnectProof = packet.ReadBytes(16u);
		packet.ReadBytes(16u);
		RandomNumberGenerator rand = RandomNumberGenerator.Create();
		byte[] R1 = new byte[16];
		rand.GetBytes(R1);
		byte[] R2 = HashAlgorithm.SHA1.Hash(Encoding.ASCII.GetBytes(_username), R1, reconnectProof, GetSessionKey());
		byte[] R3 = HashAlgorithm.SHA1.Hash(R1, new byte[20]);
		SendReconnectProof(R1, R2, R3);
	}

	private void SendReconnectProof(byte[] R1, byte[] R2, byte[] R3)
	{
		ByteBuffer buffer = new ByteBuffer();
		buffer.WriteUInt8(3);
		buffer.WriteBytes(R1);
		buffer.WriteBytes(R2);
		buffer.WriteBytes(R3);
		buffer.WriteUInt8(0);
		SendPacket(buffer);
	}

	public void HandleReconnectProof(ByteBuffer packet)
	{
		AuthResult error = (AuthResult)packet.ReadUInt8();
		if (error != AuthResult.SUCCESS)
		{
			Log.Print(LogType.Error, $"Reconnect failed. Reason: {error}", "Auth\\AuthClient.cs");
			SetAuthResponse(error);
		}
		else
		{
			SetAuthResponse(AuthResult.SUCCESS);
		}
	}

	public void SendRealmListUpdateRequest()
	{
		Log.Print(LogType.Server, "Requesting RealmList update for " + _username, "Auth\\AuthClient.cs");
		ByteBuffer buffer = new ByteBuffer();
		buffer.WriteUInt8(16);
		for (int i = 0; i < 4; i++)
		{
			buffer.WriteUInt8(0);
		}
		_realmlistRequestIsPending = true;
		SendPacket(buffer);
	}

	private void HandleRealmList(ByteBuffer packet)
	{
		packet.ReadUInt16();
		packet.ReadUInt32();
		ushort realmsCount = 0;
		realmsCount = ((Settings.ServerBuild >= ClientVersionBuild.V2_0_3_6299) ? packet.ReadUInt16() : packet.ReadUInt8());
		Log.Print(LogType.Network, $"Received {realmsCount} realms.", "AuthClient.cs");
		List<RealmInfo> realmList = new List<RealmInfo>();
		for (ushort i = 0; i < realmsCount; i++)
		{
			RealmInfo realmInfo = new RealmInfo();
			if (Settings.ServerBuild < ClientVersionBuild.V2_0_3_6299)
			{
				realmInfo.Type = (RealmType)packet.ReadUInt32();
			}
			else
			{
				realmInfo.Type = (RealmType)packet.ReadUInt8();
				realmInfo.IsLocked = packet.ReadUInt8();
			}
			realmInfo.Flags = (RealmFlags)packet.ReadUInt8();
			realmInfo.Name = packet.ReadCString();
			string addressAndPort = packet.ReadCString();
			string[] strArr = addressAndPort.Split(':');
			realmInfo.Address = strArr[0].Trim();
			realmInfo.Port = ushort.Parse(strArr[1]);
			realmInfo.Population = packet.ReadFloat();
			realmInfo.CharacterCount = packet.ReadUInt8();
			realmInfo.Timezone = packet.ReadUInt8();
			realmInfo.ID = packet.ReadUInt8();
			if ((realmInfo.Flags & RealmFlags.SpecifyBuild) != RealmFlags.None)
			{
				realmInfo.VersionMajor = packet.ReadUInt8();
				realmInfo.VersionMinor = packet.ReadUInt8();
				realmInfo.VersonBugfix = packet.ReadUInt8();
				realmInfo.Build = packet.ReadUInt16();
			}
			realmList.Add(realmInfo);
		}
		GetSession().RealmManager.UpdateRealms(realmList);
		_hasRealmlist.SetResult();
	}

	public void WaitOrRequestRealmList()
	{
		if (!_realmlistRequestIsPending || !_hasRealmlist.Task.Wait(TimeSpan.FromSeconds(2.0)))
		{
			SendRealmListUpdateRequest();
		}
		_hasRealmlist.Task.Wait();
	}
}
