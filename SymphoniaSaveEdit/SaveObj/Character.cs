using Newtonsoft.Json;
using SymphoniaSaveEdit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymphoniaSaveEdit.SaveObj
{
    public class Character
    {
        public string Name { get; set; }
        public byte Level { get; set; }
        public ushort HP { get; set; }
        public ushort MaxHP { get; set; }
        public ushort TP { get; set; }
        public ushort MaxTP { get; set; }
        public byte Status { get; set; }
        public uint Exp { get; set; }
        [JsonConverter(typeof(BoolArrayConverter))]
        public bool[] Titles { get; set; }
        public ushort Str { get; set; }
        public ushort Atk { get; set; }
        public ushort Atk2 { get; set; }
        public ushort Int { get; set; }
        public ushort Def { get; set; }
        public ushort Acc { get; set; }
        public ushort Eva { get; set; }
        public ushort Lck { get; set; }
        public ushort Weapon { get; set; }
        public ushort Armor { get; set; }
        public ushort Helm { get; set; }
        public ushort Arms { get; set; }
        public ushort Accessory1 { get; set; }
        public ushort Accessory2 { get; set; }
        public byte Overlimit { get; set; }
        public ushort Affection { get; set; }
        [JsonConverter(typeof(UShortArrayConverter))]
        public ushort[] TechUses { get; set; }
        [JsonConverter(typeof(BoolArrayConverter))]
        public bool[] Techs { get; set; }
        [JsonConverter(typeof(ByteArrayConverter))]
        public byte[] Cooking { get; set; }
        public uint Battles { get; set; }
        public byte BattleItemsUsed { get; set; }
        public byte Deaths { get; set; }
        public ushort CurrentBattles { get; set; }
        public ushort Kills { get; set; }

        public Character()
        {
            Cooking = new byte[24];
        }
    }
}
