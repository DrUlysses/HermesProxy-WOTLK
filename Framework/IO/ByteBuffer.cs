/*
 * Copyright (C) 2012-2020 CypherCore <http://github.com/CypherCore>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using Framework.GameMath;
using System;
using System.IO;
using System.Text;

namespace Framework.IO
{
    public class ByteBuffer : IDisposable
    {
        public ByteBuffer()
        {
            _writeStream = new BinaryWriter(new MemoryStream());
        }

        public ByteBuffer(byte[] data)
        {
            _readStream = new BinaryReader(new MemoryStream(data));
        }

        public void Dispose()
        {
            _writeStream?.Dispose();

            _readStream?.Dispose();
        }

        #region Read Methods
        public sbyte ReadInt8()
        {
            ResetBitPos();
            return _readStream.ReadSByte();
        }

        public short ReadInt16()
        {
            ResetBitPos();
            return _readStream.ReadInt16();
        }

        public int ReadInt32()
        {
            ResetBitPos();
            return _readStream.ReadInt32();
        }

        public long ReadInt64()
        {
            ResetBitPos();
            return _readStream.ReadInt64();
        }

        public byte ReadUInt8()
        {
            ResetBitPos();
            return _readStream.ReadByte();
        }

        public byte PeekByte()
        {
            long pos = _readStream.BaseStream.Position;
            byte val = _readStream.ReadByte();
            _readStream.BaseStream.Position = pos;
            return val;
        }

        public ushort ReadUInt16()
        {
            ResetBitPos();
            return _readStream.ReadUInt16();
        }

        public uint ReadUInt32()
        {
            ResetBitPos();
            return _readStream.ReadUInt32();
        }

        public ulong ReadUInt64()
        {
            ResetBitPos();
            return _readStream.ReadUInt64();
        }

        public float ReadFloat()
        {
            ResetBitPos();
            return _readStream.ReadSingle();
        }

        public double ReadDouble()
        {
            ResetBitPos();
            return _readStream.ReadDouble();
        }

        public T ReadByteEnum<T>() where T: Enum
        {
            return (T)(object) ReadUInt8();
        }

        public string ReadCString()
        {
            ResetBitPos();
            MemoryStream stream = (MemoryStream)_readStream.BaseStream;
            StringBuilder tmpString = new StringBuilder();

            while (stream.Position < stream.Length)
            {
                byte next = _readStream.ReadByte();
                if (next == 0)
                {
                    break;
                }

                tmpString.Append((char)next);
            }

            return tmpString.ToString();
        }

        public string ReadString(uint length)
        {
            if (length == 0)
                return "";

            ResetBitPos();
            return Encoding.UTF8.GetString(ReadBytes(length));
        }

        public bool ReadBool()
        {
            ResetBitPos();
            return _readStream.ReadBoolean();
        }

        public byte[] ReadBytes(uint count)
        {
            ResetBitPos();
            return _readStream.ReadBytes((int)count);
        }

        public void Skip(int count)
        {
            ResetBitPos();
            _readStream.BaseStream.Position += count;
        }

        public bool CanRead()
        {
            return GetCurrentStream().Position != GetCurrentStream().Length;
        }

        public uint ReadPackedTime()
        {
            return (uint)Time.GetUnixTimeFromPackedTime(ReadUInt32());
        }

        public DateTime ReadTime()
        {
            return DateTimeOffset.FromUnixTimeSeconds(ReadUInt32()).DateTime;
        }

        public DateTime ReadTime64()
        {
            return DateTimeOffset.FromUnixTimeSeconds((int)ReadUInt64()).DateTime;
        }

        public Vector2 ReadVector2()
        {
            return new Vector2(ReadFloat(), ReadFloat());
        }

        public Vector3 ReadVector3()
        {
            return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
        }

        public Vector3 ReadPackedVector3()
        {
            int packed = ReadInt32();
            float x = ((packed & 0x7FF) << 21 >> 21) * 0.25f;
            float y = ((((packed >> 11) & 0x7FF) << 21) >> 21) * 0.25f;
            float z = ((packed >> 22 << 22) >> 22) * 0.25f;
            return new Vector3(x, y, z);
        }

        public Vector4 ReadVector4()
        {
            return new Vector4(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
        }

        public Quaternion ReadPackedQuaternion()
        {
            long packed = ReadInt64();
            return new Quaternion(packed);
        }

        public Quaternion ReadQuaternion()
        {
            return new Quaternion(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
        }

        //BitPacking
        public bool ReadBit()
        {
            if (_bitPosition == 8)
            {
                _bitValue = ReadUInt8();
                _bitPosition = 0;
            }

            int returnValue = _bitValue;
            _bitValue = (byte)(2 * returnValue); // BitValue <<= 1;
            ++_bitPosition;

            return (returnValue >> 7) != 0;
        }

        public bool HasBit()
        {
            if (_bitPosition == 8)
            {
                _bitValue = ReadUInt8();
                _bitPosition = 0;
            }

            int returnValue = _bitValue;
            _bitValue = (byte)(2 * returnValue);
            ++_bitPosition;

            return Convert.ToBoolean(returnValue >> 7);
        }

        public T ReadBits<T>(int bitCount)
        {
            int value = 0;

            for (var i = bitCount - 1; i >= 0; --i)
                if (HasBit())
                    value |= (1 << i);

            return (T)Convert.ChangeType(value, typeof(T));
        }
        #endregion

        #region Write Methods
        public void WriteInt8(sbyte data)
        {
            FlushBits();
            _writeStream.Write(data);
        }

        public void WriteInt16(short data)
        {
            FlushBits();
            _writeStream.Write(data);
        }

        public void WriteInt32(int data)
        {
            FlushBits();
            _writeStream.Write(data);
        }

        public void WriteInt64(long data)
        {
            FlushBits();
            _writeStream.Write(data);
        }

        public void WriteBool(bool data)
        {
            FlushBits();
            _writeStream.Write(data);
        }

        public void WriteUInt8(byte data)
        {
            FlushBits();
            _writeStream.Write(data);
        }

        public void WriteUInt16(ushort data)
        {
            FlushBits();
            _writeStream.Write(data);
        }

        public void WriteUInt32(uint data)
        {
            FlushBits();
            _writeStream.Write(data);
        }

        public void WriteUInt64(ulong data)
        {
            FlushBits();
            _writeStream.Write(data);
        }

        public void WriteFloat(float data)
        {
            FlushBits();
            _writeStream.Write(data);
        }

        public void WriteDouble(double data)
        {
            FlushBits();
            _writeStream.Write(data);
        }

        /// <summary>
        /// Writes a string to the packet with a null terminated (0)
        /// </summary>
        /// <param name="str"></param>
        public void WriteCString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                WriteUInt8(0);
                return;
            }

            WriteString(str);
            WriteUInt8(0);
        }

        public void WriteString(string str)
        {
            if (str.IsEmpty())
                return;

            byte[] sBytes = Encoding.UTF8.GetBytes(str);
            WriteBytes(sBytes);
        }

        public void WriteBytes(byte[] data)
        {
            FlushBits();
            _writeStream.Write(data, 0, data.Length);
        }

        public void WriteBytes(byte[] data, uint count)
        {
            FlushBits();
            _writeStream.Write(data, 0, (int)count);
        }

        public void WriteBytes(ByteBuffer buffer)
        {
            WriteBytes(buffer.GetData());
        }

        public void WriteVector4(Vector4 pos)
        {
            WriteFloat(pos.X);
            WriteFloat(pos.Y);
            WriteFloat(pos.Z);
            WriteFloat(pos.W);
        }

        public void WriteVector3(Vector3 pos)
        {
            WriteFloat(pos.X);
            WriteFloat(pos.Y);
            WriteFloat(pos.Z);
        }

        public void WriteVector2(Vector2 pos)
        {
            WriteFloat(pos.X);
            WriteFloat(pos.Y);
        }

        public void WritePackXYZ(Vector3 pos)
        {
            uint packed = 0;
            packed |= ((uint)(pos.X / 0.25f) & 0x7FF);
            packed |= ((uint)(pos.Y / 0.25f) & 0x7FF) << 11;
            packed |= ((uint)(pos.Z / 0.25f) & 0x3FF) << 22;
            WriteUInt32(packed);
        }

        public bool WriteBit(bool bit)
        {
            --_bitPosition;

            if (bit)
                _bitValue |= (byte)(1 << _bitPosition);

            if (_bitPosition == 0)
            {
                _writeStream.Write(_bitValue);

                _bitPosition = 8;
                _bitValue = 0;
            }
            return bit;
        }

        public void WriteBits(object bit, int count)
        {
            for (int i = count - 1; i >= 0; --i)
                WriteBit(((Convert.ToUInt32(bit) >> i) & 1) != 0);
        }

        public void WritePackedTime(long time)
        {
            WriteUInt32(Time.GetPackedTimeFromUnixTime(time));
        }

        public void WritePackedTime()
        {
            WriteUInt32(Time.GetPackedTimeFromDateTime(DateTime.Now));
        }

        public void WriteByteEnum<T>(T x) where T: Enum
        {
            WriteUInt8((byte)(object) x);
        }

        public void WriteUint32Enum<T>(T x) where T: Enum
        {
            WriteUInt32((uint)(object) x);
        }

        #endregion

        public bool HasUnfinishedBitPack()
        {
            return _bitPosition != 8;
        }

        public void FlushBits()
        {
            if (_bitPosition == 8)
                return;

            _writeStream.Write(_bitValue);
            _bitValue = 0;
            _bitPosition = 8;
        }

        public void ResetBitPos()
        {
            if (_bitPosition > 7)
                return;

            _bitPosition = 8;
            _bitValue = 0;
        }

        public void ResetReadPos()
        {
            _readStream.BaseStream.Position = 0;
            _readStream.BaseStream.Seek(0, SeekOrigin.Begin);
            ResetBitPos();
        }

        public byte[] ReadToEnd()
        {
            Stream stream = GetCurrentStream();
            var length = (uint)(stream.Length - stream.Position);
            return ReadBytes(length);
        }

        public byte[] GetData()
        {
            Stream stream = GetCurrentStream();

            var data = new byte[stream.Length];

            long pos = stream.Position;
            stream.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)stream.ReadByte();

            stream.Seek(pos, SeekOrigin.Begin);
            return data;
        }

        public uint GetSize()
        {
            return (uint)GetCurrentStream().Length;
        }

        public Stream GetCurrentStream()
        {
            return _writeStream != null ? _writeStream.BaseStream : _readStream.BaseStream;
        }

        public void Clear()
        {
            _bitPosition = 8;
            _bitValue = 0;
            _writeStream = new BinaryWriter(new MemoryStream());
        }

        private byte _bitPosition = 8;
        private byte _bitValue;
        private BinaryWriter _writeStream;
        private readonly BinaryReader _readStream;

        // Hex Printer from WPP
        // https://github.com/TrinityCore/WowPacketParser/blob/7edfda7e4daf9a5b9069083806a9a3c261dea8a7/WowPacketParser/Misc/Utilities.cs#L48
        public void DebugPrintHex()
        {
            const bool shortVersion = false;
            const int offset = 0;

            var data = GetData();
            
            var n = Environment.NewLine;

            var prefix = new string(' ', offset);

            var hexDump = new StringBuilder(prefix);

            var header = "|-------------------------------------------------|---------------------------------|" + n +
                         "| 00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F | 0 1 2 3 4 5 6 7 8 9 A B C D E F |" + n +
                         "|-------------------------------------------------|---------------------------------|" + n;

            hexDump.Append(header);

            for (var i = 0; i < data.Length; i += 16)
            {
                var text = new StringBuilder();
                var hex = new StringBuilder(i == 0 ? "" : prefix);

                hex.Append("| ");

                for (var j = 0; j < 16; j++)
                {
                    if (j + i < data.Length)
                    {
                        var val = data[j + i];
                        hex.Append(data[j + i].ToString("X2"));

                        hex.Append(' ');

                        if (val is >= 32 and <= 127)
                            text.Append((char)val);
                        else
                            text.Append('.');

                        text.Append(' ');
                    }
                    else
                    {
                        hex.Append(shortVersion ? "  " : "   ");
                        text.Append(shortVersion ? " " : "  ");
                    }
                }

                hex.Append(shortVersion ? "|" : "| ");
                hex.Append(text);
                hex.Append('|');
                hex.Append(n);
                hexDump.Append(hex);
            }

            hexDump.Append("|-------------------------------------------------|---------------------------------|");

            Console.WriteLine(hexDump);
        }
    }
}
