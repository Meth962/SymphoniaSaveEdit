using SymphoniaSaveEdit.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static SymphoniaSaveEdit.Utils.BinaryHelper;

namespace SymphoniaSaveEdit.SaveObj
{
    internal class GCSave : SaveFile
    {
        const string gameID = "GQS";

        int offset = 0;

        public GCSave(string filename) : base(SaveType.GC)
        {
            OpenSave(filename);
        }

        public override void OpenSave(string filename)
        {
            Saves.Clear();
            bytes = File.ReadAllBytes(filename);

            StreamReader sr = new StreamReader(new MemoryStream(bytes));
            BinaryReader br = new BinaryReader(sr.BaseStream);

            string extension = Path.GetExtension(filename).ToUpper();
            SetOffsetByExtension(extension);

            CalculateChecksum();

            br.BaseStream.Seek(offset, SeekOrigin.Begin);

            bool regionPrompted = false;
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                GameSave save = new GameSave(saveType);

                string id = Encoding.Default.GetString(br.ReadBytes(3));
                byte region = br.ReadByte();

                if (id != gameID)
                {
                    br.BaseStream.Seek(0x603f - 4, SeekOrigin.Current);
                    continue;
                }

                if (region != 'E' && !regionPrompted)
                {
                    var result = MessageBox.Show("This is untested for Europe and Japanese regions. Try it anyway?", "Region Mismatch", MessageBoxButton.YesNo);
                    if (result != MessageBoxResult.Yes)
                        return;
                    regionPrompted = true;
                }

                string companyID = Encoding.Default.GetString(br.ReadBytes(2));
                byte graphicFormat = br.ReadByte();
                if (bigEndian)
                    graphicFormat = br.ReadByte();
                else
                    br.ReadByte();
                string fileID = Encoding.Default.GetString(br.ReadBytes(32));
                save.LastModified = new DateTime(1999, 12, 31, 23, 59, 59).AddSeconds(ReverseUInt(br.ReadBytes(4)));

                br.BaseStream.Seek(52, SeekOrigin.Current);
                bigEndian = true; // Gamesave is big Endian, so we need to switch it here
                save.SaveName = Encoding.Default.GetString(br.ReadBytes(32));

                br.BaseStream.Seek(0x2000, SeekOrigin.Current);
                save.Checksum1 = ReverseUInt(br.ReadBytes(4));
                save.Checksum2 = ReverseUInt(br.ReadBytes(4));
                br.ReadBytes(4); // System value
                br.ReadBytes(4); // System value

                save.GameTime = new GameTime(ReverseUInt(br.ReadBytes(4)));
                save.Gald = ReverseUInt(br.ReadBytes(4));
                save.Encounters = ReverseUShort(br.ReadBytes(2));
                save.Combo = ReverseUShort(br.ReadBytes(2));
                br.BaseStream.Seek(0x10, SeekOrigin.Current);

                //save.MaxGameTime = TimeSpan.FromMilliseconds(ReverseInt(br.ReadBytes(4)) / 60.0 * 1000);
                save.MaxGameTime = new GameTime(ReverseUInt(br.ReadBytes(4)));
                save.MaxGald = ReverseUInt(br.ReadBytes(4));
                save.TotalGaldUsed = ReverseUInt(br.ReadBytes(4));
                save.Saves = ReverseUInt(br.ReadBytes(4));
                save.GameCleared = ReverseUInt(br.ReadBytes(4));
                save.MaxEncounters = ReverseUInt(br.ReadBytes(4));
                save.Escapes = ReverseUInt(br.ReadBytes(4));
                save.MaxCombo = ReverseUShort(br.ReadBytes(2));
                br.ReadBytes(2);
                save.MaxDmg = ReverseUInt(br.ReadBytes(4));
                save.MaxGrade = ReverseUInt(br.ReadBytes(4)) / 100.0;

                save.Battles = ReverseUInt(br.ReadBytes(4));
                for (int i = 0; i < 9; i++)
                {
                    save.Characters[i].Battles = ReverseUInt(br.ReadBytes(4));
                }

                br.BaseStream.Seek(0x144, SeekOrigin.Current);
                save.GaldCurrent = ReverseUInt(br.ReadBytes(4));
                save.EncountersCurrent = ReverseUShort(br.ReadBytes(2));
                save.ComboCurrent = ReverseUShort(br.ReadBytes(2));
                save.GameTimeCurrent = new GameTime(ReverseUInt(br.ReadBytes(4)));
                save.SavesCurrent = ReverseUInt(br.ReadBytes(4));
                for (int i = 0; i < 251; i++)
                {
                    save.MonsterList[i] = br.ReadByte();
                }

                br.BaseStream.Seek(0x1bd, SeekOrigin.Current);
                for (int i = 0; i < 9; i++)
                {
                    save.Characters[i].Level = br.ReadByte();
                    br.ReadBytes(7);
                    save.Characters[i].Exp = ReverseUInt(br.ReadBytes(4));
                    br.ReadBytes(3);
                    save.Characters[i].Status = br.ReadByte();
                    //var work = ReverseBytes(br.ReadBytes(4));
                    //save.Characters[i].Titles = BitConverter.ToString(work).Replace("-",string.Empty);
                    save.Characters[i].Titles = ReverseBytes(br.ReadBytes(4)).ToBoolArray();
                    br.ReadBytes(2);
                    save.Characters[i].HP = ReverseUShort(br.ReadBytes(2));
                    save.Characters[i].TP = ReverseUShort(br.ReadBytes(2));
                    br.ReadBytes(12);
                    save.Characters[i].MaxHP = ReverseUShort(br.ReadBytes(2));
                    save.Characters[i].MaxTP = ReverseUShort(br.ReadBytes(2));
                    save.Characters[i].Str = ReverseUShort(br.ReadBytes(2));
                    save.Characters[i].Atk = ReverseUShort(br.ReadBytes(2));
                    save.Characters[i].Atk2 = ReverseUShort(br.ReadBytes(2));
                    save.Characters[i].Int = ReverseUShort(br.ReadBytes(2));
                    save.Characters[i].Def = ReverseUShort(br.ReadBytes(2));
                    save.Characters[i].Acc = ReverseUShort(br.ReadBytes(2));
                    save.Characters[i].Eva = ReverseUShort(br.ReadBytes(2));
                    save.Characters[i].Lck = ReverseUShort(br.ReadBytes(2));

                    save.Characters[i].Weapon = ReverseUShort(br.ReadBytes(2));
                    save.Characters[i].Armor = ReverseUShort(br.ReadBytes(2));
                    save.Characters[i].Helm = ReverseUShort(br.ReadBytes(2));
                    save.Characters[i].Arms = ReverseUShort(br.ReadBytes(2));
                    save.Characters[i].Accessory1 = ReverseUShort(br.ReadBytes(2));
                    save.Characters[i].Accessory2 = ReverseUShort(br.ReadBytes(2));

                    save.Characters[i].Overlimit = br.ReadByte();
                    br.ReadBytes(3);
                    save.Characters[i].Affection = ReverseUShort(br.ReadBytes(2));
                    //seek 0xf, read 5 tech mask bytes
                    br.BaseStream.Seek(0x17, SeekOrigin.Current);
                    save.Characters[i].Techs = ReverseBytes(br.ReadBytes(5)).ToBoolArrayLow();

                    br.BaseStream.Seek(0x10, SeekOrigin.Current);
                    for (int t = 0; t < save.Characters[i].TechUses.Length; t++)
                    {
                        save.Characters[i].TechUses[t] = ReverseUShort(br.ReadBytes(2));
                    }

                    br.BaseStream.Seek(0x6c - save.Characters[i].TechUses.Length * 2, SeekOrigin.Current);

                    for (int c = 0; c < 24; c++)
                    {
                        save.Characters[i].Cooking[c] = br.ReadByte();
                    }

                    br.BaseStream.Seek(0x1c, SeekOrigin.Current);
                }

                byte flags;
                br.BaseStream.Seek(0x7B, SeekOrigin.Current);//0x2F5A
                for (int x = 0; x < 5; x++)
                {
                    flags = br.ReadByte();
                    save.DogLover[x * 8] = (flags & 1) == 1;
                    save.DogLover[x * 8 + 1] = (flags & 2) == 2;
                    save.DogLover[x * 8 + 2] = (flags & 4) == 4;
                    save.DogLover[x * 8 + 3] = (flags & 8) == 8;
                    save.DogLover[x * 8 + 4] = (flags & 0x10) == 0x10;
                    save.DogLover[x * 8 + 5] = (flags & 0x20) == 0x20;
                    save.DogLover[x * 8 + 6] = (flags & 0x40) == 0x40;
                    save.DogLover[x * 8 + 7] = (flags & 0x80) == 0x80;
                }

                br.BaseStream.Seek(0x65, SeekOrigin.Current);
                for (int x = 0; x < 5; x++)
                {
                    flags = br.ReadByte();
                    save.Gigolo[x * 8] = (flags & 1) == 1;
                    save.Gigolo[x * 8 + 1] = (flags & 2) == 2;
                    save.Gigolo[x * 8 + 2] = (flags & 4) == 4;
                    save.Gigolo[x * 8 + 3] = (flags & 8) == 8;
                    save.Gigolo[x * 8 + 4] = (flags & 0x10) == 0x10;
                    save.Gigolo[x * 8 + 5] = (flags & 0x20) == 0x20;
                    save.Gigolo[x * 8 + 6] = (flags & 0x40) == 0x40;
                    save.Gigolo[x * 8 + 7] = (flags & 0x80) == 0x80;
                }

                br.BaseStream.Seek(0x95, SeekOrigin.Current);
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
                    save.Treasure[x * 8 + 7] = (flags & 2) == 2;
                    save.Treasure[x * 8 + 8] = (flags & 4) == 4;
                    save.Treasure[x * 8 + 9] = (flags & 8) == 8;
                    save.Treasure[x * 8 + 10] = (flags & 0x10) == 0x10;
                    save.Treasure[x * 8 + 11] = (flags & 0x20) == 0x20;
                    save.Treasure[x * 8 + 12] = (flags & 0x40) == 0x40;
                    save.Treasure[x * 8 + 13] = (flags & 0x80) == 0x80;
                }

