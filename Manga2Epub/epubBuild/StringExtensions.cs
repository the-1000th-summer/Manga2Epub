using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Manga2Epub.epubBuild {
    public static class StringExtensions {
        public static string StripLeadingWhitespace(this string s) {
            var outStr = s;
            Regex r = new Regex(@"^\s{12}", RegexOptions.Multiline);

            //while (r.IsMatch(outStr)) {
            outStr = r.Replace(outStr, string.Empty);
            //}
            return outStr;
        }
    }
}
