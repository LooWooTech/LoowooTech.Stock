using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Models
{
    public class DCDYTB:XZC
    {
        public string TBBH { get; set; }
        public string DCDYLX { get; set; }
        public double MJ { get; set; }
    }

    public class TB:XZC
    {
        public string TBBH { get; set; }
        public double MJ { get; set; }

    }
}
