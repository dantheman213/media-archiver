using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JackTheVideoRipper
{
    class AppUpdate
    {
        private static string assemblyInfoFileUrl = "https://raw.githubusercontent.com/dantheman213/JackTheVideoRipper/master/JackTheVideoRipper/Properties/AssemblyInfo.cs";

        public static string checkForNewAppVersion()
        {
            try
            {
                var webRequest = WebRequest.Create(assemblyInfoFileUrl);

                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                using (var reader = new StreamReader(content))
                {
                    var str = reader.ReadToEnd();
                    int i = str.IndexOf("[assembly: AssemblyFileVersion");
                    if (i > -1)
                    {
                        string serverVersion = String.Format("v{0}", str.Substring(str.IndexOf('"', i) + 1, 5));
                        if (Common.getAppVersion() != serverVersion)
                        {
                            return serverVersion;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

            return "";
        }
    }
}
