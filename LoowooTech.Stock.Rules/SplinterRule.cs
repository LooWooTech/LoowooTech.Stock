using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class SplinterRule:IRule
    {
        public string RuleName { get { return "面层是否存在不符合上图要求的碎片多边形"; } }
        public string ID { get { return "4201"; } }
        public void Check()
        {

        }
    }
}
