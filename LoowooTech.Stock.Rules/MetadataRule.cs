using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class MetadataRule:IRule
    {
        public string RuleName { get { return "元数据"; } }
        public string ID { get { return "1102"; } }
        public bool Space { get { return false; } }

        public void Check()
        {

        }
    }
}
