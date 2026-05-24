using System;

namespace Framework.Util
{
    public static class NetworkUtility
    {
        public static uint EndianConvert(uint value)
        {
            byte[] sizeArr = BitConverter.GetBytes(value);
            Array.Reverse(sizeArr);
            return BitConverter.ToUInt32(sizeArr);
        }
        public static ushort EndianConvert(ushort value)
        {
            byte[] sizeArr = BitConverter.GetBytes(value);
            Array.Reverse(sizeArr);
            return BitConverter.ToUInt16(sizeArr);
        }
    }
}
