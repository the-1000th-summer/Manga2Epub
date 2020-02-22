using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manga2Epub.epubBuild {
    class ImageInfo {
        private string fileName = "";
        private string filepath = "";
        private int width = 0;
        private int height = 0;
        private string movedName = "";  // 目标文件夹图片文件名（含扩展名）
        private string suffix = "";    // 扩展名
        private string id = "";        // 文件名（不含扩展名）
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

        private void proceed() {
            scanImages();
            copyImages();
        }

        private void scanImages() {

        }

        private string getSuffix(string filePath) {

        }

        private void copyImages() {

        }

    }
}
