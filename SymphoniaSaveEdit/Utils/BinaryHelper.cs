using System;

namespace SymphoniaSaveEdit.Utils
{
    public class BinaryHelper
    {
        public static uint ReverseUInt(byte[] bytes)
        {
            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static short ReverseShort(byte[] bytes)
        {
            Array.Reverse(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }

        public static ushort ReverseUShort(byte[] bytes)
        {
            Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes, 0);
        }

        public static byte[] ReverseBytes(byte[] bytes)
        {
            Array.Reverse(bytes);
            return bytes;
        }

        public static uint GetOffset(uint offset, SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.Switch:
                    return offset - 0xA0;
                default:
                    return offset;
            }
        }

        public static int ReverseInt(byte[] bytes)
        {
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static UInt32 ReverseBytes(UInt32 value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }
    }
}
