using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Manga2Epub.epubBuild {
    public static class StringExtensions {
        public static string StripLeadingWhitespace(this string s) {
            Regex r = new Regex(@"^\s+", RegexOptions.Multiline);
            return r.Replace(s, string.Empty);
        }
    }
}
