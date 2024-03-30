using System;
using System.Collections.Generic;

namespace SymphoniaSaveEdit.Utils
{
    public static class Extensions
    {
        public static bool[] ToBoolArray(this byte[] bytes)
        {
            var bools = new List<bool>();

            for (int b = 0; b < bytes.Length; b++)
            {
                for (int n = 7; n >= 0; n--)
                {
                    bools.Add((bytes[b] & (byte)Math.Pow(2, n)) > 0);
                }
            }

            return bools.ToArray();
        }

        public static bool[] ToReversedBoolArray(this byte[] bytes)
        {
            var bools = new List<bool>();

            for (int b = bytes.Length-1; b >= 0; b--)
            {
                for (int n = 7; n >= 0; n--)
                {
                    bools.Add((bytes[b] & (byte)Math.Pow(2, n)) > 0);
                }
            }

            return bools.ToArray();
        }

        public static byte[] To4ByteArray(this bool[] bools, bool bigEndian = false)
        {
            if (bools.Length % 8 != 0)
                throw new ArgumentOutOfRangeException("Boolean array length must be divisible by 8. Otherwise padding of bits is unknown.");

            var byteCount = bools.Length / 8;
            if (bigEndian)
            {
                bool[] copy = new bool[bools.Length];
                Array.Copy(bools, copy, bools.Length);
                // Now reverse bytes
                for (int i = 0; i < byteCount; i++)
                    Array.Copy(copy, i * 8, bools, byteCount * 8 - (i + 1) * 8, 8);
            }

            byte[] bytes = new byte[byteCount];
            for (byte b = 0; b < byteCount; b++)
            {
                for (int n = 7; n >= 0; n--)
                {
                    if (bools[b*8+n])
                        bytes[b] |= (byte)Math.Pow(2, 7-n);
                }
            }

            return bytes;
        }
    }
}
