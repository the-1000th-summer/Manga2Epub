using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Manga2Epub.epubBuild {
    class XhtmlInfo {
        internal string fileName;
        internal string id;

        public XhtmlInfo() {
            fileName = "";
            id = "";
        }

        public void setId() {
            id = Path.GetFileNameWithoutExtension(fileName);
        }
    }

    class XhtmlGenerator {
        private string targetDirPath;
        private List<ImageInfo> images;
        private string bookName;
        private List<XhtmlInfo> xhtmls;
        private BackgroundWorker bgWorker;

        public XhtmlGenerator(string targetDirPath, List<ImageInfo> images, string bookName, BackgroundWorker bgWorker) {
            this.targetDirPath = targetDirPath;
            this.images = images;
            this.bookName = bookName;
            this.xhtmls = new List<XhtmlInfo>();
            this.bgWorker = bgWorker;
        }

        public void generate() {
            var picNumMiddle = this.images.Count / 2;  // int

            var widthNum = (this.images[picNumMiddle].width + 
                            this.images[picNumMiddle - 1].width +
                            this.images[picNumMiddle - 2].width + 
                            this.images[picNumMiddle + 1].width +
                            this.images[picNumMiddle + 2].width) / 5;
            var heightNum = (this.images[picNumMiddle].height +
                             this.images[picNumMiddle - 1].height +
                             this.images[picNumMiddle - 2].height +
                             this.images[picNumMiddle + 1].height +
                             this.images[picNumMiddle + 2].height) / 5;

            var imagesCount = this.images.Count;
            for (int i = 0; i < imagesCount; i++) {
                string xhtmlName;
                if (i == 0) {
                    xhtmlName = "p_cover.xhtml";
                } else {
                    xhtmlName = "p_" + (i - 1).ToString().PadLeft(4, '0') + ".xhtml";
                }

                var xhtmlPath = Path.Combine(this.targetDirPath, xhtmlName);
                utils.mkfile(xhtmlPath, string.Format(FileTemplates.xhtmlTemplate,
                    this.bookName,
                    this.images[i].movedName,
                    widthNum,
                    heightNum));

                var xhtmlInfo = new XhtmlInfo();
                xhtmlInfo.fileName = xhtmlName;
                xhtmlInfo.setId();
                this.xhtmls.Add(xhtmlInfo);

                bgWorker.ReportProgress(40 + (i + 1) * 20 / imagesCount);
            }
        }

        public List<XhtmlInfo> getXhtmls() {
            return this.xhtmls;
        }

    }
}
