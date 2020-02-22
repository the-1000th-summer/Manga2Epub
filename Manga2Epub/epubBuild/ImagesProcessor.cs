using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;



namespace Manga2Epub.epubBuild {
    class ImageInfo {
        internal string fileName = "";
        internal string filepath = "";
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

        public ImagesProcessor(string dirPath, string targetDirPath) {
            this.imagesDirPath = dirPath;
            this.targetDirPath = targetDirPath;
            this.images = new List<ImageInfo>();
        }

        public void proceed() {
            scanImages();
            copyImages();
        }

        private void scanImages() {
            var filesPath = Directory.GetFiles(this.imagesDirPath, "*", SearchOption.TopDirectoryOnly);
            foreach (var filePath in filesPath) {
                var fileName = Path.GetFileName(filePath);

                int w, h;
                using (var im = new Bitmap(filePath)) {
                    w = im.Height;
                    h = im.Width;
                }

                var im_info = new ImageInfo();
                im_info.fileName = fileName;
                im_info.filepath = filePath;
                im_info.width = w;
                im_info.height = h;
                im_info.suffix = getSuffix(fileName);

                this.images.Add(im_info);
            }
        }

        private string getSuffix(string filePath) {
            var suffix = Path.GetExtension(filePath)?.Substring(1);
            if (suffix == "jpg") {
                suffix = "jpeg";
            }
            return suffix;
        }

        private void copyImages() {
            for (int i = 0; i < this.images.Count; i++) {
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

                File.Copy(imagePath, newPath);
            }
        }

        public List<ImageInfo> getImagesList() {
            return this.images;
        }
    }
}
