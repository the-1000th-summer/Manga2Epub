using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private BackgroundWorker bgWorker;
        private string cSelDir {
            get => Properties.Settings.Default.selectDir;
        }

        public MainWindow() {
            InitializeComponent();
            //var myTextBlock = (TextBlock)this.FindName("myTextBlock");
            dirTextBox.Text = cSelDir;
            bgWorker = (BackgroundWorker)this.FindResource("backgroundWorker");

        }

        /// <summary>
        /// 点击浏览文件夹按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void browseButton_Click(object sender, RoutedEventArgs e) {
            var dialog = new CommonOpenFileDialog();

            var savedSDir = cSelDir;
            dialog.InitialDirectory = savedSDir == "" ? "C:\\Users" : savedSDir;
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                var selectDir = dialog.FileName;
                saveSelectDir(selectDir);
                dirTextBox.Text = selectDir;
            }
        }

        /// <summary>
        /// 文件夹textbox有焦点，并按下enter键，全选textbox并保存路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DirTextBox_OnKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                dirTextBox.SelectAll();
                saveSelectDir(dirTextBox.Text);
            }
        }

        /// <summary>
        /// 文件夹textbox失去焦点，执行此方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DirTextBox_OnLostFocus(object sender, RoutedEventArgs e) {
            saveSelectDir(dirTextBox.Text);
        }

        /// <summary>
        /// 按下退出选项，退出程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// 按下开始按钮，执行此方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startButton_Click(object sender, RoutedEventArgs e) {
            startBuildButton.IsEnabled = false;
            browseButton.IsEnabled = false;
            menu_selDir.IsEnabled = false;
            pBarIndicator.Content = "读取图片信息......";
            //var a = "sdf/ds-sd".Split('/');
            //foreach (var i in a) {
            //    Console.WriteLine("__"+i+"__");
            //}
            bgWorker.RunWorkerAsync(cSelDir);

        }
        /// <summary>
        /// 分配到后台线程，从执行此方法开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorker_DoWork(object sender, DoWorkEventArgs e) {
            string dirInput = (string) e.Argument;
            MainBuild.build(dirInput, bgWorker);
        }
        /// <summary>
        /// 后台线程执行完毕，执行此方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorker_runCompleted(object sender, RunWorkerCompletedEventArgs e) {
            startBuildButton.IsEnabled = true;
            browseButton.IsEnabled = true;
            menu_selDir.IsEnabled = true;
            pBar.Value = 0;
            pBarIndicator.Content = "";
            MessageBox.Show("Completed!!!");
        }
        /// <summary>
        /// 进度条值改变，执行此方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorker_progressChanged(object sender, ProgressChangedEventArgs e) {
            var percent = e.ProgressPercentage;
            switch (percent) {
                case 40:
                    pBarIndicator.Content = "创建xhtml文件......";
                    break;
                case 60:
                    pBarIndicator.Content = "创建.opf文件.......";
                    break;
                case 70:
                    pBarIndicator.Content = "写入图片......";
                    break;
                
            }

            pBar.Value = percent;
        }
    }
}
