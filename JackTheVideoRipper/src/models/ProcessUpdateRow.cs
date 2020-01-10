using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JackTheVideoRipper.src.models
{
    class ProcessUpdateRow
    {
        public Process proc { get; set; }
        public ListViewItem item { get; set; }
        public bool paint { get; set; }
    }
}
