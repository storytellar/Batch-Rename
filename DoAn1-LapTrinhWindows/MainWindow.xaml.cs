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

            public event PropertyChangedEventHandler PropertyChanged;

            void RaiseEvent([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                    files.Add(new FileInfo { Name = Path.GetFileName(sFileName) });

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
    }
}
