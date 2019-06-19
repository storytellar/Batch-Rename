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

namespace DoAn1_LapTrinhWindows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

            if (NC.IsChecked == true)
            {
                if (radioUpperAll.IsChecked == true)
                    actions.Add(new ToUpperCase());

                else if (radioLowerAll.IsChecked == true)
                    actions.Add(new ToLowerCase());

                else if (radioUpperFirstOne.IsChecked == true)
                    actions.Add(new SpecialCase());
            }
            
            if (UniqueNameBox.IsChecked==true)
            {
                actions.Add(new UniqueName());
            }

            if (FullNameNormalizeBox.IsChecked == true)
            {
                actions.Add(new FullNameNormalizer());
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


        public string FixRepeatedName(string target, string ext, string[] files)
        {
            int tail = 1;
            bool isRepeated = false;

            foreach(var file in files)
            {
                if (target + ext == file)
                {
                    isRepeated = true;
                    foreach (var f in files)
                        if (target + tail.ToString() + ext == f)
                            tail++;
                }
            }
            if (isRepeated == false)
                return target;
            return target + tail.ToString();
        }

        private void ClickSavePresetButton(object sender, RoutedEventArgs e)
        {
            // save preset
        }

        private void ClickRemovePresetButton(object sender, RoutedEventArgs e)
        {
            // remove preset
        }
    }
}
