using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class F1ExcelRule:FBaseRule
    {
        public override string RuleName { get { return "F1检查"; } }
        public override string ID { get { return "41"; } }
        public override string ExcelName { get { return "F1"; } }
        public override void Check()
        {
            base.Check();

        }
    }
}
