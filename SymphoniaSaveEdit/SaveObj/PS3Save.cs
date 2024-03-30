using PS3FileSystem;
using SymphoniaSaveEdit.Utils;
using System.IO;
using System.Linq;
using System.Windows;

using static SymphoniaSaveEdit.Utils.BinaryHelper;

namespace SymphoniaSaveEdit.SaveObj
{
    internal class PS3Save : SaveFile
    {
        bool unencrypted = false;
        Ps3SaveManager manager;

        public PS3Save(string[] filenames)
        {
            saveType = SaveType.PS3;
            OpenSave(filenames);
        }

        public override void OpenSave(string[] filenames)
        {
            Decrypt(filenames);

            bigEndian = true;

            BinaryReader br = new BinaryReader(new MemoryStream(bytes));
            GameSave save = new GameSave();
            
            br.BaseStream.Seek(0x10, SeekOrigin.Begin);
            save.GameTime = new GameTime(ReverseUInt(br.ReadBytes(4)));
            save.Gald = ReverseUInt(br.ReadBytes(4));
            save.Encounters = ReverseUShort(br.ReadBytes(2));
            save.Combo = ReverseUShort(br.ReadBytes(2));
            br.BaseStream.Seek(0x10, SeekOrigin.Current);

            // Records
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

            // Battles
            save.Battles = ReverseUInt(br.ReadBytes(4));
            for (int i = 0; i < 9; i++)
                save.Characters[i].Battles = ReverseUInt(br.ReadBytes(4));

            // Current
            br.BaseStream.Seek(0x1d0, SeekOrigin.Begin);
            save.GaldCurrent = ReverseUInt(br.ReadBytes(4));
            save.EncountersCurrent = ReverseUShort(br.ReadBytes(2));
            save.ComboCurrent = ReverseUShort(br.ReadBytes(2));
            save.GameTimeCurrent = new GameTime(ReverseUInt(br.ReadBytes(4)));
            save.SavesCurrent = ReverseUInt(br.ReadBytes(4));
            for (int i = 0; i < 257; i++)
            {
                save.MonsterList[i] = br.ReadByte();
            }

            // Characters
            br.BaseStream.Seek(0x4a0, SeekOrigin.Begin);
            for (int i = 0; i < 9; i++)
            {
                save.Characters[i].Level = br.ReadByte();
                br.ReadBytes(7);
                save.Characters[i].Exp = ReverseUInt(br.ReadBytes(4));
                br.ReadBytes(3);
                save.Characters[i].Status = br.ReadByte();
                //var work = ReverseBytes(br.ReadBytes(4));
                //save.Characters[i].Titles = BitConverter.ToString(work).Replace("-", string.Empty);
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
                save.Characters[i].Def = ReverseUShort(br.ReadBytes(2));
                save.Characters[i].Lck = ReverseUShort(br.ReadBytes(2));
                save.Characters[i].Acc = ReverseUShort(br.ReadBytes(2));
                save.Characters[i].Eva = ReverseUShort(br.ReadBytes(2));
                save.Characters[i].Int = ReverseUShort(br.ReadBytes(2));

                save.Characters[i].Weapon = ReverseUShort(br.ReadBytes(2));
                save.Characters[i].Armor = ReverseUShort(br.ReadBytes(2));
                save.Characters[i].Helm = ReverseUShort(br.ReadBytes(2));
                save.Characters[i].Arms = ReverseUShort(br.ReadBytes(2));
                save.Characters[i].Accessory1 = ReverseUShort(br.ReadBytes(2));
                save.Characters[i].Accessory2 = ReverseUShort(br.ReadBytes(2));

                save.Characters[i].Overlimit = br.ReadByte();
                br.ReadBytes(3);
                save.Characters[i].Affection = ReverseUShort(br.ReadBytes(2));
                //br.BaseStream.Seek(0x2e, SeekOrigin.Current);
                //seek 0x0f, read 5 tech mask bytes
                br.BaseStream.Seek(0x17, SeekOrigin.Current);
                save.Characters[i].Techs = ReverseBytes(br.ReadBytes(5)).ToBoolArray();

                br.BaseStream.Seek(0x10, SeekOrigin.Current);
                for (int t = 0; t < save.Characters[i].TechUses.Length; t++)
                {
                    save.Characters[i].TechUses[t] = ReverseUShort(br.ReadBytes(2));
                }
                //br.BaseStream.Seek(0x9e - save.Characters[i].TechUses.Length * 2, SeekOrigin.Current);//0xCC-0x2e-techs*2
                br.BaseStream.Seek(0x6c - save.Characters[i].TechUses.Length * 2, SeekOrigin.Current);
                for (int c = 0; c < 24; c++)
                {
                    save.Characters[i].Cooking[c] = br.ReadByte();
                }

                br.BaseStream.Seek(0x1c, SeekOrigin.Current);
            }

            byte flags;
            // Dogs
            br.BaseStream.Seek(0xf0c, SeekOrigin.Begin); //0xf0c
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
            br.BaseStream.Seek(0xF5C, SeekOrigin.Begin);
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
            br.BaseStream.Seek(0xFF6, SeekOrigin.Begin);
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

            br.BaseStream.Seek(0x1075, SeekOrigin.Begin);
            for (int i = 0; i < 8; i++)
                save.Party[i] = br.ReadByte();

            // Items
            br.BaseStream.Seek(0x1086, SeekOrigin.Begin);
            for (int i = 0; i < Globals.ItemNames[saveType].Length; i++)
                save.Items[i] = br.ReadByte();

            // Collector's Book
            br.BaseStream.Seek(0x12fc, SeekOrigin.Begin);
            for (int i = 0; i < 0x43; i++)
                save.CollectorsBook[i] = br.ReadByte();

            br.BaseStream.Seek(0x1c08, SeekOrigin.Begin);
            save.Recipes = ReverseBytes(br.ReadBytes(4)).ToBoolArray();
            br.ReadBytes(7);
            save.EncounterMode = br.ReadByte();

            // Battles
            br.BaseStream.Seek(0x1c2e, SeekOrigin.Begin);
            for (int i = 0; i < 9; i++)
                save.Characters[i].CurrentBattles = ReverseUShort(br.ReadBytes(2));

            // HardMode
            save.UnisonGauge = br.ReadByte();
            br.ReadBytes(2);
            save.Challenges = br.ReadByte();
            save.GelsUsed = (save.Challenges & 8) == 8;
            save.HaveDied = (save.Challenges & 2) == 2;
            save.DefaultEquip = (save.Challenges & 1) == 1;
            br.ReadBytes(2);
            save.HardModeBattles = ReverseUShort(br.ReadBytes(2));

            // Deaths
            br.BaseStream.Seek(0x1C5C, SeekOrigin.Begin);
            for (int i = 0; i < 9; i++)
                save.Characters[i].Deaths = br.ReadByte();
            // Battle Items Used
            for (int i = 0; i < 9; i++)
                save.Characters[i].BattleItemsUsed = br.ReadByte();

            // Kills
            for (int i = 0; i < 9; i++)
                save.Characters[i].Kills = ReverseUShort(br.ReadBytes(2));

            // Grade - 0x1cdc (4), 0x1d1c (4)
            br.BaseStream.Seek(0x1cd8, SeekOrigin.Begin);
            save.Grade = ReverseUInt(br.ReadBytes(4)) / 100.0; // grade used?

            save.SaveName = "PS3 Save";
            Saves.Add(save);
        }

