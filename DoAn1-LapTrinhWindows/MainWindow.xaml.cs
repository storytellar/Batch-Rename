using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using Microsoft.Win32;
using Path = System.IO.Path;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace DoAn1_LapTrinhWindows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        class Preset : INotifyPropertyChanged
        {
            private string name;
            private string presetPath;

            public string Name
            {
                get => name; set
                {
                    name = value;
                    RaiseEvent("Name");
                }
            }
            public string PresetPath
            {
                get => presetPath; set
                {
                    presetPath = value;
                    RaiseEvent("PresetPath");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            void RaiseEvent(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        }
        BindingList<Preset> listPresetItem;
        public MainWindow()
        {
            InitializeComponent();
        }

        public class TargetInfo : INotifyPropertyChanged
        {
            public string name;
            public string newName;
            public string status;
            public string dir;
            public string extension;
            public string Name
            {
                get => name; set
                {
                    name = value;
                    RaiseEvent();
                }
            }

            public string Extension
            {
                get => extension; set
                {
                    extension = value;
                    RaiseEvent();
                }
            }

            public string Dir
            {
                get => dir; set
                {
                    dir = value;
                    RaiseEvent();
                }
            }

            public string NewName
            {
                get => newName; set
                {
                    newName = value;
                    RaiseEvent();
                }
            }

            public string Status
            {
                get => status; set
                {
                    status = value;
                    RaiseEvent();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            void RaiseEvent([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public interface IArgs
        {

        }
        /// <summary>
        /// Các hành động thao tác với chuối
        /// </summary>
        public interface IAction
        {
            IArgs Args { get; set; }

            string Process(string origin);
        }
        public class ReplaceArgs : IArgs
        {
            public string Needle { get; set; }
            public string Hammer { get; set; }
        }
        /// <summary>
        /// Thao tác Replace
        /// </summary>
        public class Replacer : IAction
        {
            public IArgs Args { get; set; }

            public string Process(string origin)
            {
                var args = Args as ReplaceArgs;
                var needle = args.Needle;
                var hammer = args.Hammer;

                var res = origin.Replace(needle, hammer);

                return res;
            }
        }

        public class ToUpperCase : IAction
        {
            public IArgs Args { get; set; }

            public string Process(string origin)
            {
                return origin.ToUpper();
            }
        }

        public class ToLowerCase : IAction
        {
            public IArgs Args { get; set; }

            public string Process(string origin)
            {
                return origin.ToLower();
            }
        }

        public class SpecialCase : IAction
        {
            public IArgs Args { get; set; }

            public string Process(string origin)
            {
                string kq = origin.ToLower();
                return kq.First().ToString().ToUpper() + kq.Substring(1);
            }
        }

        public class FullNameNormalizer : IAction
        {
            public IArgs Args { get; set; }

            public string Process(string origin)
            {
                string result = "";
                string str = origin;
                for (int i = 0; i < str.Length; i++)
                {
                    // Remove starting with trash character
                    while (!Char.IsLetter(str[0]))
                        str = str.Remove(0, 1);

                    // Remove ending with trash character
                    while (!Char.IsLetter(str[str.Length - 1]))
                        str = str.Remove(str.Length - 1);

                    // UpperCase and LowerCase
                    if (!Char.IsLetter(str[i]) && Char.IsLetter(str[i + 1]))
                    {
                        result = result + " " + Char.ToUpper(str[i + 1]); i++; continue;
                    }
                    else if (i == 0)
                    {
                        result = result + Char.ToUpper(str[i]);
                    }
                    else if (Char.IsLetter(str[i]))
                           result = result + Char.ToLower(str[i]);
                }
                return result;
            }
        }

        public class UniqueName: IAction
        {
            public IArgs Args { get; set; }

            public string Process(string origin)
            {
                string res = "";
                Guid g;
                g = Guid.NewGuid();
                res = g.ToString();
                return res;
            }
        }
        public class IDFirst : IAction
        {
            public IArgs Args { get; set; }

            public string Process(string origin)
            {
                string res = "";
                Regex myRex = new Regex(@"(\d)((\d|-){11})(\d)");         //Find the ISBN number.

                Match ISBN = myRex.Match(origin);

                res += ISBN.ToString();
                res += " ";
                res += origin.Replace(ISBN.ToString(), "");

                return res;
            }
        }
        public class NameFirst : IAction
        {
            public IArgs Args { get; set; }

            public string Process(string origin)
            {
                string res = "";
                Regex myRex = new Regex(@"(\d)((\d|-){11})(\d)");         //Find the ISBN number.

                Match ISBN = myRex.Match(origin);

                res += origin.Replace(ISBN.ToString(), "");
                res += " ";
                res += ISBN.ToString();

                return res;
            }
        }
        BindingList<TargetInfo> targets = new BindingList<TargetInfo>();
        List<IAction> actions = new List<IAction>();

        private void ClickBrowseButton(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\";
            dialog.Multiselect = true;

            if (FolderButton.IsChecked == true)
            {
                dialog.IsFolderPicker = true;

                if (targets.Count != 0 && targets[0].Extension != "")
                    targets.Clear();
            }

            if (FileButton.IsChecked == true)
            {
                dialog.IsFolderPicker = false;

                if (targets.Count != 0 && targets[0].Extension == "")
                    targets.Clear();
            }

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {

                foreach (string sFileName in dialog.FileNames)
                {
                    targets.Add(new TargetInfo { Name = Path.GetFileNameWithoutExtension(sFileName), NewName = "No name", Status = "Unchanged", Dir = Path.GetDirectoryName(sFileName) + "\\", Extension = Path.GetExtension(sFileName) });

                    for (int i = 0; i < targets.Count; i++)
                    {
                        for (int j = i + 1; j < targets.Count; j++) 
                        {
                            if (targets[i].Name + targets[i].Extension == targets[j].Name + targets[j].Extension)
                                targets.Remove(targets[j]);
                        }
                    }

                    lv.Items.Clear();
                    foreach (TargetInfo target in targets)
                    {
                        lv.Items.Add(target);
                    }

                    txtGetFile.Text = Path.GetDirectoryName(sFileName);
                }
            }
        }

        private void ClickRefreshButton(object sender, RoutedEventArgs e)
        {
            targets.Clear();
            lv.Items.Clear();
            txtGetFile.Text = "C:\\path...";
        }

        private void BatchButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReplaceBox.IsChecked == true)           
                actions.Add(new Replacer() { Args = new ReplaceArgs() { Needle = TextNeedle.Text, Hammer = TextHammer.Text } });

            if (NewCaseBox.IsChecked == true)
            {
                if (radioUpperAll.IsChecked == true)
                    actions.Add(new ToUpperCase());

                else if (radioLowerAll.IsChecked == true)
                    actions.Add(new ToLowerCase());

                else if (radioUpperFirstOne.IsChecked == true)
                    actions.Add(new SpecialCase());
            }

            if (FullNameNormalizeBox.IsChecked == true)
            {
                actions.Add(new FullNameNormalizer());
            }

            if (UniqueNameBox.IsChecked==true)
            {
                actions.Add(new UniqueName());
            }

            if(MoveBox.IsChecked == true)
            {
                if (IDFirstRadio.IsChecked == true)
                    actions.Add(new IDFirst());
                else if(NameFirstRadio.IsChecked == true)
                    actions.Add(new NameFirst());
            }

            foreach (var target in targets)
            {
                var res = target.Name;
                var temp = target.Name;
                var ext = target.Extension;

                foreach (var action in actions)
                {
                    res = action.Process(res);
                    int tail = 1;

                    if (target.Extension != "")
                    {
                        string[] infos = Directory.GetFiles(target.Dir);

                        foreach(var info in infos)
                        {
                            if(target.Dir + res + ext == info)
                            {
                                foreach (var smallInfo in infos)
                                    if (target.Dir + res + tail.ToString() + ext == smallInfo)
                                        tail++;
                                res += tail.ToString();
                            }
                        }
                        
                        File.Move(target.Dir + temp + ext, target.Dir + res + ext);
                    }

                    else
                    {
                        string[] infos = Directory.GetDirectories(target.Dir);

                        foreach (var info in infos)
                        {
                            if (target.Dir + res + ext == info)
                            {
                                foreach (var smallInfo in infos)
                                    if (target.Dir + res + tail.ToString() + ext == smallInfo)
                                        tail++;
                                res += tail.ToString();
                            }
                        }

                        Directory.Move(target.Dir + temp, target.Dir + temp + "_temp");
                        Directory.Move(target.Dir + temp + "_temp", target.Dir + res);
                    }
                    target.NewName = res;
                    temp = res;
                }

                target.Status = "Changed";
            }
        }

        private void ClickSavePresetButton(object sender, RoutedEventArgs e)
        {
            // save preset
            string timeNow = DateTime.Now.ToString("yyyy-MM-dd h_mm_ss tt");
            string presetContent = buildPresetContent();
            System.IO.File.WriteAllText(@".\presets\" + timeNow + ".bin", presetContent);
            listPresetItem.Add(new Preset() { Name = timeNow + ".bin", PresetPath = "./presets/" + timeNow + ".bin" });
            ComboBoxPreset.SelectedIndex = listPresetItem.Count - 1;
        }
        private string buildPresetContent()
        {
            string presetContent = "";
            string option = "";

            if (ReplaceBox.IsChecked == true)
            {
                presetContent += "Replace|TRUE|" + TextNeedle.Text + "|" + TextHammer.Text + "|";
            }
            else
            {
                presetContent += "Replace|FALSE|";
            }

            presetContent += "\r\n";
            if (NewCaseBox.IsChecked == true)
            {
                if (radioUpperAll.IsChecked == true)
                    option = "1";
                else if (radioLowerAll.IsChecked == true)
                    option = "2";
                else if (radioUpperFirstOne.IsChecked == true)
                    option = "3";
                presetContent += "NewCase|TRUE|" + option + "|";
            }
            else
            {
                presetContent += "NewCase|FALSE|";
            }

            presetContent += "\r\n";
            if (FullNameNormalizeBox.IsChecked == true)
            {
                presetContent += "FullName|TRUE|";
            }
            else
            {
                presetContent += "FullName|FALSE|";
            }

            presetContent += "\r\n";
            if (MoveBox.IsChecked == true)
            {
                if (IDFirstRadio.IsChecked == true)
                    option = "1";
                else if (NameFirstRadio.IsChecked == true)
                    option = "2";
                presetContent += "Move|TRUE|" + option + "|";
            }
            else
            {
                presetContent += "Move|FALSE|";
            }

            presetContent += "\r\n";
            if (UniqueNameBox.IsChecked == true)
            {
                presetContent += "UniqueName|TRUE|";
            }
            else
            {
                presetContent += "UniqueName|FALSE|";
            }
            return presetContent;
        }
        private void applyPreset(string path)
        {
            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                string[] pieces = line.Split('|');
                switch (pieces[0])
                {
                    case "Replace":
                        if (pieces[1] == "TRUE")
                        {
                            ReplaceBox.IsChecked = true;
                            TextNeedle.Text = pieces[2];
                            TextHammer.Text = pieces[3];
                        }
                        else
                            ReplaceBox.IsChecked = false;
                        break;
                    case "NewCase":
                        if (pieces[1] == "TRUE")
                        {
                            NewCaseBox.IsChecked = true;
                            if (pieces[2] == "1")
                                radioUpperAll.IsChecked = true;
                            else if (pieces[2] == "2")
                                radioLowerAll.IsChecked = true;
                            else if (pieces[2] == "3")
                                radioUpperFirstOne.IsChecked = true;
                        }
                        else
                            NewCaseBox.IsChecked = false;
                        break;
                    case "FullName":
                        if (pieces[1] == "TRUE")
                            FullNameNormalizeBox.IsChecked = true;
                        else
                            FullNameNormalizeBox.IsChecked = false;
                        break;
                    case "Move":
                        if (pieces[1] == "TRUE")
                        {
                            MoveBox.IsChecked = true;
                            if (pieces[2] == "1")
                                IDFirstRadio.IsChecked = true;
                            else if (pieces[2] == "2")
                                NameFirstRadio.IsChecked = true;
                        }
                        else
                            MoveBox.IsChecked = false;
                        break;
                    case "UniqueName":
                        if (pieces[1] == "TRUE")
                            UniqueNameBox.IsChecked = true;
                        else
                            UniqueNameBox.IsChecked = false; break;
                    default: break;

                }
            }

        }
        private void ClickRemovePresetButton(object sender, RoutedEventArgs e)
        {
            // remove preset
            var index = ComboBoxPreset.SelectedIndex;
            var path = ComboBoxPreset.SelectedValue;
            ComboBoxPreset.SelectedIndex = 0;
            if (index != 0)
            {
                listPresetItem.RemoveAt(index);
                File.Delete(path.ToString());
            }

        }
        private void ChangePreset(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxPreset.SelectedValue.ToString() != "0")
                applyPreset(ComboBoxPreset.SelectedValue.ToString());
        }
        private void ComboBoxPreset_Loaded(object sender, RoutedEventArgs e)
        {
            listPresetItem = new BindingList<Preset>() { };
            DirectoryInfo presetDir = new DirectoryInfo(@".\presets");
            FileInfo[] Files = presetDir.GetFiles("*.bin");
            listPresetItem.Add(new Preset() { Name = "(none)", PresetPath = "0" });
            foreach (FileInfo file in Files)
            {
                listPresetItem.Add(new Preset() { Name = file.Name, PresetPath = "./presets/" + file.Name });
            }

            ComboBoxPreset.ItemsSource = listPresetItem;
            ComboBoxPreset.SelectedIndex = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.IO.Directory.CreateDirectory("presets");
        }
    }
}
