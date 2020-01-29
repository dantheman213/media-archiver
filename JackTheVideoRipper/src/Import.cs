using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JackTheVideoRipper
{
    class Import
    {
        public static List<string> getAllUrlsFromPayload(string s)
        {
            var result = new List<string>();
            foreach (Match item in Regex.Matches(s, @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?"))
            {
                result.Add(item.Value);
            }

            return result;
        }
    }
}
