using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class TBBHRule:ValueBaseRule,IRule
    {
        public override string RuleName
        {
            get
            {
                return "图层内属性一致性";
            }
        }
        public override string ID
        {
            get
            {
                return "5101";
            }
        }
        public override void Init()
        {
            Tools.Add(new ValueCurrectTool() { TableName = "CLZJD", Fields = new string[] { "XZCDM", "TBBH" }, Values = DCDYTBManager.List.Select(e => string.Format("{0}/{1}", e.XZCDM, e.TBBH)).ToList(), Split = "/", ID = "10006" });
            Tools.Add(new ValueCurrectTool() { TableName = "JYXJSYD", Fields = new string[] { "XZCDM", "TBBH" }, Values = DCDYTBManager.List.Select(e => string.Format("{0}/{1}", e.XZCDM, e.TBBH)).ToList(), Split = "/", ID = "11006" });
            Tools.Add(new ValueCurrectTool() { TableName = "GGGL_GGFWSSYD", Fields = new string[] { "XZCDM", "TBBH" }, Values = DCDYTBManager.List.Select(e => string.Format("{0}/{1}", e.XZCDM, e.TBBH)).ToList(), Split = "/", ID = "12006" });
            Tools.Add(new ValueCurrectTool() { TableName = "QTCLJSYD", Fields = new string[] { "XZCDM", "TBBH" }, Values = DCDYTBManager.List.Select(e => string.Format("{0}/{1}", e.XZCDM, e.TBBH)).ToList(), Split = "/", ID = "13006" });
        }
    }
}
