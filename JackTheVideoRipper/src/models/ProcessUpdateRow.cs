using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JackTheVideoRipper
{
    class ProcessUpdateRow
    {
        public Process proc { get; set; }
        public ListViewItem item { get; set; }
        public List<string> results { get; set; }
        public int cursor { get; set; }
    }
}
