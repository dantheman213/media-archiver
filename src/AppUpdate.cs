using JackTheVideoRipper.src.models;
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
        private static string assemblyInfoFileUrl = "https://raw.githubusercontent.com/dantheman213/JackTheVideoRipper/master/version";

        public static AppVersionModel checkForNewAppVersion()
        {
            var model = new AppVersionModel();

            try
            {
                var webRequest = WebRequest.Create(assemblyInfoFileUrl);

                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                using (var reader = new StreamReader(content))
                {
                    var serverVersion = reader.ReadToEnd().Replace("\n", "");
                    model.version = serverVersion;
                    
                    var dstVersion = new Version(serverVersion);
                    var localVersion = new Version(Common.getAppVersion());

                    var result = dstVersion.CompareTo(localVersion);
                    if (result > 0)
                    {
                        // Version on the Internet is higher...
                        model.isNewerVersionAvailable = true;
                    }

                    return model;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }
    }
}
