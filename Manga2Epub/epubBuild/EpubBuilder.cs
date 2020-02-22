using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Ionic.Zip;
using Ionic.Zlib;

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
            utils.mkfile(this.cwd, FileTemplates.mimetype);
            // 创建META-INF文件夹
            cd("META-INF");
            Directory.CreateDirectory(this.cwd);
            // 创建container.xml文件
            cd("META-INF", "container.xml");
            utils.mkfile(this.cwd, FileTemplates.containerXml);

            // 创建OEBPS文件夹
            cd("OEBPS");
            Directory.CreateDirectory(this.cwd);

            // 创建OEBPS文件夹中除standard.opf文件、xhtml文件、图片文件以外的所有文件和文件夹
            cd("OEBPS", "navigation-documents.xhtml");
            utils.mkfile(this.cwd, FileTemplates.navigationDocumentsXhtml);
            cd("OEBPS", "image");
            Directory.CreateDirectory(this.cwd);
            cd("OEBPS", "text");
            Directory.CreateDirectory(this.cwd);
            cd("OEBPS", "style");
            Directory.CreateDirectory(this.cwd);
            cd("OEBPS", "style", "fixed-layout-jp.css");
            utils.mkfile(this.cwd, FileTemplates.fixedLayoutJPCss);

        }

        private void makeDetails() {
            // 获取原始文件夹中所有图片的信息，并拷贝图片到目标文件夹
            cd("OEBPS", "image");
            var pro = new ImagesProcessor(this.inputPath, this.cwd);
            pro.proceed();
            var images = pro.getImagesList();

            // 生成xhtml文件，并获取xhtml文件信息array（xhtmls）
            cd("OEBPS", "text");
            var xhtml = new XhtmlGenerator(this.cwd, pro.getImagesList(), this.bookName);
            xhtml.generate();
            var xhtmls = xhtml.getXhtmls();

            // 生成standard.opf文件
            cd("OEBPS", "standard.opf");
            var mfst = new ManifestGenerator(this.cwd, this.bookName, this.author, this.publisher, images, xhtmls);
            mfst.generate();
        }

        private void makeEpub() {
            cd(""); // 返回根文件夹
            var sourceDir = this.cwd; // 根文件夹绝对路径
            var zipFileName =
                Path.GetFullPath(Path.Combine(this.outputRoot, "..", $"{this.bookName}.epub")); // zip压缩包文件完整路径

            // 创建zip文件（扩展名为epub）
            using (var zip = new ZipFile(Encoding.UTF8)) {

                zip.CompressionLevel = CompressionLevel.None;

                zip.AddFile(Path.Combine(this.outputRoot, "mimetype"), "");
                zip.AddDirectory(Path.Combine(this.outputRoot, "META-INF"), "META-INF");
                zip.AddDirectory(Path.Combine(this.outputRoot, "OEBPS"), "OEBPS");
                // 可能会抛出FileNotFoundException
                zip.Save(zipFileName);

            }
        }

        public void build() {
            makeMainStructure();
            makeDetails();
            makeEpub();
        }
    }
}
