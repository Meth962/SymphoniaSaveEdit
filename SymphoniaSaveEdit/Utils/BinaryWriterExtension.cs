using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymphoniaSaveEdit.Utils
{
    public static class BinaryWriterExtension
    {
        public static void WriteReverse(this BinaryWriter bw, uint number)
        {
            byte[] bytes = BitConverter.GetBytes(number);
            Array.Reverse(bytes);
            bw.Write(bytes);
        }

        public static void WriteReverse(this BinaryWriter bw, ushort number)
        {
            byte[] bytes = BitConverter.GetBytes(number);
            Array.Reverse(bytes);
            bw.Write(bytes);
        }
    }
}
