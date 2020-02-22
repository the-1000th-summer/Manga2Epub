using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manga2Epub.epubBuild {
    class FileTemplates {
        public static string mimetype = @"application/epub+zip";

        public static string containerXml = @"<?xml version=""1.0""?>
            <container
             version=""1.0""
             xmlns=""urn:oasis:names:tc:opendocument:xmlns:container""
            >
            <rootfiles>
            <rootfile
             full-path=""OEBPS/standard.opf""
             media-type=""application/oebps-package+xml""
            />
            </rootfiles>
            </container>".StripLeadingWhitespace();

        public static string navigationDocumentsXhtml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
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

        public static string fixedLayoutJPCss = @"@charset ""UTF-8"";
            
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

        public static string xhtmlTemplate = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <!DOCTYPE html>
            <html
             xmlns=""http://www.w3.org/1999/xhtml""
             xmlns:epub=""http://www.idpf.org/2007/ops""
             xml:lang=""ja""
            >
            <head>
            <meta charset=""UTF-8"" />
            <title>{0}</title>
            <link rel=""stylesheet"" type=""text/css"" href=""../style/fixed-layout-jp.css""/>
            <meta name=""viewport"" content=""width={2}, height={3}""/>
            </head>
            <body>
            <div class=""main"">
            <svg xmlns=""http://www.w3.org/2000/svg"" version=""1.1""
             xmlns:xlink=""http://www.w3.org/1999/xlink""
             width=""100%"" height=""100%"" viewBox=""0 0 {2} {3}"">
            <image width=""100%"" height=""100%"" preserveAspectRatio=""xMidYMid meet"" xlink:href=""../image/{1}"" />
            </svg>
            </div>
            </body>
            </html>".StripLeadingWhitespace();

        public static string manifestTemplate = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <package xmlns=""http://www.idpf.org/2007/opf"" version=""3.0"" xml:lang=""ja"" unique-identifier=""unique-id"" prefix=""rendition: http://www.idpf.org/vocab/rendition/#         epub-bundle-tool: https://wing-kai.github.io/epub-manga-creator/         ebpaj: http://www.ebpaj.jp/         fixed-layout-jp: http://www.digital-comic.jp/"">
            
            <metadata xmlns:dc=""http://purl.org/dc/elements/1.1/"">
            
            <!-- 作品名 -->
            <dc:title id=""title"">{0}</dc:title>
            <meta refines=""#title"" property=""file-as""></meta>
            
            <!-- 著者名 -->
            <dc:creator id=""creator01"">{1}</dc:creator>
            <meta refines=""#creator01"" property=""role"" scheme=""marc:relators"">aut</meta>
            <meta refines=""#creator01"" property=""file-as""></meta>
            <meta refines=""#creator01"" property=""display-seq"">0</meta>
            
            <dc:subject>成年コミック</dc:subject>
            
            <!-- 出版社名 -->
            <dc:publisher id=""publisher"">{2}</dc:publisher>
            <meta refines=""#publisher"" property=""file-as""></meta>
            
            <!-- 言語 -->
            <dc:language>ja</dc:language>
            
            <!-- ファイルid -->
            <dc:identifier id=""unique-id"">urn:uuid:{3}</dc:identifier>
            
            <!-- 更新日 -->
            <meta property=""dcterms:modified"">{4}</meta>
            
            <!-- Fixed-Layout Documents指定 -->
            <meta property=""rendition:layout"">pre-paginated</meta>
            <meta property=""rendition:spread"">landscape</meta>
            
            <!-- etc. -->
            <meta property=""ebpaj:guide-version"">1.1</meta>
            <meta name=""SpineColor"" content=""#FFFFFF""></meta>
            <meta name=""cover"" content=""cover""></meta>
            
            </metadata>
            
            <manifest>
            
            <!-- navigation -->
            <item media-type=""application/xhtml+xml"" id=""toc"" href=""navigation-documents.xhtml"" properties=""nav""></item>
            
            <!-- style -->
            <item media-type=""text/css"" id=""fixed-layout-jp"" href=""style/fixed-layout-jp.css""></item>
            
            <!-- image -->
            {5}
            <!-- text -->
            {6}
            </manifest>
            
            <spine page-progression-direction=""rtl"">
            
            {7}
            </spine>
            
            </package>".StripLeadingWhitespace();

        public static string cover_image_item = @"<item id=""cover"" href=""image/cover.{0}"" media-type=""image/{0}"" properties=""cover-image""></item>";
        public static string image_item = @"<item id=""{0}"" href=""image/{1}"" media-type=""image/{2}""></item>";

        public static string cover_xhtml_item = @"<item id=""p-cover"" href=""text/p_cover.xhtml"" media-type=""application/xhtml+xml"" properties=""svg"" fallback=""cover""></item>";
        public static string xhtml_item = @"<item id=""{0}"" href=""text/{1}"" media-type=""application/xhtml+xml"" properties=""svg"" fallback=""{2}""></item>";

        public static string cover_itemref = @"<itemref linear=""yes"" idref=""p-cover"" properties=""rendition:page-spread-center""></itemref>";
        public static string right_itemref = @"<itemref linear=""yes"" idref=""{0}"" properties=""page-spread-right""></itemref>";
        public static string left_itemref = @"<itemref linear=""yes"" idref=""{0}"" properties=""page-spread-left""></itemref>";


    }
}
