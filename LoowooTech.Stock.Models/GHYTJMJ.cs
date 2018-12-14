using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Models
{
    public class GHYTJMJ
    {
        public string BSM { get; set; }
        public double MJ { get; set; }
        public double JMJ { get; set; }

        public List<TKXSEntry> TKXSList { get; set; }

    }
    public class TKXSEntry
    {
        public double TKXS { get; set; }
        public double Area { get; set; }
    }
}
