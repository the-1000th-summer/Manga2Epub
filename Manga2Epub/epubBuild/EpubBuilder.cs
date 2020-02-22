using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace Manga2Epub.epubBuild {
    class EpubBuilder {
        private string inputPath = "";
        private string author = "";
        private string bookName = "";
        private string publisher = "";
        private string outputRoot = "";
        private string cwd = "";

        public EpubBuilder(string inputPath, string outputPath) {
            this.inputPath = inputPath;
            this.identify();
            this.outputRoot = Path.Combine(outputPath, this.bookName + "(output)");
            //MessageBox.Show("fd" + outputRoot);
            this.cwd = this.outputRoot;
        }

        private void identify() {
            var dirName = Regex.Match(this.inputPath,@"(?<=[\\/])[^\\/]+$").Groups[0].Value;
            dirName = dirName.Replace(" ", "");
            MessageBox.Show(dirName);

            
            var bookRe = new Regex(@"(?<=[\]\)])[^\[\]\(\)]+");
            this.bookName = bookRe.IsMatch(dirName) ? bookRe.Match(dirName).Groups[0].Value : dirName;

            if (true) {
                var authorRegex = Regex.Matches(dirName, @"(?<=\[)[^\[\]]+(?=\])");
                if (authorRegex.Count != 0) {
                    if (authorRegex[0].Value.EndsWith("?�}��??????")) {
                        this.author = authorRegex[1].Value;
                    } else {
                        this.author = authorRegex[0].Value;
                    }
                } else {
                    this.author = "";
                }
            } else {
                
            }

            if (true) {
                this.publisher = "";
            } else {
                
            }

        }

        private void cd(params string[] dest) {
            string[] cDest = new string[dest.Length + 1];
            cDest[0] = this.outputRoot;
            dest.CopyTo(cDest, 1);
            this.cwd = Path.Combine(cDest);
        }

        private void makeMainStructure() {
            Directory.CreateDirectory(this.cwd);
            // 创建mimetype文件
            cd("mimetype");
            mkfile(this.cwd, FileTemplates.mimetype);
            // 创建META-INF文件夹
            cd("META-INF");
            Directory.CreateDirectory(this.cwd);
            // 创建container.xml文件
            cd("META-INF", "container.xml");
            mkfile(this.cwd, FileTemplates.containerXml);

            // 创建OEBPS文件夹
            cd("OEBPS");
            Directory.CreateDirectory(this.cwd);

            // 创建OEBPS文件夹中除standard.opf文件、xhtml文件、图片文件以外的所有文件和文件夹
            cd("OEBPS", "navigation-documents.xhtml");
            mkfile(this.cwd, FileTemplates.navigationDocumentsXhtml);
            cd("OEBPS", "image");
            Directory.CreateDirectory(this.cwd);
            cd("OEBPS", "text");
            Directory.CreateDirectory(this.cwd);
            cd("OEBPS", "style");
            Directory.CreateDirectory(this.cwd);
            cd("OEBPS", "style", "fixed-layout-jp.css");
            mkfile(this.cwd, FileTemplates.fixedLayoutJPCss);

        }

        private void makeDetails() {
            // 获取原始文件夹中所有图片的信息，并拷贝图片到目标文件夹
            cd("OEBPS", "image");

        }

        private void makeEpub() {

        }

        public void build() {
            makeMainStructure();
            makeDetails();
            makeEpub();
        }

        private void mkfile(string filepath, string content) {
            string[] c = {content};
            File.WriteAllLines(filepath, c, Encoding.UTF8);
        }
    }
}
