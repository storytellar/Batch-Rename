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

        private void Get_File(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            //filedDialog.Filter
            Nullable<bool> dialogOK = fileDialog.ShowDialog();
            if (dialogOK == true)
            {
                foreach (string sFileName in fileDialog.FileNames)
                {
                    files.Add(new FileInfo { Name = Path.GetFileName(sFileName) });

                    for(int i = 0; i < files.Count; i++)
                    {
                        for(int j = i + 1; j < files.Count; j++)
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

        private void Refresh(object sender, RoutedEventArgs e)
        {
            files.Clear();
            lv.Items.Clear();

            txtGetFile.Text = "C:\\path...";
        }

    }
}
