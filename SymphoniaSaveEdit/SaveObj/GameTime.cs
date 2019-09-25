using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymphoniaSaveEdit.SaveObj
{
    public class GameTime
    {
        public uint TotalFrames { get { return (uint)(Hours * 60 * 60 * 60 + Minutes * 60 * 60 + Seconds * 60 + Frames); } }
        public ushort Hours { get; set; }
        public byte Minutes { get; set; }
        public byte Seconds { get; set; }
        public byte Frames { get; set; }

        public GameTime()
        {

        }

        public GameTime(uint frames)
        {
            Hours = (ushort)(frames / 60 / 60 / 60);
            frames -= (uint)Hours * 60 * 60 * 60;
            Minutes = (byte)(frames / 60 / 60);
            frames -= (uint)Minutes * 60 * 60;
            Seconds = (byte)(frames / 60);
            frames -= (uint)Seconds * 60;
            Frames = (byte)frames;
        }

        public override string ToString()
        {
            return string.Format("{0:00}h {1:00}m {2:00}.{3}s", Hours, Minutes, Seconds, Frames);
        }
    }
}
