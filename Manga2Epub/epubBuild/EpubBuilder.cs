using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private List<ImageInfo> images;
        private BackgroundWorker bgWorker;

        public EpubBuilder(string inputPath, string outputPath, BackgroundWorker bgWorker) {
            images = new List<ImageInfo>();
            this.inputPath = inputPath;
            this.identify();
            this.outputRoot = Path.Combine(outputPath, this.bookName + "(output)");
            //MessageBox.Show("fd" + outputRoot);
            this.cwd = this.outputRoot;
            this.bgWorker = bgWorker;
        }

        public void build() {
            makeMainStructure();
            makeDetails();
            makeEpub(bgWorker);
            bgWorker.ReportProgress(100);
        }

        /// <summary>
        /// 处理原始文件夹名称的函数，处理的结果写入
        /// author, book_name, publisher属性中
        /// </summary>
        private void identify() {
            //var dirName = Regex.Match(this.inputPath, @"(?<=[\\/])[^\\/]+$").Groups[0].Value;
            var dirName = Path.GetFileNameWithoutExtension(this.inputPath);
            dirName = dirName.Replace(" ", "");
            //MessageBox.Show(dirName);

            // 书名识别
            var bookRe = new Regex(@"(?<=[\]\)])[^\[\]\(\)]+");
            this.bookName = bookRe.IsMatch(dirName) ? bookRe.Match(dirName).Groups[0].Value : dirName;

            // 作者名识别
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

        /// <summary>
        /// 改变cwd属性的函数，相当于起一个cd的作用
        /// </summary>
        /// <param name="dest"></param>
        private void cd(params string[] dest) {
            string[] cDest = new string[dest.Length + 1];
            cDest[0] = this.outputRoot;
            dest.CopyTo(cDest, 1);
            this.cwd = Path.Combine(cDest);
        }

        /// <summary>
        /// 此函数构造epub路径的框架，创建几个所需的文件夹
        /// </summary>
        private void makeMainStructure() {
            // 创建文件夹
            string[] folderDirs = {"", "META-INF", "OEBPS", "OEBPS/image", "OEBPS/text", "OEBPS/style" };
            foreach (var folderDir in folderDirs) {
                createFolder(folderDir);
            }

            // 创建文件
            string[] fileNames = {"mimetype", "META-INF/container.xml", "OEBPS/navigation-documents.xhtml", "OEBPS/style/fixed-layout-jp.css"};
            string[] templates = {FileTemplates.mimetype, FileTemplates.containerXml, FileTemplates.navigationDocumentsXhtml, FileTemplates.fixedLayoutJPCss};
            for (int i = 0; i < 4; i++) {
                createFiles(fileNames[i], templates[i]);
            }
        }

        private void createFolder(string dirName) {
            cd(dirName.Split('/'));
            Directory.CreateDirectory(this.cwd);
        }
        private void createFiles(string fileName, string tempStr) {
            cd(fileName.Split('/'));
            utils.mkfile(this.cwd, tempStr);
        }

        /// <summary>
        /// 向各文件夹中写入剩下未写入的文件
        /// </summary>
        private void makeDetails() {
            // 获取原始文件夹中所有图片的信息，并拷贝图片到目标文件夹
            cd("OEBPS", "image");
            var pro = new ImagesProcessor(this.inputPath, this.cwd, bgWorker);
            pro.proceed();
            this.images = pro.getImagesList();

            // 生成xhtml文件，并获取xhtml文件信息array（xhtmls）
            cd("OEBPS", "text");
            var xhtml = new XhtmlGenerator(this.cwd, pro.getImagesList(), this.bookName, bgWorker);
            xhtml.generate();
            var xhtmls = xhtml.getXhtmls();
            
            // 生成standard.opf文件
            cd("OEBPS", "standard.opf");
            var mfst = new ManifestGenerator(this.cwd, this.bookName, this.author, this.publisher, this.images, xhtmls, bgWorker);
            mfst.generate();
        }

        /// <summary>
        /// 将output文件夹压缩为一个epub文件
        /// </summary>
        private void makeEpub(BackgroundWorker bgWorker) {
            cd(""); // 返回根文件夹
            var sourceDir = this.cwd; // 根文件夹绝对路径
            var zipFileName =
                Path.GetFullPath(Path.Combine(this.outputRoot, "..", $"{this.bookName}.epub")); // zip压缩包文件完整路径

            // 创建zip文件（扩展名为.epub）
            using (var zip = new ZipFile(Encoding.UTF8)) {

                zip.CompressionLevel = CompressionLevel.BestCompression;

                zip.AddFile(Path.Combine(this.outputRoot, "mimetype"), "");
                zip.AddDirectory(Path.Combine(this.outputRoot, "META-INF"), "META-INF");
                zip.AddDirectory(Path.Combine(this.outputRoot, "OEBPS"), "OEBPS");

                foreach (var img in this.images) {
                    zip.AddFile(img.filepath).FileName = "OEBPS/image/" + img.movedName;
                }

                // 可能会抛出FileNotFoundException
                zip.Save(zipFileName);
            }

            deleteDir(sourceDir);
        }


        /// <summary>
        /// 删除文件夹及其所有内容的函数
        /// </summary>
        /// <param name="file"></param>
        private void deleteDir(string file) {
            try {
                //去除文件夹和子文件的只读属性
                //去除文件夹的只读属性
                System.IO.DirectoryInfo fileInfo = new DirectoryInfo(file);
                fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;

                //去除文件的只读属性
                System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);

                //判断文件夹是否还存在
                if (Directory.Exists(file)) {
                    foreach (string f in Directory.GetFileSystemEntries(file)) {
                        if (File.Exists(f)) {
                            //如果有子文件删除文件
                            File.Delete(f);
                            //Console.WriteLine(f);
                        } else {
                            //循环递归删除子文件夹
                            deleteDir(f);
                        }
                    }
                    //删除空文件夹
                    Directory.Delete(file);
                    //Console.WriteLine(file);
                }
            } catch (Exception ex) {    // 异常处理
                Console.WriteLine(ex.Message.ToString());// 异常信息
            }
        }
    }
}
