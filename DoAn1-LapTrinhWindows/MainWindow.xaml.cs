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

      
        private void ClickBrowseButton(object sender, RoutedEventArgs e)
        {
            // result = askFileOrFolder() (Show dialog ask "Which type do you want to rename? (FILE / FOLDER)" 
            // if result = 0 => file => loadFilesFromPath(path)
            // if result = 1 => folder => loadFolderFromPath(path)
            // else => NO => Do nothing
            // (result is a global variable to be used in the Refresh function)
        }

        private void ClickRefreshButton(object sender, RoutedEventArgs e)
        {
            // loadFilesFromPath(path)
            // or
            // loadFolderFromPath(path)
        }
    }
}
