using System;
using Framework.Collections;
using Framework.IO;
using Framework.Logging;

namespace HermesProxy.World.Objects;

public class UpdateFieldsArray
{
	public uint ValuesCount;

	public UpdateValues[] m_updateValues;

	public UpdateMask m_updateMask;

	public UpdateFieldsArray(uint size)
	{
		this.ValuesCount = size;
		this.m_updateValues = new UpdateValues[size];
		this.m_updateMask = new UpdateMask(size);
	}

	public void WriteToPacket(ByteBuffer buffer)
	{
		ByteBuffer fieldBuffer = new ByteBuffer();
		for (int index = 0; index < this.ValuesCount; index++)
		{
			if (this.m_updateMask.GetBit(index))
			{
				fieldBuffer.WriteUInt32(this.m_updateValues[index].UnsignedValue);
			}
		}
		this.m_updateMask.AppendToPacket(buffer);
		buffer.WriteBytes(fieldBuffer);
	}

	public void SetUpdateField<T>(object index, T value, byte offset = 0) where T : new()
	{
		if (value is byte byteValue)
		{
			if (offset > 3)
			{
				Log.Print(LogType.Error, $"SetUpdateField<UInt8>: Wrong offset: {offset}", "SetUpdateField", "UpdateFieldsArray.cs");
			}
			else if ((byte)(this.m_updateValues[(int)index].UnsignedValue >> offset * 8) != byteValue)
			{
				this.m_updateValues[(int)index].UnsignedValue &= (uint)(~(255 << offset * 8));
				this.m_updateValues[(int)index].UnsignedValue |= (uint)(byteValue << offset * 8);
				this.m_updateMask.SetBit((int)index);
			}
		}
		else if (value is ushort ushortValue)
		{
			if (offset > 1)
			{
				Log.Print(LogType.Error, $"SetUpdateField<UInt16>: Wrong offset: {offset}", "SetUpdateField", "UpdateFieldsArray.cs");
			}
			else if ((ushort)(this.GetUpdateField<uint>(index, 0) >> offset * 16) != ushortValue)
			{
				this.m_updateValues[(int)index].UnsignedValue &= (uint)(~(65535 << offset * 16));
				this.m_updateValues[(int)index].UnsignedValue |= (uint)(ushortValue << offset * 16);
				this.m_updateMask.SetBit((int)index);
			}
		}
		else if (value is int intValue)
		{
			if (this.m_updateValues[(int)index].SignedValue != intValue)
			{
				this.m_updateValues[(int)index].SignedValue = intValue;
				this.m_updateMask.SetBit((int)index);
			}
		}
		else if (value is uint uintValue)
		{
			if (this.m_updateValues[(int)index].UnsignedValue != uintValue)
			{
				this.m_updateValues[(int)index].UnsignedValue = uintValue;
				this.m_updateMask.SetBit((int)index);
			}
		}
		else if (value is float floatValue)
		{
			if (this.m_updateValues[(int)index].FloatValue != floatValue)
			{
				this.m_updateValues[(int)index].FloatValue = floatValue;
				this.m_updateMask.SetBit((int)index);
			}
		}
		else if (value is ulong ulongValue)
		{
			if (this.GetUpdateField<ulong>(index, 0) != ulongValue)
			{
				this.m_updateValues[(int)index].UnsignedValue = MathFunctions.Pair64_LoPart(ulongValue);
				this.m_updateValues[(int)index + 1].UnsignedValue = MathFunctions.Pair64_HiPart(ulongValue);
				this.m_updateMask.SetBit((int)index);
				this.m_updateMask.SetBit((int)index + 1);
			}
		}
		else
		{
			if (!(value is WowGuid128 guid))
			{
				throw new Exception("Unhandled type " + typeof(T).ToString() + " in SetUpdateField!");
			}
			this.SetUpdateField(index, guid.GetLowValue(), 0);
			this.SetUpdateField((int)index + 2, guid.GetHighValue(), 0);
		}
	}

