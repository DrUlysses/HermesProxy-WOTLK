using System;
using System.Linq;
using Framework.Cryptography;

namespace HermesProxy.World.Client;

public class WotlkWorldCrypt : LegacyWorldCrypt
{
	public const uint CRYPTED_SEND_LEN = 6u;

	public const uint CRYPTED_RECV_LEN = 4u;

	private byte[] m_sendKey;

	private byte[] m_recvKey;

	private byte[] m_sendState;

	private byte[] m_recvState;

	private bool m_isInitialized;

	public void Initialize(byte[] sessionKey)
	{
		var encSeed = new byte[16]
		{
			194, 179, 114, 60, 198, 174, 217, 181, 52, 60,
			83, 238, 47, 67, 103, 206
		};
		var decSeed = new byte[16]
		{
			204, 152, 174, 4, 232, 151, 234, 202, 18, 221,
			192, 147, 66, 145, 83, 87
		};
		var encHash = new HmacHash(encSeed);
		encHash.Finish(sessionKey, sessionKey.Length);
		m_sendKey = encHash.Digest.ToArray();
		var decHash = new HmacHash(decSeed);
		decHash.Finish(sessionKey, sessionKey.Length);
		m_recvKey = decHash.Digest.ToArray();
		m_sendState = InitRC4(m_sendKey);
		m_recvState = InitRC4(m_recvKey);
		m_isInitialized = true;
	}

	private byte[] InitRC4(byte[] key)
	{
		var s = new byte[256];
		for (var i = 0; i < 256; i++)
		{
			s[i] = (byte)i;
		}
		var j = 0;
		for (var k = 0; k < 256; k++)
		{
			j = (j + s[k] + key[k % key.Length]) & 0xFF;
			ref var reference = ref s[k];
			ref var reference2 = ref s[j];
			var b = s[j];
			var b2 = s[k];
			reference = b;
			reference2 = b2;
		}
		var state = new byte[258];
		Buffer.BlockCopy(s, 0, state, 0, 256);
		state[256] = 0;
		state[257] = 0;
		var drop = new byte[1024];
		RC4Process(state, drop, 1024);
		return state;
	}

	private static void RC4Process(byte[] state, byte[] data, int len)
	{
		int x = state[256];
		int y = state[257];
		for (var k = 0; k < len; k++)
		{
			x = (x + 1) & 0xFF;
			y = (y + state[x]) & 0xFF;
			ref var reference = ref state[x];
			ref var reference2 = ref state[y];
			var b = state[y];
			var b2 = state[x];
			reference = b;
			reference2 = b2;
			data[k] ^= state[(state[x] + state[y]) & 0xFF];
		}
		state[256] = (byte)x;
		state[257] = (byte)y;
	}

	public void Decrypt(byte[] data, int len)
	{
		if (m_isInitialized && len >= 4L)
		{
			RC4Process(m_recvState, data, 4);
		}
	}

	public void Encrypt(byte[] data, int len)
	{
		if (m_isInitialized && len >= 6L)
		{
			RC4Process(m_sendState, data, 6);
		}
	}
}
