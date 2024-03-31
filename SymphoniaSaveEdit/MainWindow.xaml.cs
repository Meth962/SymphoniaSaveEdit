using Microsoft.Win32;
using Newtonsoft.Json;
using SymphoniaSaveEdit.SaveObj;
using SymphoniaSaveEdit.Utils;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Schema;
using Path = System.IO.Path;

namespace SymphoniaSaveEdit
{
    public partial class MainWindow : Window
    {
        private short treasureCount = 0;
        private short itemCount = 0;
        private short dogCount = 0;
        private short womenCount = 0;
        private int thankyou = 0;

        SaveFile save;
        string filename = string.Empty;
        SaveType saveType = SaveType.PCRaw;

        // Ugly WPF fault coding
        int previousTab = -1;
        bool codeControl = false;
        int currentItem = -1;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ClearFields()
        {
            cbxSaves.Items.Clear();
            lblMaxGald.Content = string.Empty;
            lblMaxGrade.Content = string.Empty;
            lblMaxItems.Content = string.Empty;
            lblMaxStats.Content = string.Empty;
            lblMaxTechs.Content = string.Empty;
            lblInfo.Text = string.Empty;
            lbxTreasures.Items.Clear();
            lblLastModified.Content = string.Empty;
            lblTotal.Content = string.Empty;
            pbTotal.Visibility = System.Windows.Visibility.Hidden;
            tbxStats.Clear();
            lbxTitles.Items.Clear();
            lbxTechs.Items.Clear();
        }

        private void ShowItems()
        {
            if (cbxSaves.SelectedIndex < 0)
                return;

            if (save.Saves.Count < cbxSaves.SelectedIndex + 1)
                return;

            GameSave s = save.Saves[cbxSaves.SelectedIndex];

            int items = s.Items.Count(i => i > 0);
            lblTotal.Content = string.Format("Items: {0}/{1} ({2:n1}%)", items, itemCount, Math.Truncate(items * 100.0 / itemCount * 10) / 10);
            pbTotal.Visibility = System.Windows.Visibility.Visible;
            pbTotal.Maximum = itemCount;
            pbTotal.Value = items;
        }

        private void ShowDogs()
        {
            if (cbxSaves.SelectedIndex < 0)
                return;

            if (save.Saves.Count < cbxSaves.SelectedIndex + 1)
                return;

            GameSave s = save.Saves[cbxSaves.SelectedIndex];

            int dogs = s.DogLover.Count(d => d == true);
            lblTotal.Content = string.Format("Dogs: {0}/{1} ({2:n1}%)", dogs, dogCount, Math.Truncate(dogs * 100.0 / dogCount * 10) / 10);
            pbTotal.Visibility = System.Windows.Visibility.Visible;
            pbTotal.Maximum = dogCount;
            pbTotal.Value = dogs;
        }

        private void ShowWomen()
        {
            if (cbxSaves.SelectedIndex < 0)
                return;

            if (save.Saves.Count < cbxSaves.SelectedIndex + 1)
                return;

            GameSave s = save.Saves[cbxSaves.SelectedIndex];

            int women = s.Gigolo.Count(g => g == true);
            lblTotal.Content = string.Format("Women: {0}/{1} ({2:n1}%)", women, womenCount, Math.Truncate(women * 100.0 / womenCount * 10) / 10);
            pbTotal.Visibility = System.Windows.Visibility.Visible;
            pbTotal.Maximum = womenCount;
            pbTotal.Value = women;
        }

        private void ShowTreasure()
        {
            if (cbxSaves.SelectedIndex < 0)
                return;

            if (save.Saves.Count < cbxSaves.SelectedIndex + 1)
                return;

            GameSave s = save.Saves[cbxSaves.SelectedIndex];

            int treasures = s.Treasure.Count(t => t == true);
            lblTotal.Content = string.Format("Treasures: {0}/{1} ({2:n1}%)", treasures, treasureCount, Math.Truncate(treasures * 100.0 / treasureCount * 10) / 10);
            pbTotal.Visibility = System.Windows.Visibility.Visible;
            pbTotal.Maximum = treasureCount;
            pbTotal.Value = treasures;
        }

        private void GetData()
        {
            if (cbxSaves.SelectedIndex < 0)
                return;

            //if (saves.Count < cbxSaves.SelectedIndex + 1)
            if (save.Saves.Count < cbxSaves.SelectedIndex + 1)
                return;

            GameSave s = save.Saves[cbxSaves.SelectedIndex];
            lblLastModified.Content = s.LastModified.ToString();
            //lblChecksum.Content = save.Checksum1.ToString("X2");

            UpdateCounts(s);

            UpdateStats(s);

            UpdateItems(s);

            UpdateSaveString(s);
        }

