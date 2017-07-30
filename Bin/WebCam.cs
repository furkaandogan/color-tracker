using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bin
{
    public class WebCam
    {
        public WebCam(string name, string monikerString)
        {
            Name = name;
            MonikerString = monikerString;
        }
        public string Name { get; set; }
        public string MonikerString { get; set; }

    }
}
