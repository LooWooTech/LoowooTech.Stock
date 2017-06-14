using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class ContinuousRule:IRule
    {
        public string RuleName { get { return "面层单个图斑要素的空间不连续"; } }
        public string ID { get { return "4301"; } }
        public bool Space { get { return false; } }
        public void Check()
        {

        }

    }
}
