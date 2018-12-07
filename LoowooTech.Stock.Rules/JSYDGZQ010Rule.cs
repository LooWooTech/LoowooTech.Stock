using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class JSYDGZQ010Rule:ArcGISBaseTool,Models.IRule
    {
        public override string RuleName { get { return "允许建设区一致性检查"; } }
        public override string ID { get { return "6007"; } }
        public override CheckProject2 CheckProject { get { return CheckProject2.村规划允许建设区不超过乡规划范围; } }
        public bool Space { get { return true; } }
        public void Check()
        {
            if (System.IO.File.Exists(ParameterManager2.XGH) == false)
            {
                return;
            }

            if(ExtractJSYDGZQ("GZQLXDM = '010'") == false)
            {

            }
        }
    }
}
