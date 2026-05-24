using System.Linq;

namespace HermesProxy.World.Client;

public class VanillaWorldCrypt : LegacyWorldCrypt
{
	public const uint CRYPTED_SEND_LEN = 6u;

	public const uint CRYPTED_RECV_LEN = 4u;

	private byte[] m_key;

	private byte m_send_i;

	private byte m_send_j;

	private byte m_recv_i;

	private byte m_recv_j;

	private bool m_isInitialized;

	public void Initialize(byte[] sessionKey)
	{
		SetKey(sessionKey);
		m_send_i = m_send_j = m_recv_i = m_recv_j = 0;
		m_isInitialized = true;
	}

	public void Decrypt(byte[] data, int len)
	{
		if (len >= 4L)
		{
			byte t = 0;
			while (t < 4u)
			{
				m_recv_i %= (byte)m_key.Count();
				var x = (byte)((data[t] - m_recv_j) ^ m_key[m_recv_i]);
				m_recv_i++;
				m_recv_j = data[t];
				data[t] = x;
				t++;
			}
		}
	}

	public void Encrypt(byte[] data, int len)
	{
		if (m_isInitialized && len >= 6L)
		{
			byte t = 0;
			while (t < 6u)
			{
				m_send_i %= (byte)m_key.Count();
				var x = (byte)((data[t] ^ m_key[m_send_i]) + m_send_j);
				m_send_i++;
				data[t] = m_send_j = x;
				t++;
			}
		}
	}

	public void SetKey(byte[] key)
	{
		m_key = key.ToArray();
	}
}
