using System;

namespace SymphoniaSaveEdit.SaveObj
{
    internal class SwitchSave : SaveFile
    {
        public SwitchSave(string filename) : base(SaveType.Switch) 
        {
            OpenSave(filename);
        }

        public override void CalculateChecksum(int slot = 0)
        {
            // Switch saves are incorrectly skipping the last 28 bytes
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
