using ESRI.ArcGIS.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tools
{
    public static class DisplayExtensions
    {
        public static IRgbColor GetRGBColor(int Red, int Green, int Blue, byte Alpha = 255)
        {
            IRgbColor color = new RgbColorClass();
            color.Red = Red;
            color.Green = Green;
            color.Blue = Blue;
            color.Transparency = Alpha;
            return color;
        }
    }
}
