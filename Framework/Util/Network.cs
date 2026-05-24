using System;

namespace Framework.Util
{
    public static class NetworkUtility
    {
        public static uint EndianConvert(uint value)
        {
            var sizeArr = BitConverter.GetBytes(value);
            Array.Reverse(sizeArr);
            return BitConverter.ToUInt32(sizeArr);
        }
        public static ushort EndianConvert(ushort value)
        {
            var sizeArr = BitConverter.GetBytes(value);
            Array.Reverse(sizeArr);
            return BitConverter.ToUInt16(sizeArr);
        }
    }
}
