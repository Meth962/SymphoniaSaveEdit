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
using System.Configuration;
using System.Xml.Schema;
using Path = System.IO.Path;
using System.IO;

namespace SymphoniaSaveEdit
{
    public partial class MainWindow : Window
    {
        private short treasureCount = 0;
        private short itemCount = 0;
        private short dogCount = 0;
        private short womenCount = 0;
        private int thankyou = 0;

        private SaveFile save;
        private Settings settings = new Settings();
        private string filename = string.Empty;

        // Ugly WPF fault coding
        private int previousTab = -1;
        private bool codeControl = false;
        private int currentItem = -1;

        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("settings.json"));
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void UpdateSettings()
        {
            try
            {
                File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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
            cbxStatChar.SelectedIndex = -1;
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

        private void ClearCharStats()
        {
            cbxStatChar.SelectedIndex = -1;
            tbxStats.Clear();
            lbxTitles.Items.Clear();
            lbxTechs.Items.Clear();
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
            sb.AppendFormat("Weapon: {0}\r\n", c.Weapon == 0 ? string.Empty : Globals.ItemNames[save.saveType][c.Weapon - 1]);
            sb.AppendFormat("Armor: {0}\r\n", c.Armor == 0 ? string.Empty : Globals.ItemNames[save.saveType][c.Armor - 1]);
            sb.AppendFormat("Helm: {0}\r\n", c.Helm == 0 ? string.Empty : Globals.ItemNames[save.saveType][c.Helm - 1]);
            sb.AppendFormat("Arms: {0}\r\n", c.Arms == 0 ? string.Empty : Globals.ItemNames[save.saveType][c.Arms - 1]);
            sb.AppendFormat("Accessory: {0}\r\n", c.Accessory1 == 0 ? string.Empty : Globals.ItemNames[save.saveType][c.Accessory1 - 1]);
            sb.AppendFormat("Accessory: {0}\r\n", c.Accessory2 == 0 ? string.Empty : Globals.ItemNames[save.saveType][c.Accessory2 - 1]);
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
                if (i + 1 > Globals.ItemNames[this.save.saveType].Length)
                {
                    lbxItems.Items.Add($"Unknown : {save.Items[i]}");
                    Console.WriteLine($"Error itemId {i} does not exist in {this.save.saveType}");
                }
                else
                    lbxItems.Items.Add(string.Format("{0} : {1}", Globals.ItemNames[this.save.saveType][i], save.Items[i]));
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
            if (save == null || save.Saves.Count == 0 || cbxSaves.SelectedIndex > save.Saves.Count - 1)
                return;

            SaveFileDialog svd = new SaveFileDialog();
            switch(save.saveType)
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

            //MessageBox.Show(Path.GetFileName(filename), "File Saved", MessageBoxButton.OK, MessageBoxImage.Information);
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
            itemCount = (short)Globals.ItemNames[0].Count(i => !i.Contains("None"));
            dogCount = (short)Globals.DogNames.Count(d => !d.Contains("None"));
            womenCount = (short)Globals.WomenNames.Count(w => !w.Contains("None"));
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "PS4 Unencrypted Save (*.dat)|*.dat|Switch Save File (*.dat)|*.dat|PC Save Files (*.dat)|*.dat;|GameCube Save files (*.gci, *.gcs, *.sav)|*.gci;*.gcs;*.sav|PS3 Save files (*.bin, *.pfd, *.sfo)|*.pfd;*.sfo;*.bin|All files (*.*)|*.*";
            if (settings.LastSaveType > -1)
                ofd.FilterIndex = settings.LastSaveType;
            if (settings.LastSaveLocation != null)
                ofd.InitialDirectory = settings.LastSaveLocation;

            var result = ofd.ShowDialog();
            if (result == true)
            {
                ClearFields();
                settings.LastSaveType = ofd.FilterIndex;
                settings.LastSaveLocation = Path.GetDirectoryName(ofd.FileName);
                UpdateSettings();

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
                    if (ofd.FilterIndex == 2)
                        save = new SwitchSave(filename);
                    else if (ofd.FilterIndex == 1)
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
            if (cbxSaves.SelectedIndex < 0) return;
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
            if (save == null || cbxStatChar.SelectedIndex < 0) return;
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
            if (save == null) return;
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
            if (save == null) return;
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
            ClearCharStats();
            lblMaxStats.Content = "Set all characters stats to maximums";
        }

        private void btnMaxGrade_Click(object sender, RoutedEventArgs e)
        {
            if (save == null) return;
            int i = cbxSaves.SelectedIndex;
            save.Saves[i].Grade = 9999.00;
            save.Saves[i].MaxGrade = 9999.00;
            UpdateSaveString(save.Saves[i]);
            UpdateStats(save.Saves[i]);
            lblMaxGrade.Content = "Set grade to 9,999.0";
        }

        private void btnMaxItems_Click(object sender, RoutedEventArgs e)
        {
            if (save == null) return;
            int i = cbxSaves.SelectedIndex;
            //byte q = 1;
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
            if (save == null) return;
            int i = cbxSaves.SelectedIndex;
            for (int c = 0; c < 9; c++)
            {
                for (int t = 0; t < save.Saves[i].Characters[c].TechUses.Length; t++)
                {
                    save.Saves[i].Characters[c].TechUses[t] = 999;
                }
            }
            UpdateSaveString(save.Saves[i]);
            ClearCharStats();
            lblMaxTechs.Content = "Set all tech uses to 999";
        }

        private void btnFixChecksum_Click(object sender, RoutedEventArgs e)
        {
            if (save == null) return;
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
            if (save == null) return;
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
            ClearCharStats();
            lblAllTitles.Content = "All titles unlocked.";
        }

        private void btnAllTechs_Click(object sender, RoutedEventArgs e)
        {
            if (save == null) return;
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
            ClearCharStats();
            lblAllTechs.Content = "All techs unlocked.";
        }

        private void btnMaxCooking_Click(object sender, RoutedEventArgs e)
        {
            if (save == null) return;
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

        private void btnAllCollectibles_Click(object sender, RoutedEventArgs e)
        {
            if (save == null) return;
            int i = cbxSaves.SelectedIndex;

            save.Saves[i].FigurineBook = Globals.MAX_FIGURINES;
            save.Saves[i].MonsterList = Globals.MAX_MONSTERS;
            save.Saves[i].CollectorsBook = Globals.MAX_COLLECTORS;

            UpdateSaveString(save.Saves[i]);
            lblMaxCollectibles.Content = "Books completed!";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (save == null) return;
            SaveFile();
        }

        private void sldItemQty_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (save == null || codeControl)
                return;
            int i = cbxSaves.SelectedIndex;
            save.Saves[i].Items[currentItem] = (byte)sldItemQty.Value;
            lbxItems.Items[currentItem] = string.Format("{0} : {1}", Globals.ItemNames[save.saveType][currentItem], save.Saves[i].Items[currentItem]);
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

        private void lbxTreasures_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lbx = (ListBox)sender;
            if (lbx.SelectedIndex < 0) return;

            try
            {
                int idx = int.Parse(lbx.SelectedItem.ToString().Split(':')[0]);
                GameSave s = save.Saves[cbxSaves.SelectedIndex];
                s.Treasure[idx - 1] = true;

                lbx.Items.RemoveAt(lbx.SelectedIndex);
                UpdateSaveString(s);
            }
            catch { }
        }

        private void lbxGigolo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lbx = (ListBox)sender;
            if (lbx.SelectedIndex < 0) return;

            try
            {
                int idx = int.Parse(lbx.SelectedItem.ToString().Split(':')[0]);
                GameSave s = save.Saves[cbxSaves.SelectedIndex];
                s.Gigolo[idx - 1] = true;

                lbx.Items.RemoveAt(lbx.SelectedIndex);
                UpdateSaveString(s);
            }
            catch { }
        }

        private void lbxDogs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lbx = (ListBox)sender;
            if (lbx.SelectedIndex < 0) return;

            try
            {
                int idx = int.Parse(lbx.SelectedItem.ToString().Split(':')[0]);
                GameSave s = save.Saves[cbxSaves.SelectedIndex];
                s.DogLover[idx - 1] = true;

                lbx.Items.RemoveAt(lbx.SelectedIndex);
                UpdateSaveString(s);
            }
            catch { }
        }

        private void lblThankyou_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(Globals.ThankYous[thankyou, 1], Globals.ThankYous[thankyou, 0], MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion
    }
}
