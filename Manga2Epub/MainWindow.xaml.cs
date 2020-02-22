using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Manga2Epub.epubBuild;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Manga2Epub {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        //private TextBlock myTextBlock;

        private string cSelDir {
            get => Properties.Settings.Default.selectDir;
        }

        public MainWindow() {
            InitializeComponent();
            //var myTextBlock = (TextBlock)this.FindName("myTextBlock");
            dirTextBox.Text = cSelDir;
            
        }

        private void browseButton_Click(object sender, RoutedEventArgs e) {
            var dialog = new CommonOpenFileDialog();
            
            var savedSDir = cSelDir;
            dialog.InitialDirectory = savedSDir=="" ? "C:\\Users" : savedSDir;
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                var selectDir = dialog.FileName;
                saveSelectDir(selectDir);
                dirTextBox.Text = selectDir;
            }
        }

        private void DirTextBox_OnKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                dirTextBox.SelectAll();
                saveSelectDir(dirTextBox.Text);
            }
        }
        private void DirTextBox_OnLostFocus(object sender, RoutedEventArgs e) {
            saveSelectDir(dirTextBox.Text);
        }

        private void exit_OnClick(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
        /// <summary>
        /// 保存工作文件夹字符串到settings中
        /// </summary>
        /// <param name="text"></param>
        private void saveSelectDir(string text) {
            Properties.Settings.Default.selectDir = text;
            Properties.Settings.Default.Save();
        }

        private void startButton_Click(object sender, RoutedEventArgs e) {
            var mb = new MainBuild(cSelDir);
            
        }

        
    }
}
