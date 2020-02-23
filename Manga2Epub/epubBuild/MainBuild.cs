using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Windows;
using Ionic.Zip;

namespace Manga2Epub.epubBuild {

    class MainBuild {
        //private string selDir;

        public static void build(string selDir, BackgroundWorker bgWorker) {
            //this.selDir = selDir;

            var subDirs = Directory.GetDirectories(selDir);
            if (subDirs.Length > 0) {       // ### 有子文件夹
                Properties.Settings.Default.multiBooks = true;
                Properties.Settings.Default.Save();
                var subDirCount = subDirs.Length;
                for (int i = 0; i < subDirCount; i++) {
                    build1Book(subDirs[i], bgWorker);
                    bgWorker.ReportProgress(100 + (i+1) * 100 / subDirCount);
                    if (bgWorker.CancellationPending) {
                        return;
                    }
                }
                
            } else {                // 无子文件夹
                Properties.Settings.Default.multiBooks = false;
                Properties.Settings.Default.Save();
                build1Book(selDir, bgWorker);
            }
        }

        private static void build1Book(string dir, BackgroundWorker bgWorker) {
            
            var absDirPath = Path.GetFullPath(dir);
            var parentDirPath = Path.GetFullPath(Path.Combine(absDirPath, ".."));

            //MessageBox.Show(Path.GetFullPath(Path.Combine("E:\\ACG\\folder\\aa (output)", "..", "haha.epub")));
            //utils.mkfile(@"E:\ACG\container.xml", FileTemplates.containerXml);
            //MessageBox.Show("fd");
            //Console.WriteLine("sdfds");
            var b = new EpubBuilder(absDirPath, parentDirPath, bgWorker);
            b.build();

        }

    }
}