                flags = br.ReadByte();
                save.Treasure[270] = (flags & 1) == 1;

                br.BaseStream.Seek(0x5b, SeekOrigin.Current);
                for (int i = 0; i < 8; i++)
                    save.Party[i] = br.ReadByte();

                br.BaseStream.Seek(0x9, SeekOrigin.Current);
                for (int i = 0; i < Globals.ItemNames[saveType].Length; i++)
                {
                    save.Items[i] = br.ReadByte();
                }

                br.BaseStream.Seek(0x6d, SeekOrigin.Current);
                for (int i = 0; i < 0x43; i++)
                {
                    save.CollectorsBook[i] = br.ReadByte();
                }

                br.BaseStream.Seek(0xcb1, SeekOrigin.Current);
                save.Recipes = ReverseBytes(br.ReadBytes(4)).ToBoolArray();
                br.ReadBytes(7);
                save.EncounterMode = br.ReadByte();

                br.BaseStream.Seek(0x1a, SeekOrigin.Current);//418E
                for (int i = 0; i < 9; i++)
                    save.Characters[i].CurrentBattles = ReverseUShort(br.ReadBytes(2));
                save.UnisonGauge = br.ReadByte();
                br.ReadBytes(2);
                save.Challenges = br.ReadByte();
                save.GelsUsed = (save.Challenges & 8) == 8;
                save.HaveDied = (save.Challenges & 2) == 2;
                save.DefaultEquip = (save.Challenges & 1) == 1;
                br.ReadBytes(2);
                save.HardModeBattles = ReverseUShort(br.ReadBytes(2));

