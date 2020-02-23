using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Manga2Epub.epubBuild {
    class ImageInfo {
        internal string fileName = "";   // 源图片文件名
        internal string filepath = "";   // 源图片文件绝对路径
        internal int width = 0;
        internal int height = 0;
        internal string movedName = "";  // 目标文件夹图片文件名（含扩展名）
        internal string suffix = "";    // 扩展名
        internal string id = "";        // 文件名（不含扩展名）
        
    }

    class ImagesProcessor {
        private string imagesDirPath;
        private string targetDirPath;
        private List<ImageInfo> images;
        private BackgroundWorker bgWorker;

        public ImagesProcessor(string dirPath, string targetDirPath, BackgroundWorker bgWorker) {
            this.imagesDirPath = dirPath;
            this.targetDirPath = targetDirPath;
            this.images = new List<ImageInfo>();
            this.bgWorker = bgWorker;
        }

        /// <summary>
        /// 此函数供epub_builder调用，获取图片信息并拷贝图片到目标文件夹
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="targetDirPath"></param>
        public void proceed() {
            scanImages();
            copyImages();
        }

        /// <summary>
        /// 获取每张图片的信息并写入images类属性中
        /// </summary>
        private void scanImages() {
            var filesPath = Directory.GetFiles(this.imagesDirPath, "*", SearchOption.TopDirectoryOnly);

            var filesLength = filesPath.Length;
            for (int i = 0; i < filesLength; i++ ) {
                var filePath = filesPath[i];
                var fileName = Path.GetFileName(filePath);

                int w, h;
                using (var im = new Bitmap(filePath)) {
                    w = im.Width;
                    h = im.Height;
                }

                var im_info = new ImageInfo();
                im_info.fileName = fileName;
                im_info.filepath = filePath;
                im_info.width = w;
                im_info.height = h;
                im_info.suffix = getSuffix(fileName);

                this.images.Add(im_info);

                bgWorker.ReportProgress( (i+1) * 20 / filesLength);
            }
        }

        /// <summary>
        /// 获取图片文件的扩展名（jpg改为jpeg）
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string getSuffix(string filePath) {
            var suffix = Path.GetExtension(filePath)?.Substring(1);
            if (suffix == "jpg") {
                suffix = "jpeg";
            }
            return suffix;
        }

        /// <summary>
        /// 写入图片信息实例的moved_name和id属性
        /// </summary>
        private void copyImages() {
            var imageCount = this.images.Count;
            for (int i = 0; i < imageCount; i++) {
                var imagePath = this.images[i].filepath;
                string movedName;
                if (i == 0) {   // 第一张图片作为封面，命名为cover.扩展名
                    movedName = $"cover.{this.images[i].suffix}";
                } else {       // 第二张图片命名为i_0000.扩展名，以此类推
                    movedName = "i_" + (i-1).ToString().PadLeft(4, '0') + $".{this.images[i].suffix}";
                }
                var newPath = Path.Combine(this.targetDirPath, movedName);
                this.images[i].movedName = movedName;
                this.images[i].id = Path.GetFileNameWithoutExtension(movedName);

                //File.Copy(imagePath, newPath);
                bgWorker.ReportProgress( 20 + (i + 1) * 20 / imageCount);
            }
        }

        /// <summary>
        /// 获取images array的函数
        /// </summary>
        /// <returns></returns>
        public List<ImageInfo> getImagesList() {
            return this.images;
        }
    }
}
