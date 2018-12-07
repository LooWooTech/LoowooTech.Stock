using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Tool;

namespace LoowooTech.Stock.Rules
{
    public class JClassRule:ClassBaseTool,Models.IRule
    {
        public override string RuleName { get { return "J类转换逻辑一致性检查"; } }
        public override string ID { get { return "2008"; } }
        public override CheckProject2 CheckProject { get { return CheckProject2.J类转换过程合理性; } }
        public bool Space { get { return true; } }
        public override void Check()
        {
            Tools.Add(new ValueRangeTool2 { TableName = "JQDLTB", Key = "BSM", CheckFieldName = "DLDM", Values = new string[] { "2121", "2122", "2123", "2125", "2126" },WhereClause="ZHLX = 'J'", RelationName = "JQDLTB", ID = "" });
            base.Check();
        }
    }
}
