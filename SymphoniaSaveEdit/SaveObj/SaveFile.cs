using SymphoniaSaveEdit.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace SymphoniaSaveEdit.SaveObj
{
    internal abstract class SaveFile
    {
        public SaveType saveType = SaveType.PCRaw;
        public uint Checksum1 = 0;
        public uint Checksum2 = 0;

        internal byte[] bytes;
        internal bool bigEndian = false;
        
        public List<GameSave> Saves = new List<GameSave>();

        protected SaveFile() { }
        protected SaveFile(SaveType saveType)
        {
            this.saveType = saveType;
        }

        public virtual void OpenSave(string[] filenames)
        {
            OpenSave(filenames[0]);
        }

        public virtual void OpenSave(string filename)
        {
            Saves.Clear();
            bytes = File.ReadAllBytes(filename);

            StreamReader sr = new StreamReader(new MemoryStream(bytes));
            BinaryReader br = new BinaryReader(sr.BaseStream);

            GameSave save = new GameSave(saveType);

            save.Checksum1 = br.ReadUInt32();
            save.Checksum2 = br.ReadUInt32();
            br.BaseStream.Seek(0x10, SeekOrigin.Begin);
            save.GameTime = new GameTime(br.ReadUInt32());
            save.Gald = br.ReadUInt32();
            save.Encounters = br.ReadUInt16();
            save.Combo = br.ReadUInt16();
            // read party here as well (only for save header?)
            br.BaseStream.Seek(0x10, SeekOrigin.Current);

            // Records
            save.MaxGameTime = new GameTime(br.ReadUInt32());
            save.MaxGald = br.ReadUInt32();
            save.TotalGaldUsed = br.ReadUInt32();
            save.Saves = br.ReadUInt32();
            save.GameCleared = br.ReadUInt32();
            save.MaxEncounters = br.ReadUInt32();
            save.Escapes = br.ReadUInt32();
            save.MaxCombo = br.ReadUInt16();
            br.ReadBytes(2);
            save.MaxDmg = br.ReadUInt32();
            save.MaxGrade = br.ReadUInt32() / 100.0;

            // Battles
            save.Battles = br.ReadUInt32();
            for (int i = 0; i < 9; i++)
                save.Characters[i].Battles = br.ReadUInt32();

            // Current
            br.BaseStream.Seek(0x1d0, SeekOrigin.Begin);
            save.GaldCurrent = br.ReadUInt32();
            save.EncountersCurrent = br.ReadUInt16();
            save.ComboCurrent = br.ReadUInt16();
            save.GameTimeCurrent = new GameTime(br.ReadUInt32());
            save.SavesCurrent = br.ReadUInt32();
            for (int i = 0; i < 257; i++)
                save.MonsterList[i] = br.ReadByte();

            // Characters
            br.BaseStream.Seek(0x4a8, SeekOrigin.Begin);
            for (int i = 0; i < 9; i++)
            {
                save.Characters[i].Level = br.ReadByte();
                br.ReadBytes(7);
                save.Characters[i].Exp = br.ReadUInt32();
                br.ReadBytes(3);
                save.Characters[i].Status = br.ReadByte();
                save.Characters[i].Titles = br.ReadBytes(4).ToBoolArray();
                //br.ReadByte();// this may be extra titles
                br.ReadBytes(2);
                save.Characters[i].HP = br.ReadUInt16();
                save.Characters[i].TP = br.ReadUInt16();
                br.ReadBytes(12);
                save.Characters[i].MaxHP = br.ReadUInt16();
                save.Characters[i].MaxTP = br.ReadUInt16();
                save.Characters[i].Str = br.ReadUInt16();
                save.Characters[i].Atk = br.ReadUInt16();
                save.Characters[i].Atk2 = br.ReadUInt16();
                save.Characters[i].Def = br.ReadUInt16();
                save.Characters[i].Lck = br.ReadUInt16();
                save.Characters[i].Acc = br.ReadUInt16();
                save.Characters[i].Eva = br.ReadUInt16();
                save.Characters[i].Int = br.ReadUInt16();

                save.Characters[i].Weapon = br.ReadUInt16();
                save.Characters[i].Armor = br.ReadUInt16();
                save.Characters[i].Helm = br.ReadUInt16();
                save.Characters[i].Arms = br.ReadUInt16();
                save.Characters[i].Accessory1 = br.ReadUInt16();
                save.Characters[i].Accessory2 = br.ReadUInt16();

                save.Characters[i].Overlimit = br.ReadByte();
                br.ReadByte();
                save.Characters[i].Affection = br.ReadUInt16();
                //br.BaseStream.Seek(0x2c, SeekOrigin.Current);//0x2c,0x2d
                //seek 0xb, read 5 tech mask bytes
                br.BaseStream.Seek(0x16, SeekOrigin.Current);
                save.Characters[i].Techs = br.ReadBytes(5).ToBoolArrayLow();

                br.BaseStream.Seek(0x13, SeekOrigin.Current);
                for (int t = 0; t < save.Characters[i].TechUses.Length; t++)
                {
                    save.Characters[i].TechUses[t] = br.ReadUInt16();
                }

                //br.BaseStream.Seek(0xa0 - save.Characters[i].TechUses.Length * 2, SeekOrigin.Current);//0xCC-0x2e-techs*2
                br.BaseStream.Seek(0x6c - save.Characters[i].TechUses.Length * 2, SeekOrigin.Current);
                for (int c = 0; c < 24; c++)
                {
                    save.Characters[i].Cooking[c] = br.ReadByte();
                }

                br.BaseStream.Seek(0x1c, SeekOrigin.Current);
            }

            byte flags;
            // Dogs
            br.BaseStream.Seek(0xF14, SeekOrigin.Begin);
            for (int x = 0; x < 5; x++)
            {
                flags = br.ReadByte();
                if (x != 0)
                {
                    save.DogLover[x * 8] = (flags & 1) == 1;
                    save.DogLover[x * 8 + 1] = (flags & 2) == 2;
                    save.DogLover[x * 8 + 2] = (flags & 4) == 4;
                    save.DogLover[x * 8 + 3] = (flags & 8) == 8;
                    if (x != 4)
                    {
                        save.DogLover[x * 8 + 4] = (flags & 0x10) == 0x10;
                        save.DogLover[x * 8 + 5] = (flags & 0x20) == 0x20;
                    }
                }
                if (x != 4)
                {
                    save.DogLover[x * 8 + 6] = (flags & 0x40) == 0x40;
                    save.DogLover[x * 8 + 7] = (flags & 0x80) == 0x80;
                }
            }

            // Women
            br.BaseStream.Seek(0xf64, SeekOrigin.Begin);
            for (int x = 0; x < 4; x++)
            {
                flags = br.ReadByte();
                if (x != 0)
                {
                    if (x != 3)
                        save.Gigolo[x * 8] = (flags & 1) == 1;
                    save.Gigolo[x * 8 + 1] = (flags & 2) == 2;
                    if (x != 1)
                    {
                        save.Gigolo[x * 8 + 2] = (flags & 4) == 4;
                        save.Gigolo[x * 8 + 3] = (flags & 8) == 8;
                        save.Gigolo[x * 8 + 4] = (flags & 0x10) == 0x10;
                    }
                }
                save.Gigolo[x * 8 + 5] = (flags & 0x20) == 0x20;
                save.Gigolo[x * 8 + 6] = (flags & 0x40) == 0x40;
                save.Gigolo[x * 8 + 7] = (flags & 0x80) == 0x80;
            }
            save.Gigolo[32] = (br.ReadByte() & 1) == 1;

            // Treasure
            br.BaseStream.Seek(0xFFE, SeekOrigin.Begin);
            flags = br.ReadByte();
            save.Treasure[0] = (flags & 4) == 4;
            save.Treasure[1] = (flags & 8) == 8;
            save.Treasure[2] = (flags & 0x10) == 0x10;
            save.Treasure[3] = (flags & 0x20) == 0x20;
            save.Treasure[4] = (flags & 0x40) == 0x40;
            save.Treasure[5] = (flags & 0x80) == 0x80;
            for (int x = 0; x < 33; x++)
            {
                flags = br.ReadByte();
                save.Treasure[x * 8 + 6] = (flags & 1) == 1;
                if (x != 29) // bypass glitch
                    save.Treasure[x * 8 + 7] = (flags & 2) == 2;
                else
                    save.Treasure[x * 8 + 7] = false;
                save.Treasure[x * 8 + 8] = (flags & 4) == 4;
                save.Treasure[x * 8 + 9] = (flags & 8) == 8;
                save.Treasure[x * 8 + 10] = (flags & 0x10) == 0x10;
                save.Treasure[x * 8 + 11] = (flags & 0x20) == 0x20;
                save.Treasure[x * 8 + 12] = (flags & 0x40) == 0x40;
                save.Treasure[x * 8 + 13] = (flags & 0x80) == 0x80;
            }
            flags = br.ReadByte();
            save.Treasure[270] = (flags & 1) == 1;

            br.BaseStream.Seek(0x107D, SeekOrigin.Begin);
            for (int i = 0; i < 8; i++)
                save.Party[i] = br.ReadByte();

            // Items
            br.BaseStream.Seek(0x108E, SeekOrigin.Begin);
            for (int i = 0; i < Globals.ItemNames[saveType].Length; i++)
                save.Items[i] = br.ReadByte();

            // Collector's Book
            br.BaseStream.Seek(0x1304, SeekOrigin.Begin);
            for (int i = 0; i < 0x43; i++)
                save.CollectorsBook[i] = br.ReadByte();

            br.BaseStream.Seek(0x1C0F, SeekOrigin.Begin); // 1caf, maybe 1cb0 but fat chance
            save.Recipes = br.ReadBytes(4).ToBoolArray();
            br.ReadBytes(7);
            save.EncounterMode = br.ReadByte();

            // Battles
            br.BaseStream.Seek(0x1C36, SeekOrigin.Begin);
            for (int i = 0; i < 9; i++)
                save.Characters[i].CurrentBattles = br.ReadUInt16();

            save.UnisonGauge = br.ReadByte();
            br.ReadBytes(2);
            // Challenges, no gels, default equip, deaths, etc. for titles
            save.Challenges = br.ReadByte();
            save.GelsUsed = (save.Challenges & 8) == 8;
            save.HardmodeOnly = (save.Challenges & 4) == 4;
            save.HaveDied = (save.Challenges & 2) == 2;
            save.DefaultEquip = (save.Challenges & 1) == 1;
            br.ReadBytes(2);
            save.HardModeBattles = br.ReadUInt16();

            // Deaths
            br.BaseStream.Seek(0x1c64, SeekOrigin.Begin);
            for (int i = 0; i < 9; i++)
                save.Characters[i].Deaths = br.ReadByte();
            // Battle Items Used
            for (int i = 0; i < 9; i++)
                save.Characters[i].BattleItemsUsed = br.ReadByte();
            // Kills
            for (int i = 0; i < 9; i++)
                save.Characters[i].Kills = br.ReadUInt16();
            // Figurine Book
            br.ReadBytes(4);
            for (int i = 0; i < 36; i++)
                save.FigurineBook[i] = br.ReadByte();

            // Grade 0x1d84 / 1dc4
            br.BaseStream.Seek(0x1D24, SeekOrigin.Begin); // -4 spent
            save.Grade = br.ReadUInt32() / 100.0;

            switch (saveType)
            {
                case SaveType.PCRaw:
                    save.SaveName = "PC Save";
                    break;
                case SaveType.Switch:
                    save.SaveName = "Switch Save";
                    break;
                case SaveType.PS4:
                    save.SaveName = "PS4 Save";
                    break;
                default:
                    save.SaveName = "Raw Save";
                    break;
            }

            CalculateChecksum();

            Saves.Add(save);
        }

        public virtual void WriteSave(string filename, int slot = 0) 
        {
            MemoryStream ms = new MemoryStream(bytes);
            BinaryWriter bw = new BinaryWriter(ms);

            GameSave save = Saves[slot];

            // Saves that use checksum header; PS4 does, PC/Switch do not care
            if (saveType == SaveType.PS4)
            {
                bw.Seek(0, SeekOrigin.Begin);
                bw.Write((uint)0);
                bw.Write((uint)0);
            }

            bw.BaseStream.Seek(0x10, SeekOrigin.Begin);
            bw.Write(save.GameTime.TotalFrames);
            bw.Write(save.Gald);
            bw.Write(save.Encounters);
            bw.Write(save.Combo);
            bw.BaseStream.Seek(0x10, SeekOrigin.Current);

            // Records
            bw.Write(save.MaxGameTime.TotalFrames);
            bw.Write(save.MaxGald);
            bw.Write(save.TotalGaldUsed);
            bw.Write(save.Saves);
            bw.Write(save.GameCleared);
            bw.Write(save.MaxEncounters);
            bw.Write(save.Escapes);
            bw.Write(save.MaxCombo);
            bw.BaseStream.Seek(2, SeekOrigin.Current);
            bw.Write(save.MaxDmg);
            bw.Write((uint)(save.MaxGrade * 100));

            // Battles
            bw.Write(save.Battles);
            for (int i = 0; i < 9; i++)
                bw.Write(save.Characters[i].Battles);

            // Current
            bw.BaseStream.Seek(0x1d0, SeekOrigin.Begin);
            bw.Write(save.GaldCurrent);
            bw.Write(save.EncountersCurrent);
            bw.Write(save.ComboCurrent);
            bw.Write((uint)(save.GameTimeCurrent.TotalFrames));
            bw.Write(save.SavesCurrent);
            for (int i = 0; i < 257; i++)
                bw.Write(save.MonsterList[i]);

            // Characters
            byte[] work;
            bw.BaseStream.Seek(0x4a8, SeekOrigin.Begin); // 0x548
            for (int i = 0; i < 9; i++)
            {
                bw.Write(save.Characters[i].Level);
                bw.BaseStream.Seek(7, SeekOrigin.Current);
                bw.Write(save.Characters[i].Exp);
                bw.BaseStream.Seek(3, SeekOrigin.Current);
                bw.Write(save.Characters[i].Status);

                work = save.Characters[i].Titles.ToByteArray();
                bw.Write(work);

                bw.BaseStream.Seek(2, SeekOrigin.Current);
                bw.Write(save.Characters[i].HP);
                bw.Write(save.Characters[i].TP);
                bw.BaseStream.Seek(12, SeekOrigin.Current);
                bw.Write(save.Characters[i].MaxHP);
                bw.Write(save.Characters[i].MaxTP);
                bw.Write(save.Characters[i].Str);
                bw.Write(save.Characters[i].Atk);
                bw.Write(save.Characters[i].Atk2);
                bw.Write(save.Characters[i].Def);
                bw.Write(save.Characters[i].Lck);
                bw.Write(save.Characters[i].Acc);
                bw.Write(save.Characters[i].Eva);
                bw.Write(save.Characters[i].Int);

                bw.Write(save.Characters[i].Weapon);
                bw.Write(save.Characters[i].Armor);
                bw.Write(save.Characters[i].Helm);
                bw.Write(save.Characters[i].Arms);
                bw.Write(save.Characters[i].Accessory1);
                bw.Write(save.Characters[i].Accessory2);

                bw.Write(save.Characters[i].Overlimit);
                bw.BaseStream.Seek(1, SeekOrigin.Current);
                bw.Write(save.Characters[i].Affection);
                //seek 0xb, read 5 tech mask bytes
                bw.BaseStream.Seek(0x16, SeekOrigin.Current);
                work = save.Characters[i].Techs.ToByteArrayLow();
                bw.Write(work);

                bw.BaseStream.Seek(0x13, SeekOrigin.Current);
                for (int t = 0; t < save.Characters[i].TechUses.Length; t++)
                {
                    bw.Write(save.Characters[i].TechUses[t]);
                }

                bw.BaseStream.Seek(0x6c - save.Characters[i].TechUses.Length * 2, SeekOrigin.Current);
                for (int c = 0; c < 24; c++)
                {
                    bw.Write(save.Characters[i].Cooking[c]);
                }
                bw.BaseStream.Seek(0x1c, SeekOrigin.Current);
            }

            byte flags;
            // Dogs
            bw.BaseStream.Seek(0xf14, SeekOrigin.Begin); // 0xfb4
            for (int x = 0; x < 5; x++)
            {
                flags = 0;
                flags += save.DogLover[x * 8] == true ? (byte)1 : (byte)0;
                flags += save.DogLover[x * 8 + 1] == true ? (byte)2 : (byte)0;
                flags += save.DogLover[x * 8 + 2] == true ? (byte)4 : (byte)0;
                flags += save.DogLover[x * 8 + 3] == true ? (byte)8 : (byte)0;
                flags += save.DogLover[x * 8 + 4] == true ? (byte)0x10 : (byte)0;
                flags += save.DogLover[x * 8 + 5] == true ? (byte)0x20 : (byte)0;
                flags += save.DogLover[x * 8 + 6] == true ? (byte)0x40 : (byte)0;
                flags += save.DogLover[x * 8 + 7] == true ? (byte)0x80 : (byte)0;
                bw.Write(flags);
            }

            // Women
            bw.BaseStream.Seek(0xf64, SeekOrigin.Begin); // 0x1004
            for (int x = 0; x < 4; x++)
            {
                flags = 0;
                flags += save.Gigolo[x * 8] == true ? (byte)1 : (byte)0;
                flags += save.Gigolo[x * 8 + 1] == true ? (byte)2 : (byte)0;
                flags += save.Gigolo[x * 8 + 2] == true ? (byte)4 : (byte)0;
                flags += save.Gigolo[x * 8 + 3] == true ? (byte)8 : (byte)0;
                flags += save.Gigolo[x * 8 + 4] == true ? (byte)0x10 : (byte)0;
                flags += save.Gigolo[x * 8 + 5] == true ? (byte)0x20 : (byte)0;
                flags += save.Gigolo[x * 8 + 6] == true ? (byte)0x40 : (byte)0;
                flags += save.Gigolo[x * 8 + 7] == true ? (byte)0x80 : (byte)0;
                bw.Write(flags);
            }

            // Treasure
            bw.BaseStream.Seek(0xffe, SeekOrigin.Begin); // 0x109e
            flags = 0;
            flags += save.Treasure[0] == true ? (byte)4 : (byte)0;
            flags += save.Treasure[1] == true ? (byte)8 : (byte)0;
            flags += save.Treasure[2] == true ? (byte)0x10 : (byte)0;
            flags += save.Treasure[3] == true ? (byte)0x20 : (byte)0;
            flags += save.Treasure[4] == true ? (byte)0x40 : (byte)0;
            flags += save.Treasure[5] == true ? (byte)0x80 : (byte)0;
            bw.Write(flags);

            for (int x = 0; x < 33; x++)
            {
                flags = 0;
                flags += save.Treasure[x * 8 + 6] == true ? (byte)1 : (byte)0;
                flags += save.Treasure[x * 8 + 7] == true ? (byte)2 : (byte)0;
                flags += save.Treasure[x * 8 + 8] == true ? (byte)4 : (byte)0;
                flags += save.Treasure[x * 8 + 9] == true ? (byte)8 : (byte)0;
                flags += save.Treasure[x * 8 + 10] == true ? (byte)0x10 : (byte)0;
                flags += save.Treasure[x * 8 + 11] == true ? (byte)0x20 : (byte)0;
                flags += save.Treasure[x * 8 + 12] == true ? (byte)0x40 : (byte)0;
                flags += save.Treasure[x * 8 + 13] == true ? (byte)0x80 : (byte)0;
                bw.Write(flags);
            }

            flags = 0;
            flags += save.Treasure[270] == true ? (byte)1 : (byte)0;
            bw.Write(flags);

            // Party
            bw.BaseStream.Seek(0x107d, SeekOrigin.Begin); // 0x111d
            for (int i = 0; i < 8; i++)
                bw.Write(save.Party[i]);

            // Items
            bw.BaseStream.Seek(0x108e, SeekOrigin.Begin); // 0x112e
            for (int i = 0; i < Globals.ItemNames[saveType].Length; i++)
                bw.Write(save.Items[i]);

            // Collector's Book
            bw.BaseStream.Seek(0x1304, SeekOrigin.Begin); // 0x13a4
            for (int i = 0; i < 0x43; i++)
                bw.Write(save.CollectorsBook[i]);

            bw.BaseStream.Seek(0x1c0f, SeekOrigin.Begin);// 0x1caf; maybe 1cb0 but fat chance
            bw.Write(save.Recipes.ToByteArray());
            bw.BaseStream.Seek(7, SeekOrigin.Current);
            bw.Write(save.EncounterMode);

            // Battles
            bw.BaseStream.Seek(0x1c36, SeekOrigin.Begin); // 0x1cd6
            for (int i = 0; i < 9; i++)
                bw.Write(save.Characters[i].CurrentBattles);

            // HardMode / Challenges
            bw.Write(save.UnisonGauge);
            bw.BaseStream.Seek(2, SeekOrigin.Current);
            // 1 = ? saw in Noxbur's save
            byte flag = save.Challenges;
            if (save.GelsUsed) flag |= 8;
            if (save.HardmodeOnly) flag |= 4;
            if (save.HaveDied) flag |= 2;
            if (save.DefaultEquip) flag |= 1;
            bw.Write(flag);
            bw.BaseStream.Seek(2, SeekOrigin.Current);
            bw.Write(save.HardModeBattles);

            // Deaths
            bw.BaseStream.Seek(0x1c64, SeekOrigin.Begin); // 0x1d04
            for (int i = 0; i < 9; i++)
                bw.Write(save.Characters[i].Deaths);
            // Battle Items Used
            for (int i = 0; i < 9; i++)
                bw.Write(save.Characters[i].BattleItemsUsed);
            // Kills
            for (int i = 0; i < 9; i++)
                bw.Write(save.Characters[i].Kills);
            // Figurine Book
            bw.BaseStream.Seek(4, SeekOrigin.Current);
            for (int i = 0; i < 36; i++)
            {
                bw.Write(save.FigurineBook[i]);
            }

            // Grade
            bw.BaseStream.Seek(0x1d24, SeekOrigin.Begin); // 0x1dc4
            bw.Write((uint)(save.Grade * 100));

            // Checksum
            CalculateChecksum();
            bw.BaseStream.Seek(0, SeekOrigin.Begin);
            bw.Write(Checksum1);
            bw.Write(0);

            bw.Close();

            StreamWriter sw = new StreamWriter(filename);
            bw = new BinaryWriter(sw.BaseStream);
            bw.Write(ms.ToArray());
            bw.Close();
        }

        public virtual void CalculateChecksum(int slot = 0) 
        {
            uint checksum = 0;
            // only skip first 4 bytes which is checksum
            for (int i = 4; i < 0x2614; i += 4)
            {
                checksum += BitConverter.ToUInt32(bytes, i);
            }
            Checksum1 = checksum;
        }
    }
}