        private void UpdateCharStats(GameSave sv, int charIndex)
        {
            StringBuilder sb = new StringBuilder();
            Character c = sv.Characters[charIndex];
            sb.AppendFormat("Lvl: {0}\r\n", c.Level);
            sb.AppendFormat("Exp: {0:n0}\r\n", c.Exp);
            sb.AppendFormat("Status: {0:X2}\r\n", c.Status);
            sb.AppendLine();
            sb.AppendFormat("HP: {0}/{1}\r\n", c.HP, c.MaxHP);
            sb.AppendFormat("TP: {0}/{1}\r\n", c.TP, c.MaxTP);
            sb.AppendFormat("Str: {0:000}        Def: {1:000}\r\n", c.Str, c.Def);
            sb.AppendFormat("{0}: {1:000}  Acc: {2:000}\r\n", charIndex == 0 ? "Thrust" : "Atk", c.Atk, c.Acc);
            sb.AppendFormat("{0}: {1:000}    Eva: {2:000}\r\n", charIndex == 0 ? "Slash" : "Att", c.Atk2, c.Eva);
            sb.AppendFormat("Int: {0:000}         Lck: {1:000}\r\n", c.Int, c.Lck);
            sb.AppendLine();
            sb.AppendFormat("Weapon: {0}\r\n", c.Weapon == 0 ? string.Empty : Globals.ItemNames[saveType][c.Weapon - 1]);
            sb.AppendFormat("Armor: {0}\r\n", c.Armor == 0 ? string.Empty : Globals.ItemNames[saveType][c.Armor - 1]);
            sb.AppendFormat("Helm: {0}\r\n", c.Helm == 0 ? string.Empty : Globals.ItemNames[saveType][c.Helm - 1]);
            sb.AppendFormat("Arms: {0}\r\n", c.Arms == 0 ? string.Empty : Globals.ItemNames[saveType][c.Arms - 1]);
            sb.AppendFormat("Accessory: {0}\r\n", c.Accessory1 == 0 ? string.Empty : Globals.ItemNames[saveType][c.Accessory1 - 1]);
            sb.AppendFormat("Accessory: {0}\r\n", c.Accessory2 == 0 ? string.Empty : Globals.ItemNames[saveType][c.Accessory2 - 1]);
            sb.AppendLine();
            sb.AppendLine($"Kills: {c.Kills:n0}");
            sb.AppendLine($"Deaths: {c.Deaths:n0}");
            sb.AppendLine($"Battle Items Used: {c.BattleItemsUsed:n0}");
            sb.AppendFormat("Overlimit: {0}%\r\n", c.Overlimit);
            sb.AppendFormat("Affection: {0:n0}\r\n", c.Affection);
            sb.AppendLine();
            sb.AppendLine("Tech Uses");
            for (int t = 0; t < c.TechUses.Length; t++)
                sb.AppendLine($"{Globals.Techs[charIndex, t]}: {c.TechUses[t]:n0}");

            // Titles
            lbxTitles.Items.Clear();
            for (int b = 0; b < c.Titles.Length; b++)
            {
                var chk = new CheckBox() { Tag = b, Content = Globals.Titles[charIndex, b], IsChecked = c.Titles[b], Foreground = Globals.WhiteBrush, Style = Resources["CheckBoxStyle"] as Style };
                if (chk.Content.ToString().StartsWith("Bugged"))
                {
                    chk.IsEnabled = false;
                    chk.Foreground = new SolidColorBrush(Color.FromArgb(255, 96, 96, 96));
                }
                chk.Checked += chkTitle_Checked;
                chk.Unchecked += chkTitle_Unchecked;
                lbxTitles.Items.Add(chk);
            }
            sb.AppendLine();

            // Techs
            lbxTechs.Items.Clear();
            for (int b = 0; b < c.Techs.Length; b++)
            {
                var chk = new CheckBox() { Tag = b, Content = Globals.Techs[charIndex, b], IsChecked = c.Techs[b], Foreground = Globals.WhiteBrush, Style = Resources["CheckBoxStyle"] as Style };
                chk.Checked += new RoutedEventHandler(chkTech_Checked);
                chk.Unchecked += new RoutedEventHandler(chkTech_Unchecked);
                lbxTechs.Items.Add(chk);
            }

            tbxStats.Text = sb.ToString();
        }

        private void UpdateItems(GameSave save)
        {
            lbxItems.Items.Clear();
            for (int i = 0; i < save.Items.Length; i++)
            {
                if (i + 1 > Globals.ItemNames[saveType].Length)
                {
                    lbxItems.Items.Add($"Unknown : {save.Items[i]}");
                    Console.WriteLine($"Error itemId {i} does not exist in {saveType}");
                }
                else
                    lbxItems.Items.Add(string.Format("{0} : {1}", Globals.ItemNames[saveType][i], save.Items[i]));
            }
        }

        private void UpdateStats(GameSave save)
        {
            // Stats
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Checksum1: {0:X2}\r\n", save.Checksum1);
            sb.AppendFormat("Checksum2: {0:X2}\r\n", save.Checksum2);
            sb.AppendLine();
            sb.AppendLine("Party:");
            sb.AppendLine("---------");
            for (int i = 0; i < 8; i++)
            {
                if (save.Party[i] != 0)
                {
                    if (save.Party[i] < Globals.CharacterNames.Length+1)
                        sb.AppendLine(Globals.CharacterNames[save.Party[i]-1]);
                    else
                        sb.AppendLine("Unknown");
                }
            }
            sb.AppendLine();
            sb.AppendLine("Common Data");
            sb.AppendLine("------------------------");
            sb.AppendLine(string.Format("Max Play Time: {0}", save.MaxGameTime.ToString()));
            sb.AppendLine(string.Format("Max Gald: {0:n0}", save.MaxGald));
            sb.AppendLine(string.Format("Total Gald Used: {0:n0}", save.TotalGaldUsed));
            sb.AppendLine(string.Format("Saves: {0:n0}", save.Saves));
            sb.AppendLine(string.Format("Game Cleared: {0}", save.GameCleared));
            sb.AppendLine();
            sb.AppendLine("Current");
            sb.AppendLine("------------------------");
            sb.AppendLine(string.Format("Game Time: {0}", save.GameTime.ToString()));
            sb.AppendLine(string.Format("Gald: {0:n0}", save.Gald));
            sb.AppendLine(string.Format("Encounters: {0:n0}", save.Encounters));
            sb.AppendLine(string.Format("Combo: {0} hit", save.MaxCombo));
            sb.AppendLine(string.Format("Grade: {0:n2}", save.Grade));
            sb.AppendLine();
            sb.AppendLine("Challenge Title Progress");
            sb.AppendLine("------------------------");
            sb.AppendLine($"Default Weapon?: {save.DefaultEquip}");
            sb.AppendLine($"Used Gels: {save.GelsUsed}");
            sb.AppendLine($"No Deaths: {save.HaveDied}");
            sb.AppendLine();
            sb.AppendLine("Battle Record");
            sb.AppendLine("------------------------");
            sb.AppendLine(string.Format("Encounters: {0:n0}", save.MaxEncounters));
            sb.AppendLine(string.Format("Escapes: {0:n0}", save.Escapes));
            sb.AppendLine(string.Format("Max Combo: {0} hit", save.MaxCombo));
            sb.AppendLine(string.Format("Max Damage: {0:n0}", save.MaxDmg));
            sb.AppendLine(string.Format("Max Grade: {0:n2}", save.MaxGrade));
            sb.AppendLine();
            sb.AppendLine($"Battles: {save.Battles:n0}");
            sb.AppendLine("------------------------");
            for (int i = 0; i < 9; i++)
            {
                sb.AppendFormat("{0} Battles: {1:n0}\r\n", save.Characters[i].Name, save.Characters[i].Battles);
            }
            sb.AppendLine();
            sb.AppendLine("Kills");
            sb.AppendLine("------------------------");
            for (int i = 0; i < 9; i++)
            {
                sb.AppendFormat("{0} Kills: {1:n0}\r\n", save.Characters[i].Name, save.Characters[i].Kills);
            }
            sb.AppendLine();
            //sb.AppendLine(save.TreasureString);
            lblInfo.Text = sb.ToString();
            sb = null;
        }

