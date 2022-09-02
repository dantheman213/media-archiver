using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackTheVideoRipper.src.models
{
    internal class AppVersionModel
    {
        public string version { get; set; }
        public bool isNewerVersionAvailable { get; set; }
    }
}
