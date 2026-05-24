using System.Collections.Generic;
using Framework.IO;
using HermesProxy.World.Client;
using HermesProxy.World.Enums;
using Ionic.Zlib;

namespace HermesProxy.World;

public class WorldPacket : ByteBuffer
{
	private uint opcode;

	private long m_receivedTime;

	public WorldPacket(uint opcode = 0u)
	{
		this.opcode = opcode;
	}

	public WorldPacket(Opcode opcode)
	{
		this.opcode = LegacyVersion.GetCurrentOpcode(opcode);
	}

	public WorldPacket(uint opcode, byte[] data)
		: base(data)
	{
		this.opcode = opcode;
	}

	public WorldPacket(byte[] data)
		: base(data)
	{
		opcode = ReadUInt16();
	}

	public KeyValuePair<int, bool> ReadEntry()
	{
		uint entry = ReadUInt32();
		uint realEntry = entry & 0x7FFFFFFF;
		return new KeyValuePair<int, bool>((int)realEntry, realEntry != entry);
	}

	public WowGuid64 ReadGuid()
	{
		return new WowGuid64(ReadUInt64());
	}

	public WowGuid64 ReadPackedGuid()
	{
		return new WowGuid64(ReadPackedUInt64(ReadUInt8()));
	}

	public WowGuid128 ReadPackedGuid128()
	{
		byte loLength = ReadUInt8();
		byte hiLength = ReadUInt8();
		ulong low = ReadPackedUInt64(loLength);
		return new WowGuid128(ReadPackedUInt64(hiLength), low);
	}

	private ulong ReadPackedUInt64(byte length)
	{
		if (length == 0)
		{
			return 0uL;
		}
		ulong guid = 0uL;
		for (int i = 0; i < 8; i++)
		{
			if (((1 << i) & length) != 0)
			{
				guid |= (ulong)ReadUInt8() << i * 8;
			}
		}
		return guid;
	}

	public UpdateField ReadUpdateField()
	{
		uint val = ReadUInt32();
		return new UpdateField(val);
	}

	public WorldPacket Inflate(int inflatedSize)
	{
		byte[] arr = ReadToEnd();
		byte[] newarr = new byte[inflatedSize];
		ZlibCodec stream = new ZlibCodec(CompressionMode.Decompress)
		{
			InputBuffer = arr,
			NextIn = 0,
			AvailableBytesIn = arr.Length,
			OutputBuffer = newarr,
			NextOut = 0,
			AvailableBytesOut = inflatedSize
		};
		stream.Inflate(FlushType.None);
		stream.Inflate(FlushType.Finish);
		stream.EndInflate();
		WorldPacket pkt = new WorldPacket(GetOpcode(), newarr);
		pkt.SetReceiveTime(GetReceivedTime());
		return pkt;
	}

	public void WriteGuid(WowGuid64 guid)
	{
		WriteUInt64(guid.GetLowValue());
	}

	public void WritePackedGuid(WowGuid64 guid)
	{
		WritePackedUInt64(guid.Low);
	}

	public void WritePackedGuid128(WowGuid128 guid)
	{
		if (guid.IsEmpty())
		{
			WriteUInt8(0);
			WriteUInt8(0);
			return;
		}
		byte lowMask;
		byte[] lowPacked;
		uint loSize = PackUInt64(guid.GetLowValue(), out lowMask, out lowPacked);
		byte highMask;
		byte[] highPacked;
		uint hiSize = PackUInt64(guid.GetHighValue(), out highMask, out highPacked);
		WriteUInt8(lowMask);
		WriteUInt8(highMask);
		base.WriteBytes(lowPacked, loSize);
		base.WriteBytes(highPacked, hiSize);
	}

	public void WritePackedUInt64(ulong guid)
	{
		byte mask;
		byte[] packed;
		uint packedSize = PackUInt64(guid, out mask, out packed);
		WriteUInt8(mask);
		base.WriteBytes(packed, packedSize);
	}

	private uint PackUInt64(ulong value, out byte mask, out byte[] result)
	{
		uint resultSize = 0u;
		mask = 0;
		result = new byte[8];
		byte i = 0;
		while (value != 0)
		{
			if ((value & 0xFF) != 0)
			{
				mask |= (byte)(1 << i);
				result[resultSize++] = (byte)(value & 0xFF);
			}
			value >>= 8;
			i++;
		}
		return resultSize;
	}

	public void WriteBytes(WorldPacket data)
	{
		FlushBits();
		base.WriteBytes(data.GetData());
	}

	public uint GetOpcode()
	{
		return opcode;
	}

	public Opcode GetUniversalOpcode(bool isModern)
	{
		if (isModern)
		{
			return ModernVersion.GetUniversalOpcode(GetOpcode());
		}
		return LegacyVersion.GetUniversalOpcode(GetOpcode());
	}

	public long GetReceivedTime()
	{
		return m_receivedTime;
	}

	public void SetReceiveTime(long receivedTime)
	{
		m_receivedTime = receivedTime;
	}
}
