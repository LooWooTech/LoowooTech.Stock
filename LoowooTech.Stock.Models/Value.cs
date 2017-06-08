using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Models
{
    public class Value:XZC
    {
        public string DCDYLX { get; set; }
    }

    public class XZC
    {
        public string XZCDM { get; set; }
        public string XZCMC { get; set; }
    }


    public class XZDC:XZC
    {
        public XZC XZQ { get; set; }
    }
}
