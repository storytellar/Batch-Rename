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

        public class FileInfo : INotifyPropertyChanged
        {
            public string name;
            public string Name
            {
                get => name; set
                {
                    name = value;
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
            private string newName;

            public string NewCase(int mode)
            {
                if (mode == 1)
                    return Name.ToLower();
                else if (mode == 2)
                    return Name.ToUpper();
                else
                {
                    string kq = name.ToLower();
                    return kq.First().ToString().ToUpper() + kq.Substring(1);

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
                throw new NotImplementedException();
            }
        }
        BindingList<FileInfo> files = new BindingList<FileInfo>();

        private void ClickBrowseButton(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\";
            dialog.Multiselect = true;

            if (FolderButton.IsChecked == true)
            {
                dialog.IsFolderPicker = true;

                if (files.Count != 0 && Path.GetExtension(files[0].Name) != "")
                    files.Clear();
            }

            if (FileButton.IsChecked == true)
            {
                dialog.IsFolderPicker = false;

                if (files.Count != 0 && Path.GetExtension(files[0].Name) == "")
                    files.Clear();
            }

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                
                foreach (string sFileName in dialog.FileNames)
                {
                    files.Add(new FileInfo { Name = Path.GetFileName(sFileName),NewName="No name" });

                    for (int i = 0; i < files.Count; i++)
                    {
                        for (int j = i + 1; j < files.Count; j++)
                        {
                            if (files[i].Name == files[j].Name)
                                files.Remove(files[j]);
                        }
                    }

                    lv.Items.Clear();
                    foreach (FileInfo file in files)
                    {
                        lv.Items.Add(file);
                    }

                    txtGetFile.Text = Path.GetDirectoryName(sFileName);
                }
            }
        }

        private void ClickRefreshButton(object sender, RoutedEventArgs e)
        {
            files.Clear();
            lv.Items.Clear();
            txtGetFile.Text = "C:\\path...";
        }

        private void ReNameButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(var file in files)
            {
                file.NewName = file.NewCase(1);
            }
        }

        private void Lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
