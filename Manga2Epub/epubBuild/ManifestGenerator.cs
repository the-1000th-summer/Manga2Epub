using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Manga2Epub.epubBuild {
    class ManifestGenerator {
        private string targetFilepath;
        private string bookName;
        private string author;
        private string publisher;
        private List<ImageInfo> images;
        private List<XhtmlInfo> xhtmls;
        private BackgroundWorker bgWorker;

        public ManifestGenerator(string targetFilepath, string bookName, string author, string publisher, List<ImageInfo> images, List<XhtmlInfo> xhtmls, BackgroundWorker bgWorker) {
            this.targetFilepath = targetFilepath;
            this.bookName = bookName;
            this.author = author;
            this.publisher = publisher;
            this.images = images;
            this.xhtmls = xhtmls;
            this.bgWorker = bgWorker;
        }

        public void generate() {
            string imageItems, xhtmlItems, itemrefs;
            genFileManifest(out imageItems, out xhtmlItems, out itemrefs);
            utils.mkfile(this.targetFilepath, string.Format(FileTemplates.manifestTemplate,
                this.bookName,
                this.author,
                this.publisher,
                Guid.NewGuid().ToString(),
                //"5b021173-d0fa-4a5e-9729-2cdd8ccdb259",
                DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"),
                //"2020-02-23T20:22:19Z",
                imageItems,
                xhtmlItems,
                itemrefs
            ));
        }

        private void genFileManifest(out string image_items, out string xhtml_items, out string itemrefs) {

            // Images
            image_items = "";
            for (int i = 0; i < this.images.Count; i++) {
                string image_item;
                if (i == 0) {
                    image_item = string.Format(FileTemplates.cover_image_item, this.images[i].suffix) + "\n";
                } else {
                    image_item = string.Format(FileTemplates.image_item,
                        this.images[i].id,
                        this.images[i].movedName,
                        this.images[i].suffix) + "\n";
                }
                image_items += image_item;
            }

            bgWorker.ReportProgress(63);

            // xhtmls
            xhtml_items = "";
            for (int i = 0; i < this.xhtmls.Count; i++) {
                string xhtml_item;
                if (i == 0) {
                    xhtml_item = FileTemplates.cover_xhtml_item + "\n\n";
                } else {
                    xhtml_item = string.Format(FileTemplates.xhtml_item,
                        this.xhtmls[i].id,
                        this.xhtmls[i].fileName,
                        this.images[i].id) + "\n";
                }
                xhtml_items += xhtml_item;
            }
            bgWorker.ReportProgress(66);

            // Itemrefs
            itemrefs = "";
            for (int i = 0; i < this.images.Count; i++) {
                string itemref;
                if (i == 0) {
                    itemref = FileTemplates.cover_itemref + "\n\n";
                } else {
                    if (i % 2 == 1) {
                        itemref = string.Format(FileTemplates.right_itemref, this.xhtmls[i].id) + "\n";
                    } else {
                        itemref = string.Format(FileTemplates.left_itemref, this.xhtmls[i].id) + "\n";
                    }
                }
                itemrefs += itemref;
            }
            bgWorker.ReportProgress(70);
        }

    }

}
