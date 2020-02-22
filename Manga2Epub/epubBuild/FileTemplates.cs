using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manga2Epub.epubBuild {
    class FileTemplates {
        public static string mimetype = @"application/epub+zip";

        public static string containerXml = @"    <?xml version=""1.0""?>
            <container
            version = ""1.0""
            xmlns=""urn:oasis:names:tc:opendocument:xmlns:container""
            >
            <rootfiles>
            <rootfile
            full-path=""OEBPS/standard.opf""
            media-type=""application/oebps-package+xml""
            />
            </rootfiles>
            </container>".StripLeadingWhitespace();

        public static string navigationDocumentsXhtml = @"    <?xml version=""1.0"" encoding=""UTF-8""?>
            <!DOCTYPE html><html xmlns=""http://www.w3.org/1999/xhtml"" xmlns:epub=""http://www.idpf.org/2007/ops"" xml:lang=""ja"">
            <head>
            <meta charset=""UTF-8""></meta>
            <title>Navigation</title>
            </head>
            <body>
            <nav epub:type=""toc"" id=""toc"">
            <h1>Navigation</h1>
            <ol>
            <li><a href=""text/p_cover.xhtml"">表紙</a></li>
            </ol>
            </nav>
            <nav epub:type=""landmarks"">
            <ol>
            <li><a epub:type=""bodymatter"" href=""text/p_cover.xhtml"">Start of Content</a></li>
            </ol>
            </nav>
            </body>
            </html>".StripLeadingWhitespace();

        public static string fixedLayoutJPCss = @"    @charset ""UTF-8"";

            html,
            body {
                margin:    0;
                padding:   0;
                font-size: 0;
            }
            svg {
                margin:    0;
                padding:   0;
            }".StripLeadingWhitespace();

    }
}
