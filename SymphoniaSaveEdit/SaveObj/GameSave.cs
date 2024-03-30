using Newtonsoft.Json;
using SymphoniaSaveEdit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymphoniaSaveEdit.SaveObj
{
    public class GameSave
    {
        [JsonIgnore]
        public string SaveName { get; set; }
        [JsonIgnore]
        public DateTime LastModified { get; set; }

        public uint Checksum1 { get; set; }
        public uint Checksum2 { get; set; }

        public GameTime MaxGameTime { get; set; }
        public uint MaxGald { get; set; }
        public uint TotalGaldUsed { get; set; }
        public uint Saves { get; set; }
        public uint SavesCurrent { get; set; }
        public uint GameCleared { get; set; }

        public uint MaxEncounters { get; set; }
        public uint Escapes { get; set; }
        public ushort MaxCombo { get; set; }
        public uint MaxDmg { get; set; }
        public double MaxGrade { get; set; }

        public GameTime GameTime { get; set; }
        public GameTime GameTimeCurrent { get; set; }
        public uint Gald { get; set; }
        public uint GaldCurrent { get; set; }
        public ushort Encounters { get; set; }
        public ushort EncountersCurrent { get; set; }
        public double Grade { get; set; }
        public ushort Combo { get; set; }
        public ushort ComboCurrent { get; set; }

        [JsonConverter(typeof(ByteHexArrayConverter))]
        public byte[] MonsterList { get; set; }
        [JsonConverter(typeof(ByteHexArrayConverter))]
        public byte[] CollectorsBook { get; set; }
        [JsonConverter(typeof(BoolArrayConverter))]
        public bool[] Recipes { get; set; }
        public byte EncounterMode { get; set; }
        public byte UnisonGauge { get; set; }

        public Character[] Characters { get; set; }

        [JsonConverter(typeof(BoolArrayConverter))]
        public bool[] Treasure { get; set; }
        [JsonConverter(typeof(BoolArrayConverter))]
        public bool[] DogLover { get; set; }
        [JsonConverter(typeof(BoolArrayConverter))]
        public bool[] Gigolo { get; set; }

        public uint Battles { get; set; }
        public ushort HardModeBattles { get; set; }
        [JsonIgnore]
        public byte Challenges { get; set; }
        public bool DefaultEquip { get; set; }
        public bool GelsUsed { get; set; }
        public bool HaveDied { get; set; }

        [JsonConverter(typeof(ByteArrayConverter))]
        public byte[] Party { get; set; }
        [JsonConverter(typeof(ByteArrayConverter))]
        public byte[] Items { get; set; }

        [JsonConverter(typeof(ByteHexArrayConverter))]
        public byte[] FigurineBook { get; set; }

        public GameSave(SaveType saveType = SaveType.PCRaw)
        {
            MonsterList = new byte[257];
            CollectorsBook = new byte[67];
            FigurineBook = new byte[36];
            Characters = new Character[9];
            for (int i = 0; i < 9; i++)
            {
                Characters[i] = new Character();
                Characters[i].Name = Globals.CharacterNames[i];
                switch (i)
                {
                    case 0:
                    case 2:
                        Characters[i].TechUses = new ushort[35];
                        break;
                    case 1:
                    case 3:
                        Characters[i].TechUses = new ushort[25];
                        break;
                    case 4:
                        Characters[i].TechUses = new ushort[29];
                        break;
                    case 5:
                    case 8:
                        Characters[i].TechUses = new ushort[27];
                        break;
                    case 6:
                        Characters[i].TechUses = new ushort[20];
                        break;
                    case 7:
                        Characters[i].TechUses = new ushort[22];
                        break;
                }
            }
            Treasure = new bool[271];
            DogLover = new bool[40];
            Gigolo = new bool[40];
            Party = new byte[8];
            Items = new byte[Globals.ItemNames[saveType].Length];
        }
    }
}