        private void UpdateCounts(GameSave save)
        {
            // Treasure
            lbxTreasures.Items.Clear();
            for (int x = 0; x < save.Treasure.Length; x++)
            {
                if (!save.Treasure[x] && x != 239)
                    lbxTreasures.Items.Add(string.Format("{0}: {1}", x + 1, Globals.TreasureNames[x]));
            }

            // Gigolo
            lbxWomen.Items.Clear();
            for (int x = 0; x < save.Gigolo.Length; x++)
            {
                if (!save.Gigolo[x] && Globals.WomenNames[x] != "None")
                    lbxWomen.Items.Add(string.Format("{0}: {1}", x + 1, Globals.WomenNames[x]));
            }

            // Dog Lover
            lbxDogs.Items.Clear();
            for (int x = 0; x < save.DogLover.Length; x++)
            {
                if (!save.DogLover[x] && Globals.DogNames[x] != "None")
                    lbxDogs.Items.Add(string.Format("{0}: {1}", x + 1, Globals.DogNames[x]));
            }
        }

        private void UpdateSaveString(GameSave save)
        {
            //tbxJSON.Text = new JSonPresentationFormatter().Format(save);
            tbxJSON.Text = JsonConvert.SerializeObject(save, Formatting.Indented);
        }

        private void SaveFile()
        {
            if (save.Saves.Count == 0 || cbxSaves.SelectedIndex > save.Saves.Count - 1)
                return;

            SaveFileDialog svd = new SaveFileDialog();
            switch(saveType)
            {
                case SaveType.GC:
                    svd.Filter = "GameCube Save|*.gci;*.gcs;*.sav";
                    break;
                case SaveType.PS3:
                    svd.Filter = "PS3 Save File|*.bin;*.pfd;*.sfo";
                    break;
                case SaveType.PS4:
                    svd.Filter = "PS4 Unencrypted File|*.dat";
                    break;
                case SaveType.Switch:
                    svd.Filter = "Switch Save File|*.dat";
                    break;
                case SaveType.PCRaw:
                    svd.Filter = "PC Raw File|*.dat";
                    break;
                default:
                    svd.Filter = "Raw Save File|*.dat";
                    break;
            }
            if ((bool)!svd.ShowDialog())
                return;

            filename = svd.FileName;

            GameSave s = save.Saves[cbxSaves.SelectedIndex];
            try
            {
                s = JsonConvert.DeserializeObject<GameSave>(tbxJSON.Text);
                save.Saves[cbxSaves.SelectedIndex] = s;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error Serializing Edit", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            save.WriteSave(filename, cbxSaves.SelectedIndex);

            MessageBox.Show(Path.GetFileName(filename), "File Saved", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private string GetThankYou()
        {
            thankyou = new Random().Next(Globals.ThankYous.GetLength(0));
            return Globals.ThankYous[thankyou,0];
        }

        #region Form Code
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Title = $"Symphonia Save Editor v{version.Major}.{version.Minor}.{version.Build} ({version.Revision})";
            lblThankyou.Content = GetThankYou();

            treasureCount = (short)Globals.TreasureNames.Count(t => !t.Contains("None"));
            itemCount = (short)Globals.ItemNames[saveType].Count(i => !i.Contains("None"));
            dogCount = (short)Globals.DogNames.Count(d => !d.Contains("None"));
            womenCount = (short)Globals.WomenNames.Count(w => !w.Contains("None"));
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.DefaultExt = "*.gci, *.gcs, *.sav";
            ofd.Filter = "GameCube Save files (*.gci, *.gcs, *.sav)|*.gci;*.gcs;*.sav|PS3 Save files (*.bin, *.pfd, *.sfo)|*.pfd;*.sfo;*.bin|Switch Save File (*.dat)|*.dat|PS4 Unencrypted Save (*.dat)|*.dat|PC Save Files (*.dat)|*.dat;|All files (*.*)|*.*";

            var result = ofd.ShowDialog();
            if (result == true)
            {
                ClearFields();

                filename = ofd.FileName;
                if (ofd.FileNames.Length > 1)
                {
                    //unencrypted = false;
                    save = new PS3Save(ofd.FileNames);
                }
                else if (filename.ToLower().EndsWith(".bin"))
                {
                    //unencrypted = true;
                    save = new PS3Save(ofd.FileNames);
                }
                else if (filename.ToLower().EndsWith(".dat"))
                {
                    if (ofd.FilterIndex == 3)
                        save = new SwitchSave(filename);
                    else if (ofd.FilterIndex == 4)
                        save = new PS4Save(filename);
                    else
                        save = new PCSave(filename);
                }
                else
                    save = new GCSave(filename);

                if (save.Saves.Count == 1) cbxSaves.Items.Add(save.Saves[0].SaveName);
                else cbxSaves.Items.Add(save.Saves.Select(x => x.SaveName).ToArray());

                if (cbxSaves.Items.Count > 0)
                    cbxSaves.SelectedIndex = 0;
            }
        }

        private void cbxSaves_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetData();

            switch (tabControl.SelectedIndex)
            {
                case 0:
                    ShowTreasure();
                    break;
                case 1:
                    ShowWomen();
                    break;
                case 2:
                    ShowDogs();
                    break;
                case 3:
                    ShowItems();
                    break;
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedIndex == previousTab)
                return;
            previousTab = tabControl.SelectedIndex;
            switch (tabControl.SelectedIndex)
            {
                case 1:
                    ShowTreasure();
                    break;
                case 2:
                    ShowWomen();
                    break;
                case 3:
                    ShowDogs();
                    break;
                case 4:
                    ShowItems();
                    break;
            }
        }

        private void cbxStatChar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCharStats(save.Saves[cbxSaves.SelectedIndex], cbxStatChar.SelectedIndex);
        }