        public override void WriteSave(string filename, int slot = 0)
        {
            MemoryStream ms = new MemoryStream(bytes);
            BinaryWriter bw = new BinaryWriter(ms);

            GameSave save = Saves[slot];

            bw.BaseStream.Seek(0x10, SeekOrigin.Begin);
            bw.WriteReverse(save.GameTime.TotalFrames);
            bw.WriteReverse(save.Gald);
            bw.WriteReverse(save.Encounters);
            bw.WriteReverse(save.Combo);
            bw.BaseStream.Seek(0x10, SeekOrigin.Current);

            // Records
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

            // Battles
            bw.WriteReverse(save.Battles);
            for (int i = 0; i < 9; i++)
                bw.WriteReverse(save.Characters[i].Battles);

            // Current
            bw.BaseStream.Seek(0x1d0, SeekOrigin.Begin);
            bw.WriteReverse(save.GaldCurrent);
            bw.WriteReverse(save.EncountersCurrent);
            bw.WriteReverse(save.ComboCurrent);
            bw.WriteReverse(save.GameTimeCurrent.TotalFrames);
            bw.WriteReverse(save.SavesCurrent);
            for (int i = 0; i < 257; i++)
                bw.Write(save.MonsterList[i]);

            // Characters
            bw.BaseStream.Seek(0x4a0, SeekOrigin.Begin);
            byte[] work;
            for (int i = 0; i < 9; i++)
            {
                bw.Write(save.Characters[i].Level);
                bw.BaseStream.Seek(7, SeekOrigin.Current);
                bw.WriteReverse(save.Characters[i].Exp);
                bw.BaseStream.Seek(3, SeekOrigin.Current);
                bw.Write(save.Characters[i].Status);
                //uint32 = uint.Parse(save.Characters[i].Titles);
                //work = Enumerable.Range(0, save.Characters[i].Titles.Length)
                //                .Where(x => x % 2 == 0)
                //                .Select(x => Convert.ToByte(save.Characters[i].Titles.Substring(x, 2), 16)).ToArray();
                work = save.Characters[i].Titles.To4ByteArray(false);//true for ps3?
                                                                     //if (littleEndian)
                                                                     //Array.Reverse(work);
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
                //seek 0x0f, read 5 tech mask bytes
                bw.BaseStream.Seek(0x17, SeekOrigin.Current);
                work = save.Characters[i].Techs.To4ByteArray(false);//true for ps3?
                bw.Write(work);

                bw.BaseStream.Seek(0x10, SeekOrigin.Current);
                for (int t = 0; t < save.Characters[i].TechUses.Length; t++)
                {
                    bw.WriteReverse(save.Characters[i].TechUses[t]);
                }
                //br.BaseStream.Seek(0x9e - save.Characters[i].TechUses.Length * 2, SeekOrigin.Current);//0xCC-0x2e-techs*2
                bw.BaseStream.Seek(0x6c - save.Characters[i].TechUses.Length * 2, SeekOrigin.Current);
                for (int c = 0; c < 24; c++)
                {
                    bw.Write(save.Characters[i].Cooking[c]);
                }

                bw.BaseStream.Seek(0x1c, SeekOrigin.Current);
            }

            byte flags;
            // Dogs
            bw.BaseStream.Seek(0xF0C, SeekOrigin.Begin);
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
            bw.BaseStream.Seek(0xF5C, SeekOrigin.Begin);
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

            // Treasure
            bw.BaseStream.Seek(0xFF6, SeekOrigin.Begin);
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

            bw.BaseStream.Seek(0x1075, SeekOrigin.Begin);
            for (int i = 0; i < 8; i++)
                bw.Write(save.Party[i]);

            // Items
            bw.BaseStream.Seek(0x1086, SeekOrigin.Begin);
            for (int i = 0; i < Globals.ItemNames[saveType].Length; i++)
                bw.Write(save.Items[i]);

            // Collector's Book
            bw.BaseStream.Seek(0x12fc, SeekOrigin.Begin);
            for (int i = 0; i < 0x43; i++)
                bw.Write(save.CollectorsBook[i]);

            bw.BaseStream.Seek(0x1c08, SeekOrigin.Begin);
            bw.Write(save.Recipes.To4ByteArray(false));
            bw.BaseStream.Seek(7, SeekOrigin.Current);
            bw.Write(save.EncounterMode);

            // Battles
            bw.BaseStream.Seek(0x1c2e, SeekOrigin.Begin);
            for (int i = 0; i < 9; i++)
                bw.WriteReverse(save.Characters[i].CurrentBattles);

            // HardMode
            bw.Write(save.UnisonGauge);
            bw.BaseStream.Seek(2, SeekOrigin.Current);
            byte flag = save.Challenges;
            if (save.GelsUsed) flag |= 8;
            if (save.HaveDied) flag |= 2;
            if (save.DefaultEquip) flag |= 1;
            bw.Write(flag);
            bw.BaseStream.Seek(2, SeekOrigin.Current);
            bw.WriteReverse(save.HardModeBattles);

            // Deaths
            bw.BaseStream.Seek(0x1C5C, SeekOrigin.Begin);
            for (int i = 0; i < 9; i++)
                bw.Write(save.Characters[i].Deaths);
            // Battle Items Used
            for (int i = 0; i < 9; i++)
                bw.Write(save.Characters[i].BattleItemsUsed);

            // Kills
            for (int i = 0; i < 9; i++)
                bw.WriteReverse(save.Characters[i].Kills);

            // Grade
            bw.BaseStream.Seek(0x1cd8, SeekOrigin.Begin);
            bw.WriteReverse((uint)(save.Grade * 100.0));

            bw.Close();

            StreamWriter sw = new StreamWriter(filename);
            bw = new BinaryWriter(sw.BaseStream);
            bw.Write(ms.ToArray());
            bw.Close();

            if (!unencrypted)
            {
                //get file entry using name
                Ps3File file = manager.Files.FirstOrDefault(t => t.PFDEntry.file_name == "SAVEDATA.BIN");
                //define byte array that the decrypted data should be allocated
                byte[] filedata = null;
                //check if file is not null
                if (file != null)
                    filedata = file.EncryptToBytes();
                if (filedata != null)
                {
                    sw = new StreamWriter(filename);
                    bw = new BinaryWriter(sw.BaseStream);
                    bw.Write(filedata);
                    bw.Close();
                    MessageBox.Show("Saved successfully.", "File Saved");
                }
                else
                    MessageBox.Show("Error encrypting data to PS3 format.");
            }
        }
        
