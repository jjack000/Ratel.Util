using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ratel.Util.Templates
{
    /// <summary>
    /// 레시피 용으로 간단하게 만든 Rect
    /// </summary>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    /// <param name="Width"></param>
    /// <param name="Height"></param>
    public record struct RRect(int X, int Y, int Width, int Height)
    {
        public RRect(SD.Rectangle rect) : this(rect.X, rect.Y, rect.Width, rect.Height)
        { }

        public RRect(OpenCvSharp.Rect rect) : this(rect.X, rect.Y, rect.Width, rect.Height)
        { }


        public SD.Rectangle ToRect()
        {
            return new(X, Y, Width, Height);
        }

        public OpenCvSharp.Rect ToCvRect()
        {
            return new(X, Y, Width, Height);
        }
    }
}
