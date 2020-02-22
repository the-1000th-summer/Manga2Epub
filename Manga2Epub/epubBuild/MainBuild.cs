using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Ionic.Zip;

namespace Manga2Epub.epubBuild {

    class MainBuild {
        private string selDir;

        public MainBuild(string selDir) {
            this.selDir = selDir;
            build1Book(this.selDir);
        }

        private void build1Book(string dir) {
            
            var absDirPath = Path.GetFullPath(dir);
            var parentDirPath = Path.GetFullPath(Path.Combine(absDirPath, ".."));

            //MessageBox.Show(Path.GetFullPath(Path.Combine("E:\\ACG\\folder\\aa (output)", "..", "haha.epub")));


            //utils.mkfile(@"E:\ACG\container.xml",FileTemplates.containerXml);
            var b = new EpubBuilder(absDirPath, parentDirPath);
            b.build();

        }

       
    }
}
