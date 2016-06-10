using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Models
{
    public class CombinationClass
    {
        public List<string> Tables { get; set; }
        public int Count
        {
            get
            {
                return Tables.Count;
            }
        }
        public string GetMessage
        {
            get
            {
                var sb = new StringBuilder(Tables[0]);
                for (var i = 1; i < Count; i++)
                {
                    sb.AppendFormat("、{0}", Tables[i]);
                }
                return sb.ToString();
            }
        }
    }
}
