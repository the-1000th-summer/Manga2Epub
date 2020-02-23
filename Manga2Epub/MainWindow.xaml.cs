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
using System.IO;
using Manga2Epub.epubBuild;
using Microsoft.WindowsAPICodePack.Dialogs;
using Path = System.IO.Path;

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
            Properties.Settings.Default.isRunning = false;
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
                changeUIAccV(validateDir(dirTextBox.Text));
            }
        }

        /// <summary>
        /// 文件夹textbox失去焦点，执行此方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DirTextBox_OnLostFocus(object sender, RoutedEventArgs e) {
            saveSelectDir(dirTextBox.Text);
            changeUIAccV(validateDir(dirTextBox.Text));
        }

        private bool validateDir(string dir) {
            if (Directory.Exists(dir)) {
                var subDirs = Directory.GetDirectories(dir);
                if (subDirs.Length > 0) {
                    return true;
                } else {
                    if (Directory.GetFiles(dir).Length > 0) {
                        return true;
                    }
                }
            }
            return false;
        }
        private void changeUIAccV(bool vali) {
            if (vali) {
                startBuildButton.IsEnabled = true;
            } else {
                startBuildButton.IsEnabled = false;
                MessageBox.Show("此路径无效。");
            }
        }

        /// <summary>
        /// 按下退出选项，退出程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exit_OnClick(object sender, RoutedEventArgs e) {
            if (Properties.Settings.Default.isRunning) {
                if (MessageBox.Show("退出程序吗？任务仍在执行。", "退出程序吗？", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                    MessageBoxResult.Yes) {
                    Application.Current.Shutdown();
                }
            } else {
                Application.Current.Shutdown();
            }
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
            dirTextBox.IsEnabled = false;
            cancelButton.IsEnabled = true;
            pBarIndicator.Content = "读取图片信息......";
            Properties.Settings.Default.isRunning = true;
            Properties.Settings.Default.Save();
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

            if (bgWorker.CancellationPending) {
                e.Cancel = true;
            }
        }
        /// <summary>
        /// 后台线程执行完毕，执行此方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorker_runCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Cancelled) {
                pBarIndicator.Content = "epub生成中止";
                deleteIntermediate();
                MessageBox.Show("epub生成中止！");
            } else if (e.Error != null) {
                pBarIndicator.Content = "出现未知错误";
                MessageBox.Show("出现未知错误！");
            } else {
                pBarIndicator.Content = "完成";
                MessageBox.Show("生成完毕！");
            }

            Properties.Settings.Default.isRunning = false;
            Properties.Settings.Default.Save();
            startBuildButton.IsEnabled = true;
            browseButton.IsEnabled = true;
            menu_selDir.IsEnabled = true;
            dirTextBox.IsEnabled = true;
            cancelButton.IsEnabled = false;
            pBar.Value = 0;
            pBarAll.Value = 0;
        }

        /// <summary>
        /// 进度条值改变，执行此方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorker_progressChanged(object sender, ProgressChangedEventArgs e) {
            var percent = e.ProgressPercentage;
            if (percent <= 100) {
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
            } else {
                pBarAll.Value = percent - 100;
            }
            
        }

        /// <summary>
        /// 点击取消按钮执行此方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, RoutedEventArgs e) {
            if (MessageBox.Show("中止epub生成吗？", "中止epub生成吗？", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                MessageBoxResult.Yes) {
                bgWorker.CancelAsync();
            }
        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            //exit_OnClick(sender, null);
            if (Properties.Settings.Default.isRunning) {
                if (MessageBox.Show("退出程序吗？任务仍在执行。", "退出程序吗？", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) {
                    e.Cancel = true;
                } else {
                    Application.Current.Shutdown();
                }
            }
            
        }

        /// <summary>
        /// 删除(output)文件夹
        /// </summary>
        private void deleteIntermediate() {
            string[] booksDir;
            if (Properties.Settings.Default.multiBooks) {
                booksDir = Directory.GetDirectories(cSelDir);
            } else {
                booksDir = Directory.GetDirectories(Path.GetFullPath(Path.Combine(cSelDir, "..")));
            }
            foreach (var bookDir in booksDir) {
                if (bookDir.EndsWith("(output)") || bookDir.EndsWith(".tmp")) {
                    DeleteDir.delete(bookDir);
                }
            }
        }
    }
}