        private void chkTech_Unchecked(object sender, RoutedEventArgs e)
        {
            var i = (int)((CheckBox)e.OriginalSource).Tag;
            save.Saves[cbxSaves.SelectedIndex].Characters[cbxStatChar.SelectedIndex].Techs[i] = false;
            UpdateSaveString(save.Saves[cbxSaves.SelectedIndex]);
        }

        private void chkTech_Checked(object sender, RoutedEventArgs e)
        {
            var i = (int)((CheckBox)e.OriginalSource).Tag;
            save.Saves[cbxSaves.SelectedIndex].Characters[cbxStatChar.SelectedIndex].Techs[i] = true;
            UpdateSaveString(save.Saves[cbxSaves.SelectedIndex]);
        }

        private void chkTitle_Checked(object sender, RoutedEventArgs e)
        {
            var i = (int)((CheckBox)e.OriginalSource).Tag;
            save.Saves[cbxSaves.SelectedIndex].Characters[cbxStatChar.SelectedIndex].Titles[i] = true;
            UpdateSaveString(save.Saves[cbxSaves.SelectedIndex]);
        }

        private void chkTitle_Unchecked(object sender, RoutedEventArgs e)
        {
            var i = (int)((CheckBox)e.OriginalSource).Tag;
            save.Saves[cbxSaves.SelectedIndex].Characters[cbxStatChar.SelectedIndex].Titles[i] = false;
            UpdateSaveString(save.Saves[cbxSaves.SelectedIndex]);
        }

        private void btnSaveEdit_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        private void btnMaxGald_Click(object sender, RoutedEventArgs e)
        {
            int i = cbxSaves.SelectedIndex;
            save.Saves[i].Gald = 999999;
            save.Saves[i].GaldCurrent = 999999;
            save.Saves[i].MaxGald = 999999;
            UpdateSaveString(save.Saves[i]);
            UpdateStats(save.Saves[i]);
            lblMaxGald.Content = "Set gald to 999,999";
        }

        private void btnMaxStats_Click(object sender, RoutedEventArgs e)
        {
            int i = cbxSaves.SelectedIndex;
            for (int c = 0; c < 9; c++)
            {
                save.Saves[i].Characters[c].Acc = 1999;
                save.Saves[i].Characters[c].Atk = 1999;
                save.Saves[i].Characters[c].Atk2 = 1999;
                save.Saves[i].Characters[c].Def = 1999;
                save.Saves[i].Characters[c].Eva = 1999;
                save.Saves[i].Characters[c].Exp = 999999;
                save.Saves[i].Characters[c].HP = 9999;
                save.Saves[i].Characters[c].Int = 1999;
                save.Saves[i].Characters[c].Lck = 1999;
                save.Saves[i].Characters[c].Level = 250;
                save.Saves[i].Characters[c].MaxHP = 9999;
                save.Saves[i].Characters[c].MaxTP = 999;
                save.Saves[i].Characters[c].Overlimit = 100;
                save.Saves[i].Characters[c].Status = 0;
                save.Saves[i].Characters[c].Str = 1999;
                save.Saves[i].Characters[c].TP = 999;
            }
            UpdateSaveString(save.Saves[i]);
            lblMaxStats.Content = "Set all characters stats to maximums";
        }

        private void btnMaxGrade_Click(object sender, RoutedEventArgs e)
        {
            int i = cbxSaves.SelectedIndex;
            save.Saves[i].Grade = 9999.00;
            save.Saves[i].MaxGrade = 9999.00;
            UpdateSaveString(save.Saves[i]);
            UpdateStats(save.Saves[i]);
            lblMaxGrade.Content = "Set grade to 9,999.0";
        }

        private void btnMaxItems_Click(object sender, RoutedEventArgs e)
        {
            int i = cbxSaves.SelectedIndex;
            byte q = 1;
            for (int n = 0; n < save.Saves[i].Items.Length; n++)
            {
                // 50, skip 24 key items, 198
                if (n > 50 && n < 74)
                    save.Saves[i].Items[n] = 1;
                else
                    save.Saves[i].Items[n] = 20;
                
                //save.Saves[i].Items[n] = q++;
                //if (q > 20) q = 1;
            }
            UpdateSaveString(save.Saves[i]);
            UpdateItems(save.Saves[i]);
            lblMaxItems.Content = "Set all items to 20, and some key items to 1";
        }

