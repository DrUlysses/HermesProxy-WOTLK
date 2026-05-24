using System;
using Framework.Collections;
using Framework.IO;
using Framework.Logging;

namespace HermesProxy.World.Objects;

public class UpdateFieldsArray
{
    private readonly uint _valuesCount;

    private readonly UpdateValues[] _updateValues;

    public readonly UpdateMask UpdateMask;

    public UpdateFieldsArray(uint size)
    {
        _valuesCount = size;
        _updateValues = new UpdateValues[size];
        UpdateMask = new UpdateMask(size);
    }

    public void WriteToPacket(ByteBuffer buffer)
    {
        var fieldBuffer = new ByteBuffer();
        for (var index = 0; index < _valuesCount; index++)
        {
            if (UpdateMask.GetBit(index))
            {
                fieldBuffer.WriteUInt32(_updateValues[index].UnsignedValue);
            }
        }

        UpdateMask.AppendToPacket(buffer);
        buffer.WriteBytes(fieldBuffer);
    }

    public void SetUpdateField<T>(object index, T value, byte offset = 0) where T : new()
    {
        switch (value)
        {
            case byte when offset > 3:
                Log.Print(LogType.Error, $"SetUpdateField<UInt8>: Wrong offset: {offset}", "UpdateFieldsArray.cs");
                break;
            case byte byteValue:
            {
                if ((byte)(_updateValues[(int)index].UnsignedValue >> offset * 8) != byteValue)
                {
                    _updateValues[(int)index].UnsignedValue &= (uint)~(255 << offset * 8);
                    _updateValues[(int)index].UnsignedValue |= (uint)(byteValue << offset * 8);
                    UpdateMask.SetBit((int)index);
                }

                break;
            }
            case ushort when offset > 1:
                Log.Print(LogType.Error, $"SetUpdateField<UInt16>: Wrong offset: {offset}", "UpdateFieldsArray.cs");
                break;
            case ushort ushortValue:
            {
                if ((ushort)(GetUpdateField<uint>(index) >> offset * 16) != ushortValue)
                {
                    _updateValues[(int)index].UnsignedValue &= (uint)~(65535 << offset * 16);
                    _updateValues[(int)index].UnsignedValue |= (uint)(ushortValue << offset * 16);
                    UpdateMask.SetBit((int)index);
                }

                break;
            }
            case int intValue when _updateValues[(int)index].SignedValue == intValue:
                return;
            case int intValue:
                _updateValues[(int)index].SignedValue = intValue;
                UpdateMask.SetBit((int)index);
                break;
            case uint uintValue when _updateValues[(int)index].UnsignedValue == uintValue:
                return;
            case uint uintValue:
                _updateValues[(int)index].UnsignedValue = uintValue;
                UpdateMask.SetBit((int)index);
                break;
            case float floatValue when _updateValues[(int)index].FloatValue.CompareTo(floatValue) == 0:
                return;
            case float floatValue:
                _updateValues[(int)index].FloatValue = floatValue;
                UpdateMask.SetBit((int)index);
                break;
            case ulong ulongValue when GetUpdateField<ulong>(index) == ulongValue:
                return;
            case ulong ulongValue:
                _updateValues[(int)index].UnsignedValue = MathFunctions.Pair64_LoPart(ulongValue);
                _updateValues[(int)index + 1].UnsignedValue = MathFunctions.Pair64_HiPart(ulongValue);
                UpdateMask.SetBit((int)index);
                UpdateMask.SetBit((int)index + 1);
                break;
            default:
            {
                if (value is not WowGuid128 guid)
                {
                    throw new Exception("Unhandled type " + typeof(T) + " in SetUpdateField!");
                }

                SetUpdateField(index, guid.GetLowValue());
                SetUpdateField((int)index + 2, guid.GetHighValue());
                break;
            }
        }
    }

