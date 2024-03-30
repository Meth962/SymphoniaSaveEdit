using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymphoniaSaveEdit.SaveObj
{
    internal class SwitchSave : SaveFile
    {
        public SwitchSave(string filename) : base(SaveType.Switch) 
        {
            OpenSave(filename);
        }
    }
}