        private void btnMaxTechs_Click(object sender, RoutedEventArgs e)
        {
            int i = cbxSaves.SelectedIndex;
            for (int c = 0; c < 9; c++)
            {
                for (int t = 0; t < save.Saves[i].Characters[c].TechUses.Length; t++)
                {
                    save.Saves[i].Characters[c].TechUses[t] = 999;
                }
            }
            UpdateSaveString(save.Saves[i]);
            lblMaxTechs.Content = "Set all tech uses to 999";
        }

        private void btnFixChecksum_Click(object sender, RoutedEventArgs e)
        {
            if (save.saveType == SaveType.PCRaw || save.saveType == SaveType.PS3)
            {
                MessageBox.Show("This game platform version doesn't use checksums in the save file.");
                return;
            }

            int i = cbxSaves.SelectedIndex;
            save.CalculateChecksum(i);

            if (save.Saves[i].Checksum1 == save.Checksum1 && save.Saves[i].Checksum2 == save.Checksum2)
                lblFixChecksum.Content = "Checksums OK";
            else
            {
                lblFixChecksum.Content = string.Format("Expected: {0:X2}{1:X2}", save.Checksum1, save.Checksum2);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Invalid Checksums.");
                if (save.Saves[i].Checksum1 != save.Checksum1)
                    sb.AppendFormat("1. Found: {0:X2} Expected: {1:X2}\r\n", save.Saves[i].Checksum1, save.Checksum1);
                else
                    sb.AppendLine("1. Checksum OK");
                if (save.Saves[i].Checksum2 != save.Checksum2)
                    sb.AppendFormat("2. Found: {0:X2} Expected: {1:X2}\r\n", save.Saves[i].Checksum2, save.Checksum2);
                else
                    sb.AppendLine("2. Checksum OK");
                sb.AppendLine();
                sb.Append("Your checksum will be generated on saving, so edit whatever else you wish, then hit save to fix checksum automatically.");
                MessageBox.Show(sb.ToString(), "Checksum Mismatch");
            }
        }

        private void btnAllTitles_Click(object sender, RoutedEventArgs e)
        {
            int i = cbxSaves.SelectedIndex;

            save.Saves[i].Characters[0].Titles = new byte[] { 0xFE,0xD7,0xFF,0x7F}.ToBoolArray();
            save.Saves[i].Characters[1].Titles = new byte[] { 0xFE,0xFF,0xFF,0x00}.ToBoolArray();
            save.Saves[i].Characters[2].Titles = new byte[] { 0xFE,0xFF,0x7F,0x00}.ToBoolArray();
            save.Saves[i].Characters[3].Titles = new byte[] { 0xFE,0xFF,0x0F,0x00}.ToBoolArray();
            save.Saves[i].Characters[4].Titles = new byte[] { 0xFE,0xFF,0x0F,0x00}.ToBoolArray();
            save.Saves[i].Characters[5].Titles = new byte[] { 0xFE,0xFF,0x07,0x00}.ToBoolArray();
            save.Saves[i].Characters[6].Titles = new byte[] { 0xFE,0xFF,0x03,0x00}.ToBoolArray();
            save.Saves[i].Characters[7].Titles = new byte[] { 0xFE,0xFF,0x03,0x00}.ToBoolArray();
            save.Saves[i].Characters[8].Titles = new byte[] { 0xFE,0x17,0x00,0x00}.ToBoolArray();
            
            UpdateSaveString(save.Saves[i]);
            lblAllTitles.Content = "All titles unlocked.";
        }

        private void btnAllTechs_Click(object sender, RoutedEventArgs e)
        {
            int i = cbxSaves.SelectedIndex;

            save.Saves[i].Characters[0].Techs = new byte[]{0xFF,0xFF,0x7F,0xFE,0x03}.ToBoolArrayLow();
            save.Saves[i].Characters[1].Techs = new byte[]{0xFF,0xAF,0x8B,0x5F,0x00}.ToBoolArrayLow();
            save.Saves[i].Characters[2].Techs = new byte[]{0xBF,0xFF,0xFB,0xB9,0x07}.ToBoolArrayLow();
            save.Saves[i].Characters[3].Techs = new byte[]{0x79,0xDB,0xF6,0x7F,0x00}.ToBoolArrayLow();
            save.Saves[i].Characters[4].Techs = new byte[]{0xFF,0xFF,0xFF,0xFD,0x0F}.ToBoolArrayLow();
            save.Saves[i].Characters[5].Techs = new byte[]{0xFF,0xFF,0xFF,0x1D,0x00}.ToBoolArrayLow();
            save.Saves[i].Characters[6].Techs = new byte[]{0xEF,0xDF,0xDF,0x01,0x00}.ToBoolArrayLow();
            save.Saves[i].Characters[7].Techs = new byte[]{0x7F,0xFE,0xFF,0x07,0x00}.ToBoolArrayLow();
            save.Saves[i].Characters[8].Techs = new byte[]{0xFF,0xFF,0xFF,0x1D,0x00}.ToBoolArrayLow();

            UpdateSaveString(save.Saves[i]);
            lblAllTechs.Content = "All techs unlocked.";
        }

