using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymphoniaSaveEdit.SaveObj
{
    internal class PS4Save : SaveFile
    {
        public PS4Save(string filename) : base(SaveType.PS4) 
        {
            OpenSave(filename);
        }

        public override void CalculateChecksum(int slot = 0)
        {
            // PS4 saves are incorrectly skipping the last 28 bytes
            uint checksum = 0;
            // only skip first 4 bytes which is checksum
            for (int i = 4; i < 0x25f8; i += 4)
            {
                checksum += BitConverter.ToUInt32(bytes, i);
            }
            Checksum1 = checksum;
        }
    }
}
