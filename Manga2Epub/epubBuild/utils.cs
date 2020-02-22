using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Manga2Epub.epubBuild {
    class utils {
        public static void mkfile(string filepath, string content) {
            //string[] c = { content };
            //File.WriteAllLines(filepath, c, new UTF8Encoding(false));

            if (filepath == null)
                throw new ArgumentNullException("path");

            using (var stream = File.OpenWrite(filepath))
            using (StreamWriter writer = new StreamWriter(stream)) {
                writer.Write(content.Replace("\r\n", "\n"));
            }
        }
    }
}