    public T GetUpdateField<T>(object index, byte offset = 0)
    {
        var val = default(T);
        var result = val switch
        {
            byte =>
                (T)Convert.ChangeType(
                    (byte)(_updateValues[(int)index].UnsignedValue >> offset * 8) & 0xFF,
                    typeof(T)
                ),
            ushort =>
                (T)Convert.ChangeType(
                    (ushort)(_updateValues[(int)index].UnsignedValue >> offset * 16) & 0xFFFF,
                    typeof(T)
                ),
            int =>
                (T)Convert.ChangeType(
                    _updateValues[(int)index].SignedValue,
                    typeof(T)
                ),
            uint =>
                (T)Convert.ChangeType(
                    _updateValues[(int)index].UnsignedValue,
                    typeof(T)
                ),
            float =>
                (T)Convert.ChangeType(
                    _updateValues[(int)index].FloatValue,
                    typeof(T)
                ),
            ulong =>
                (T)Convert.ChangeType(
                    ((ulong)_updateValues[(int)index + 1].UnsignedValue << 32) |
                    _updateValues[(int)index].UnsignedValue,
                    typeof(T)
                ),
            WowGuid128 =>
                (T)Convert.ChangeType(
                    new WowGuid128(GetUpdateField<ulong>((int)index + 2),
                        GetUpdateField<ulong>(index)), typeof(T)
                ),
            _ => throw new Exception($"{typeof(T)} is not implemented in GetUpdateField<T>")
        };

        return result;
    }

    public void _LoadIntoDataField(string data, uint startOffset, uint count)
    {
        if (string.IsNullOrEmpty(data))
        {
            return;
        }

        var lines = new StringArray(data, ' ');
        if (lines.Length != count)
        {
            return;
        }

        for (var index = 0; index < count; index++)
        {
            if (!uint.TryParse(lines[index], out var value)) continue;
            _updateValues[(int)startOffset + index].UnsignedValue = value;
            UpdateMask.SetBit((int)(startOffset + index));
        }
    }

    public bool HasFlag(object index, object flag)
    {
        if ((int)index >= _valuesCount)
        {
            return false;
        }

        return (GetUpdateField<uint>(index) & (uint)flag) != 0;
    }

    private void AddFlag(object index, object newFlag)
    {
        var oldValue = _updateValues[(int)index].UnsignedValue;
        var newValue = oldValue | Convert.ToUInt32(newFlag);
        if (oldValue != newValue)
        {
            SetUpdateField(index, newValue);
        }
    }

    private void RemoveFlag(object index, object newFlag)
    {
        var oldValue = _updateValues[(int)index].UnsignedValue;
        var newValue = oldValue & ~Convert.ToUInt32(newFlag);
        if (oldValue != newValue)
        {
            SetUpdateField(index, newValue);
        }
    }

    public void ApplyFlag<T>(object index, T flag, bool apply)
    {
        if (apply)
        {
            AddFlag(index, flag);
        }
        else
        {
            RemoveFlag(index, flag);
        }
    }

    public void AddFlag64(object index, object newFlag)
    {
        var oldValue = GetUpdateField<ulong>(index);
        var newValue = oldValue | Convert.ToUInt64(newFlag);
        if (oldValue != newValue)
        {
            SetUpdateField(index, newValue);
        }
    }

    public void RemoveFlag64(object index, object newFlag)
    {
        var oldValue = GetUpdateField<ulong>(index);
        var newValue = oldValue & ~Convert.ToUInt64(newFlag);
        if (oldValue != newValue)
        {
            SetUpdateField(index, newValue);
        }
    }

    public void ApplyFlag64<T>(object index, T flag, bool apply)
    {
        if (apply)
        {
            AddFlag(index, flag);
        }
        else
        {
            RemoveFlag(index, flag);
        }
    }

    public void AddByteFlag(object index, byte offset, object newFlag)
    {
        if (offset > 4)
        {
            Log.Print(LogType.Error, $"Object.SetByteFlag: Wrong offset {offset}", "UpdateFieldsArray.cs");
        }
        else if ((((byte)_updateValues[(int)index].UnsignedValue >> offset * 8) & (int)newFlag) == 0)
        {
            _updateValues[(int)index].UnsignedValue |= (uint)newFlag << offset * 8;
            UpdateMask.SetBit((int)index);
        }
    }

    public void RemoveByteFlag(object index, byte offset, object oldFlag)
    {
        if (offset > 4)
        {
            Log.Print(LogType.Error, $"Object.RemoveByteFlag: Wrong offset {offset}", "UpdateFieldsArray.cs");
        }
        else if ((((byte)_updateValues[(int)index].UnsignedValue >> offset * 8) & (int)oldFlag) != 0)
        {
            _updateValues[(int)index].UnsignedValue &= ~((uint)oldFlag << offset * 8);
            UpdateMask.SetBit((int)index);
        }
    }
}