        private void btnMaxCooking_Click(object sender, RoutedEventArgs e)
        {
            int i = cbxSaves.SelectedIndex;

            for (int n = 0; n < 9; n++)
            {
                for (int c = 0; c < 24; c++)
                {
                    save.Saves[i].Characters[n].Cooking[c] = 8;
                }
            }

            UpdateSaveString(save.Saves[i]);
            lblMaxCooking.Content = "Cooking maxed.";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        private void sldItemQty_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (codeControl)
                return;
            int i = cbxSaves.SelectedIndex;
            save.Saves[i].Items[currentItem] = (byte)sldItemQty.Value;
            lbxItems.Items[currentItem] = string.Format("{0} : {1}", Globals.ItemNames[saveType][currentItem], save.Saves[i].Items[currentItem]);
            UpdateSaveString(save.Saves[i]);
        }

        private void lbxItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxSaves.SelectedIndex < 0 || lbxItems.SelectedIndex < 0)
                return;
            codeControl = true;
            currentItem = lbxItems.SelectedIndex;
            sldItemQty.Value = save.Saves[cbxSaves.SelectedIndex].Items[lbxItems.SelectedIndex];
            codeControl = false;
        }

        private void lblThankyou_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBox.Show(Globals.ThankYous[thankyou, 1], Globals.ThankYous[thankyou, 0], MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        #region Old Code

        /*private void OpenPS3Save(string[] filenames)
        {
            btnFixChecksum.IsEnabled = false;
            if (unencrypted)
                bytes = File.ReadAllBytes(filenames[0]);
            else
            {
                filename = filenames.Where(f => f.EndsWith("SAVEDATA.BIN")).FirstOrDefault();
                //define the securefile id
                byte[] key = new byte[] { 0xD5, 0x3D, 0xCD, 0x6C, 0xAC, 0x63, 0x8A, 0xA4, 0x56, 0xD6, 0x8B, 0x9F, 0x47, 0x21, 0x95, 0x37 };
                //declare ps3 manager using the directory path, and the secure file id
                manager = new Ps3SaveManager(System.IO.Path.GetDirectoryName(filenames[0]), key);
                //get file entry using name
                Ps3File file = manager.Files.FirstOrDefault(t => t.PFDEntry.file_name == "SAVEDATA.BIN");
                //define byte array that the decrypted data should be allocated
                bytes = null;
                //check if file is not null
                if (file != null)
                    bytes = file.DecryptToBytes();
                if (bytes != null)
                {
                    cbxSaves.Items.Clear();
                    //saves = new List<GameSave>();

                    //do stuff with the decrypted data
                    StreamWriter sw = new StreamWriter("Converted.bin");
                    BinaryWriter bw = new BinaryWriter(sw.BaseStream);
                    bw.Write(bytes);
                    bw.Close();
                }
            }

            BinaryReader br = new BinaryReader(new MemoryStream(bytes));
            GameSave save = new GameSave();

            littleEndian = true; // Gamesave is little Endian, so we need to switch it even for .SAV files

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
            br.BaseStream.Seek(0xF0C, SeekOrigin.Begin);
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
            byte flag = br.ReadByte();
            save.GelsUsed = (flag & 8) == 8;
            save.HaveDied = (flag & 2) == 2;
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

            cbxSaves.Items.Add("PS3 Save");
            //saves.Add(save);
        }*/

        /*private void OpenRawSave(string filename)
        {
            if (saveType != SaveType.PS4)
                btnFixChecksum.IsEnabled = false;

            cbxSaves.Items.Clear();
            //saves = new List<GameSave>();

            bytes = File.ReadAllBytes(filename);

            StreamReader sr = new StreamReader(filename);
            BinaryReader br = new BinaryReader(sr.BaseStream);

            GameSave save = new GameSave(saveType);
            littleEndian = false;

            save.Checksum1 = ReverseUInt(br.ReadBytes(4));
            save.Checksum2 = ReverseUInt(br.ReadBytes(4));
            br.BaseStream.Seek(0x10, SeekOrigin.Begin);
            save.GameTime = new GameTime(ReverseUInt(br.ReadBytes(4)));
            save.Gald = ReverseUInt(br.ReadBytes(4));
            save.Encounters = ReverseUShort(br.ReadBytes(2));
            save.Combo = ReverseUShort(br.ReadBytes(2));
            // read party here as well (only for save header?)
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
                save.MonsterList[i] = br.ReadByte();

            // Characters
            SeekTo(br, 0x548);
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
                //br.ReadByte();// this may be extra titles
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
                br.ReadByte();
                save.Characters[i].Affection = ReverseUShort(br.ReadBytes(2));
                //br.BaseStream.Seek(0x2c, SeekOrigin.Current);//0x2c,0x2d
                //seek 0xb, read 5 tech mask bytes
                br.BaseStream.Seek(0x16, SeekOrigin.Current);
                save.Characters[i].Techs = ReverseBytes(br.ReadBytes(5)).ToBoolArray();

                br.BaseStream.Seek(0x13, SeekOrigin.Current);
                for (int t = 0; t < save.Characters[i].TechUses.Length; t++)
                {
                    save.Characters[i].TechUses[t] = ReverseUShort(br.ReadBytes(2));
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
            SeekTo(br, 0xFB4);
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
            SeekTo(br, 0x1004);
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
            SeekTo(br, 0x109E);
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

            SeekTo(br, 0x111D);
            for (int i = 0; i < 8; i++)
                save.Party[i] = br.ReadByte();

            // Items
            SeekTo(br, 0x112E);
            for (int i = 0; i < Globals.ItemNames[saveType].Length; i++)
                save.Items[i] = br.ReadByte();

            // Collector's Book
            SeekTo(br, 0x13a4);
            for (int i = 0; i < 0x43; i++)
                save.CollectorsBook[i] = br.ReadByte();

            SeekTo(br, 0x1CAF); // maybe 1cb0 but fat chance
            save.Recipes = ReverseBytes(br.ReadBytes(4)).ToBoolArray();
            br.ReadBytes(7);
            save.EncounterMode = br.ReadByte();

            // Battles
            SeekTo(br, 0x1CD6);
            for (int i = 0; i < 9; i++)
                save.Characters[i].CurrentBattles = ReverseUShort(br.ReadBytes(2));

            // HardMode
            save.UnisonGauge = br.ReadByte();
            br.ReadBytes(2);
            byte flag = br.ReadByte();
            save.GelsUsed = (flag & 8) == 8;
            save.HaveDied = (flag & 2) == 2;
            br.ReadBytes(2);
            save.HardModeBattles = ReverseUShort(br.ReadBytes(2));

            // Deaths
            SeekTo(br, 0x1D04);
            for (int i = 0; i < 9; i++)
                save.Characters[i].Deaths = br.ReadByte();
            // Battle Items Used
            for (int i = 0; i < 9; i++)
                save.Characters[i].BattleItemsUsed = br.ReadByte();
            // Kills
            for (int i = 0; i < 9; i++)
                save.Characters[i].Kills = ReverseUShort(br.ReadBytes(2));
            // Figurine Book
            br.ReadBytes(4);
            for (int i = 0; i < 36; i++)
            {
                save.FigurineBook[i] = br.ReadByte();
            }

            // Grade 0x1d84
            SeekTo(br, 0x1DC4); // -4 spent
            save.Grade = ReverseUInt(br.ReadBytes(4)) / 100.0;

            switch (saveType)
            {
                case SaveType.PCRaw:
                    cbxSaves.Items.Add("PC Save");
                    break;
                case SaveType.Switch:
                    cbxSaves.Items.Add("Switch Save");
                    break;
                case SaveType.PS4:
                    cbxSaves.Items.Add("PS4 Save");
                    break;
                default:
                    cbxSaves.Items.Add("Raw Save");
                    break;
            }
            //saves.Add(save);
        }*/

        /*private void OpenGCSave(string filename)
        {
            btnFixChecksum.IsEnabled = true;

            bytes = File.ReadAllBytes(filename);

            StreamReader sr = new StreamReader(filename);
            BinaryReader br = new BinaryReader(sr.BaseStream);

            cbxSaves.Items.Clear();
            saves = new List<GameSave>();
            string extension = System.IO.Path.GetExtension(filename).ToUpper();
            switch (extension)
            {
                case ".GCI":
                    offset = offsetGCI;
                    littleEndian = true;
                    break;
                case ".GCS":
                    offset = offsetGCS;
                    littleEndian = true;
                    break;
                case ".SAV":
                    offset = offsetSAV;
                    littleEndian = false;
                    break;
                default:
                    offset = 0;
                    break;
            }

            uint check1 = 0, check2 = 0;
            CalculateChecksum(bytes, offset, out check1, out check2);

            br.BaseStream.Seek(offset, SeekOrigin.Begin);

            bool regionPrompted = false;
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                GameSave save = new GameSave();

                string id = Encoding.Default.GetString(br.ReadBytes(3));
                byte region = br.ReadByte();

                if (id != gameID)
                {
                    br.BaseStream.Seek(saveSize - 4, SeekOrigin.Current);
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
                if (littleEndian)
                    graphicFormat = br.ReadByte();
                else
                    br.ReadByte();
                string fileID = Encoding.Default.GetString(br.ReadBytes(32));
                save.LastModified = new DateTime(1999, 12, 31, 23, 59, 59).AddSeconds(ReverseUInt(br.ReadBytes(4)));

                br.BaseStream.Seek(52, SeekOrigin.Current);
                littleEndian = true; // Gamesave is little Endian, so we need to switch it even for .SAV files
                string saveText = Encoding.Default.GetString(br.ReadBytes(32));
                cbxSaves.Items.Add(saveText);

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
                    save.Characters[i].Techs = ReverseBytes(br.ReadBytes(5)).ToBoolArray();

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
                byte flag = br.ReadByte();
                save.GelsUsed = (flag & 8) == 8;
                save.HaveDied = (flag & 2) == 2;
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
                if (check1 != save.Checksum1 && check2 != save.Checksum2)
                    lblChecksum.Content = "Invalid checksums: 2!";
                else if (check1 != save.Checksum1)
                    lblChecksum.Content = "Invalid checksum 1!";
                else if (check2 != save.Checksum2)
                    lblChecksum.Content = "Invalid checksum 2!";

                saves.Add(save);
            }

            br.Close();
        }*/

        /*private void WriteGCSave(string filename)
        {
            MemoryStream ms = new MemoryStream(bytes);
            BinaryWriter bw = new BinaryWriter(ms);

            string extension = System.IO.Path.GetExtension(filename).ToUpper();
            switch (extension)
            {
                case ".GCI":
                    offset = offsetGCI;
                    littleEndian = true;
                    break;
                case ".GCS":
                    offset = offsetGCS;
                    littleEndian = true;
                    break;
                case ".SAV":
                    offset = offsetSAV;
                    littleEndian = false;
                    break;
                default:
                    offset = 0;
                    break;
            }

            bw.BaseStream.Seek(offset, SeekOrigin.Begin);

            GameSave save = saves[cbxSaves.SelectedIndex];

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
            byte flag = 0;
            flag += save.GelsUsed ? (byte)8 : (byte)0;
            flag += save.HaveDied ? (byte)2 : (byte)0;
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

            uint check1 = 0;
            uint check2 = 0;
            CalculateChecksum(bytes, offset, out check1, out check2);
            bw.BaseStream.Seek(0x2080 + offset, SeekOrigin.Begin);
            bw.WriteReverse(check1);
            bw.WriteReverse(check2);
            bw.Close();

            StreamWriter sw = new StreamWriter(filename);
            bw = new BinaryWriter(sw.BaseStream);
            bw.Write(ms.ToArray());
            bw.Close();
        }*/

        /*private void WriteRawSave(string filename)
        {
            MemoryStream ms = new MemoryStream(bytes);
            BinaryWriter bw = new BinaryWriter(ms);

            GameSave save = saves[cbxSaves.SelectedIndex];

            // Saves that use checksum header
            if (saveType == SaveType.PS4)
            {
                bw.Seek(0, SeekOrigin.Begin);
                bw.Write((uint)0);
                bw.Write((uint)0);
            }

            bw.BaseStream.Seek(0x10, SeekOrigin.Begin);
            bw.Write((uint)(save.GameTime.TotalFrames));
            bw.Write(save.Gald);
            bw.Write(save.Encounters);
            bw.Write(save.Combo);
            bw.BaseStream.Seek(0x10, SeekOrigin.Current);

            // Records
            bw.Write((uint)(save.MaxGameTime.TotalFrames));
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
            SeekTo(bw, 0x548);
            for (int i = 0; i < 9; i++)
            {
                bw.Write(save.Characters[i].Level);
                bw.BaseStream.Seek(7, SeekOrigin.Current);
                bw.Write(save.Characters[i].Exp);
                bw.BaseStream.Seek(3, SeekOrigin.Current);
                bw.Write(save.Characters[i].Status);

                work = save.Characters[i].Titles.To4ByteArray(true);
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
                work = save.Characters[i].Techs.To4ByteArray(true);
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
            SeekTo(bw, 0xFB4);
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
            SeekTo(bw, 0x1004);
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
            SeekTo(bw, 0x109E);
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
            SeekTo(bw, 0x111d);
            for (int i = 0; i < 8; i++)
                bw.Write(save.Party[i]);

            // Items
            SeekTo(bw, 0x112E);
            for (int i = 0; i < Globals.ItemNames[saveType].Length; i++)
                bw.Write(save.Items[i]);

            // Collector's Book
            SeekTo(bw, 0x13a4);
            for (int i = 0; i < 0x43; i++)
                bw.Write(save.CollectorsBook[i]);

            SeekTo(bw, 0x1caf);// maybe 1cb0 but fat chance
            bw.Write(save.Recipes.To4ByteArray(false));
            bw.BaseStream.Seek(7, SeekOrigin.Current);
            bw.Write(save.EncounterMode);

            // Battles
            SeekTo(bw, 0x1cd6);
            for (int i = 0; i < 9; i++)
                bw.Write(save.Characters[i].CurrentBattles);

            // HardMode
            bw.Write(save.UnisonGauge);
            bw.BaseStream.Seek(2, SeekOrigin.Current);
            // 1 = ? saw in Noxbur's save
            byte flag = 0;
            flag += save.GelsUsed ? (byte)8 : (byte)0;
            flag += save.HaveDied ? (byte)2 : (byte)0;
            bw.Write(flag);
            bw.BaseStream.Seek(2, SeekOrigin.Current);
            bw.Write(save.HardModeBattles);

            // Deaths
            SeekTo(bw, 0x1d04);
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
            SeekTo(bw, 0x1dc4);
            bw.Write((uint)(save.Grade * 100));

            // Checksum
            uint check1 = 0;
            uint check2 = 0;
            CalculateChecksum(bytes, offset, out check1, out check2);
            bw.BaseStream.Seek(0 + offset, SeekOrigin.Begin);
            bw.Write(check1);
            bw.Write(check2);

            bw.Close();

            StreamWriter sw = new StreamWriter(filename);
            bw = new BinaryWriter(sw.BaseStream);
            bw.Write(ms.ToArray());
            bw.Close();
        }*/

        /*private void WritePS3Save(string filename)
        {
            MemoryStream ms = new MemoryStream(fileData);
            BinaryWriter bw = new BinaryWriter(ms);

            GameSave save = saves[cbxSaves.SelectedIndex];

            bw.BaseStream.Seek(0x10, SeekOrigin.Begin);
            bw.WriteReverse((uint)(save.GameTime.TotalFrames));
            bw.WriteReverse(save.Gald);
            bw.WriteReverse(save.Encounters);
            bw.WriteReverse(save.Combo);
            bw.BaseStream.Seek(0x10, SeekOrigin.Current);

            // Records
            bw.WriteReverse((uint)(save.MaxGameTime.TotalFrames));
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
            bw.WriteReverse((uint)(save.GameTimeCurrent.TotalFrames));
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
            byte flag = 0;
            flag += save.GelsUsed ? (byte)8 : (byte)0;
            flag += save.HaveDied ? (byte)2 : (byte)0;
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
        }*/

        /*private void CalculateChecksum(byte[] bytes, int offset, out uint check1, out uint check2)
        {
            check1 = 0;
            check2 = 0;
            bool reverse = saveType == SaveType.GC;
            int size = saveType == SaveType.GC ? 0xa75 : 0x985;
            int start = offset;
            if (saveType == SaveType.GC)
                start += 0x2080;

            uint work = 0;
            byte[] array = new byte[4];
            // Calculate Checksum 1
            for (int i = 0; i < size; i++)
            {
                Array.Copy(bytes, start + i * 4, array, 0, 4);
                if (reverse) Array.Reverse(array);
                work = BitConverter.ToUInt32(array, 0);
                if (i < 2)
                    work = 0;
                check1 += work;
            }

            // Calculate Checksum 2
            for (int i = 0; i < 100; i++)
            {
                Array.Copy(bytes, start + i * 4, array, 0, 4);
                if (reverse) Array.Reverse(array);
                work = BitConverter.ToUInt32(array, 0);
                if (i < 2)
                    work = 0;
                check2 += work;
            }

            //check1 = ReverseBytes(check1);
            //check2 = ReverseBytes(check2);
        }*/

        #endregion
    }
}