        private void Decrypt(string[] filenames)
        {
            if (filenames.Length > 1)
                unencrypted = false;
            else if (filenames[0].ToLower().EndsWith(".bin"))
                unencrypted = true;

            if (unencrypted)
                bytes = File.ReadAllBytes(filenames[0]);
            else
            {
                //filename = filenames.Where(f => f.EndsWith("SAVEDATA.BIN")).FirstOrDefault();
                //define the securefile id
                byte[] key = new byte[] { 0xD5, 0x3D, 0xCD, 0x6C, 0xAC, 0x63, 0x8A, 0xA4, 0x56, 0xD6, 0x8B, 0x9F, 0x47, 0x21, 0x95, 0x37 };
                //declare ps3 manager using the directory path, and the secure file id
                manager = new Ps3SaveManager(Path.GetDirectoryName(filenames[0]), key);
                //get file entry using name
                Ps3File file = manager.Files.FirstOrDefault(t => t.PFDEntry.file_name == "SAVEDATA.BIN");
                //define byte array that the decrypted data should be allocated
                bytes = null;
                //check if file is not null
                if (file != null)
                    bytes = file.DecryptToBytes();
                if (bytes != null)
                {
                    Saves.Clear();

                    //do stuff with the decrypted data
                    StreamWriter sw = new StreamWriter("Converted.bin");
                    BinaryWriter bw = new BinaryWriter(sw.BaseStream);
                    bw.Write(bytes);
                    bw.Close();
                }
            }
        }
    }
}
