using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bin
{
    public delegate void RenderFramegDelegate(System.Drawing.Bitmap bitmap, EventArgs e);
    public delegate void TrackingDelegate(Rectangle findObject, EventArgs e);
}