	public T GetUpdateField<T>(object index, byte offset = 0)
	{
		T val = default(T);
		if (1 == 0)
		{
		}
		T result;
		if (!(val is byte))
		{
			if (!(val is ushort))
			{
				if (!(val is int))
				{
					if (!(val is uint))
					{
						if (!(val is float))
						{
							if (!(val is ulong))
							{
								if (!(val is WowGuid128))
								{
									throw new Exception($"{typeof(T)} is not implemented in GetUpdateField<T>");
								}
								result = (T)Convert.ChangeType(new WowGuid128(this.GetUpdateField<ulong>((int)index + 2, 0), this.GetUpdateField<ulong>(index, 0)), typeof(T));
							}
							else
							{
								result = (T)Convert.ChangeType(((ulong)this.m_updateValues[(int)index + 1].UnsignedValue << 32) | this.m_updateValues[(int)index].UnsignedValue, typeof(T));
							}
						}
						else
						{
							result = (T)Convert.ChangeType(this.m_updateValues[(int)index].FloatValue, typeof(T));
						}
					}
					else
					{
						result = (T)Convert.ChangeType(this.m_updateValues[(int)index].UnsignedValue, typeof(T));
					}
				}
				else
				{
					result = (T)Convert.ChangeType(this.m_updateValues[(int)index].SignedValue, typeof(T));
				}
			}
			else
			{
				result = (T)Convert.ChangeType((ushort)(this.m_updateValues[(int)index].UnsignedValue >> offset * 16) & 0xFFFF, typeof(T));
			}
		}
		else
		{
			result = (T)Convert.ChangeType((byte)(this.m_updateValues[(int)index].UnsignedValue >> offset * 8) & 0xFF, typeof(T));
		}
		if (1 == 0)
		{
		}
		return result;
	}

	public void _LoadIntoDataField(string data, uint startOffset, uint count)
	{
		if (string.IsNullOrEmpty(data))
		{
			return;
		}
		StringArray lines = new StringArray(data, ' ');
		if (lines.Length != count)
		{
			return;
		}
		for (int index = 0; index < count; index++)
		{
			if (uint.TryParse(lines[index], out var value))
			{
				this.m_updateValues[(int)startOffset + index].UnsignedValue = value;
				this.m_updateMask.SetBit((int)(startOffset + index));
			}
		}
	}

	public bool HasFlag(object index, object flag)
	{
		if ((int)index >= this.ValuesCount)
		{
			return false;
		}
		return (this.GetUpdateField<uint>(index, 0) & (uint)flag) != 0;
	}

	public void AddFlag(object index, object newFlag)
	{
		uint oldValue = this.m_updateValues[(int)index].UnsignedValue;
		uint newValue = oldValue | Convert.ToUInt32(newFlag);
		if (oldValue != newValue)
		{
			this.SetUpdateField(index, newValue, 0);
		}
	}

	public void RemoveFlag(object index, object newFlag)
	{
		uint oldValue = this.m_updateValues[(int)index].UnsignedValue;
		uint newValue = oldValue & ~Convert.ToUInt32(newFlag);
		if (oldValue != newValue)
		{
			this.SetUpdateField(index, newValue, 0);
		}
	}

	public void ApplyFlag<T>(object index, T flag, bool apply)
	{
		if (apply)
		{
			this.AddFlag(index, flag);
		}
		else
		{
			this.RemoveFlag(index, flag);
		}
	}

	public void AddFlag64(object index, object newFlag)
	{
		ulong oldValue = this.GetUpdateField<ulong>(index, 0);
		ulong newValue = oldValue | Convert.ToUInt64(newFlag);
		if (oldValue != newValue)
		{
			this.SetUpdateField(index, newValue, 0);
		}
	}

	public void RemoveFlag64(object index, object newFlag)
	{
		ulong oldValue = this.GetUpdateField<ulong>(index, 0);
		ulong newValue = oldValue & ~Convert.ToUInt64(newFlag);
		if (oldValue != newValue)
		{
			this.SetUpdateField(index, newValue, 0);
		}
	}

	public void ApplyFlag64<T>(object index, T flag, bool apply)
	{
		if (apply)
		{
			this.AddFlag(index, flag);
		}
		else
		{
			this.RemoveFlag(index, flag);
		}
	}

	public void AddByteFlag(object index, byte offset, object newFlag)
	{
		if (offset > 4)
		{
			Log.Print(LogType.Error, $"Object.SetByteFlag: Wrong offset {offset}", "AddByteFlag", "UpdateFieldsArray.cs");
		}
		else if ((((byte)this.m_updateValues[(int)index].UnsignedValue >> offset * 8) & (int)newFlag) == 0)
		{
			this.m_updateValues[(int)index].UnsignedValue |= (uint)newFlag << offset * 8;
			this.m_updateMask.SetBit((int)index);
		}
	}

	public void RemoveByteFlag(object index, byte offset, object oldFlag)
	{
		if (offset > 4)
		{
			Log.Print(LogType.Error, $"Object.RemoveByteFlag: Wrong offset {offset}", "RemoveByteFlag", "UpdateFieldsArray.cs");
		}
		else if ((((byte)this.m_updateValues[(int)index].UnsignedValue >> offset * 8) & (int)oldFlag) != 0)
		{
			this.m_updateValues[(int)index].UnsignedValue &= ~((uint)oldFlag << offset * 8);
			this.m_updateMask.SetBit((int)index);
		}
	}
}