                // Deaths x40ac
                br.BaseStream.Seek(0x14, SeekOrigin.Current);
                for (int i = 0; i < 9; i++)
                    save.Characters[i].Deaths = br.ReadByte();
                // Battle items used x40b5
                for (int i = 0; i < 9; i++)
                    save.Characters[i].BattleItemsUsed = br.ReadByte();

                //br.BaseStream.Seek(0x2d, SeekOrigin.Current);
                for (int i = 0; i < 9; i++)
                {
                    save.Characters[i].Kills = ReverseUShort(br.ReadBytes(2));
                }

                br.BaseStream.Seek(0x98, SeekOrigin.Current);//4238
                save.Grade = ReverseUInt(br.ReadBytes(4)) / 100.0;

                // Checksum warning
                //if (Checksum1 != save.Checksum1 && Checksum2 != save.Checksum2)
                //    lblChecksum.Content = "Invalid checksums: 2!";
                //else if (check1 != save.Checksum1)
                //    lblChecksum.Content = "Invalid checksum 1!";
                //else if (check2 != save.Checksum2)
                //    lblChecksum.Content = "Invalid checksum 2!";

                Saves.Add(save);
            }

            br.Close();
        }

        public override void WriteSave(string filename, int slot = 0)
        {
            MemoryStream ms = new MemoryStream(bytes);
            BinaryWriter bw = new BinaryWriter(ms);

            string extension = System.IO.Path.GetExtension(filename).ToUpper();
            SetOffsetByExtension(extension);

            bw.BaseStream.Seek(offset, SeekOrigin.Begin);

            GameSave save = Saves[slot];

            bw.BaseStream.Seek(0x2080, SeekOrigin.Current);
            bw.Write((uint)0);
            bw.Write((uint)0);
            bw.BaseStream.Seek(8, SeekOrigin.Current);

            bw.WriteReverse(save.GameTime.TotalFrames);
            bw.WriteReverse(save.Gald);
            bw.WriteReverse(save.Encounters);
            bw.WriteReverse(save.Combo);
            bw.BaseStream.Seek(0x10, SeekOrigin.Current);

            bw.WriteReverse(save.MaxGameTime.TotalFrames);
            bw.WriteReverse(save.MaxGald);
            bw.WriteReverse(save.TotalGaldUsed);
            bw.WriteReverse(save.Saves);
            bw.WriteReverse(save.GameCleared);
            bw.WriteReverse(save.MaxEncounters);
            bw.WriteReverse(save.Escapes);
            bw.WriteReverse(save.MaxCombo);
            bw.BaseStream.Seek(2, SeekOrigin.Current);
            bw.WriteReverse(save.MaxDmg);
            bw.WriteReverse((uint)(save.MaxGrade * 100));

            bw.WriteReverse(save.Battles);
            for (int i = 0; i < 9; i++)
                bw.WriteReverse(save.Characters[i].Battles);

            bw.BaseStream.Seek(0x144, SeekOrigin.Current);
            bw.WriteReverse(save.GaldCurrent);
            bw.WriteReverse(save.EncountersCurrent);
            bw.WriteReverse(save.ComboCurrent);
            bw.WriteReverse(save.GameTimeCurrent.TotalFrames);
            bw.WriteReverse(save.SavesCurrent);
            for (int i = 0; i < 251; i++)
                bw.Write(save.MonsterList[i]);

            byte[] work;
            bw.BaseStream.Seek(0x1bd, SeekOrigin.Current);
            for (int i = 0; i < 9; i++)
            {
                bw.Write(save.Characters[i].Level);
                bw.BaseStream.Seek(7, SeekOrigin.Current);
                bw.WriteReverse(save.Characters[i].Exp);
                bw.BaseStream.Seek(3, SeekOrigin.Current);
                bw.Write(save.Characters[i].Status);

                work = save.Characters[i].Titles.To4ByteArray(false);
                bw.Write(work);

                bw.BaseStream.Seek(2, SeekOrigin.Current);
                bw.WriteReverse(save.Characters[i].HP);
                bw.WriteReverse(save.Characters[i].TP);
                bw.BaseStream.Seek(12, SeekOrigin.Current);
                bw.WriteReverse(save.Characters[i].MaxHP);
                bw.WriteReverse(save.Characters[i].MaxTP);
                bw.WriteReverse(save.Characters[i].Str);
                bw.WriteReverse(save.Characters[i].Atk);
                bw.WriteReverse(save.Characters[i].Atk2);
                bw.WriteReverse(save.Characters[i].Int);
                bw.WriteReverse(save.Characters[i].Def);
                bw.WriteReverse(save.Characters[i].Acc);
                bw.WriteReverse(save.Characters[i].Eva);
                bw.WriteReverse(save.Characters[i].Lck);

                bw.WriteReverse(save.Characters[i].Weapon);
                bw.WriteReverse(save.Characters[i].Armor);
                bw.WriteReverse(save.Characters[i].Helm);
                bw.WriteReverse(save.Characters[i].Arms);
                bw.WriteReverse(save.Characters[i].Accessory1);
                bw.WriteReverse(save.Characters[i].Accessory2);

                bw.Write(save.Characters[i].Overlimit);
                bw.BaseStream.Seek(3, SeekOrigin.Current);
                bw.WriteReverse(save.Characters[i].Affection);
                //seek 0xf, read 5 tech mask bytes
                bw.BaseStream.Seek(0x17, SeekOrigin.Current);
                work = save.Characters[i].Techs.To4ByteArray(false);
                bw.Write(work);

                bw.BaseStream.Seek(0x10, SeekOrigin.Current);
                for (int t = 0; t < save.Characters[i].TechUses.Length; t++)
                {
                    bw.WriteReverse(save.Characters[i].TechUses[t]);
                }

                bw.BaseStream.Seek(0x6c - save.Characters[i].TechUses.Length * 2, SeekOrigin.Current);

                for (int c = 0; c < 24; c++)
                {
                    bw.Write(save.Characters[i].Cooking[c]);
                }

                bw.BaseStream.Seek(0x1c, SeekOrigin.Current);
            }

            byte flags;
            bw.BaseStream.Seek(0x7B, SeekOrigin.Current);//0x2F5A
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

            bw.BaseStream.Seek(0x65, SeekOrigin.Current);
            for (int x = 0; x < 5; x++)
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

            bw.BaseStream.Seek(0x95, SeekOrigin.Current);
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

            bw.BaseStream.Seek(0x5b, SeekOrigin.Current);
            for (int i = 0; i < 8; i++)
                bw.Write(save.Party[i]);

            bw.BaseStream.Seek(0x9, SeekOrigin.Current);
            for (int i = 0; i < Globals.ItemNames[saveType].Length; i++)
                bw.Write(save.Items[i]);

            bw.BaseStream.Seek(0x6d, SeekOrigin.Current);
            for (int i = 0; i < 0x43; i++)
                bw.Write(save.CollectorsBook[i]);

            bw.BaseStream.Seek(0xcb1, SeekOrigin.Current);
            bw.Write(save.Recipes.To4ByteArray(false));
            bw.BaseStream.Seek(7, SeekOrigin.Current);
            bw.Write(save.EncounterMode);

            bw.BaseStream.Seek(0x1a, SeekOrigin.Current);
            for (int i = 0; i < 9; i++)
                bw.WriteReverse(save.Characters[i].CurrentBattles);

            bw.Write(save.UnisonGauge);
            bw.BaseStream.Seek(2, SeekOrigin.Current);
            byte flag = save.Challenges;
            if (save.GelsUsed) flag |= 8;
            if (save.HaveDied) flag |= 2;
            if (save.DefaultEquip) flag |= 1;
            bw.Write(flag);
            bw.BaseStream.Seek(2, SeekOrigin.Current);
            bw.WriteReverse(save.HardModeBattles);

            bw.BaseStream.Seek(0x14, SeekOrigin.Current);
            for (int i = 0; i < 9; i++)
                bw.Write(save.Characters[i].Deaths);
            for (int i = 0; i < 9; i++)
                bw.Write(save.Characters[i].BattleItemsUsed);

            for (int i = 0; i < 9; i++)
                bw.WriteReverse(save.Characters[i].Kills);

            bw.BaseStream.Seek(0x98, SeekOrigin.Current);
            bw.WriteReverse((uint)(save.Grade * 100.0));

            CalculateChecksum();
            bw.BaseStream.Seek(0x2080 + offset, SeekOrigin.Begin);
            bw.WriteReverse(Checksum1);
            bw.WriteReverse(Checksum2);
            bw.Close();

            StreamWriter sw = new StreamWriter(filename);
            bw = new BinaryWriter(sw.BaseStream);
            bw.Write(ms.ToArray());
            bw.Close();
        }

        public override void CalculateChecksum(int slot = 0)
        {
            Checksum1 = 0;
            Checksum2 = 0;

            int start = offset + 0x2080;
            uint work = 0;
            byte[] array = new byte[4];

            // Calculate Checksum 1, most of save
            for (int i = 0; i < 0xa75; i++)
            {
                Array.Copy(bytes, start + i * 4, array, 0, 4);
                Array.Reverse(array);
                work = BitConverter.ToUInt32(array, 0);
                if (i < 2)
                    work = 0;
                Checksum1 += work;
            }

            // Calculate Checksum 2, first 400 bytes
            for (int i = 0; i < 100; i++)
            {
                Array.Copy(bytes, start + i * 4, array, 0, 4);
                Array.Reverse(array);
                work = BitConverter.ToUInt32(array, 0);
                if (i < 2)
                    work = 0;
                Checksum2 += work;
            }
        }

        private void SetOffsetByExtension(string ext)
        {
            switch (ext)
            {
                case ".GCI":
                    offset = 0;
                    bigEndian = true;
                    break;
                case ".GCS":
                    offset = 0x110;
                    bigEndian = true;
                    break;
                case ".SAV":
                    offset = 0x80;
                    bigEndian = false;
                    break;
                default:
                    offset = 0;
                    break;
            }
        }
    }
}
