using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class XZCDMRule:ValueBaseRule,IRule
    {
        public override string RuleName
        {
            get
            {
                return "行政区编码一致性检查";
            }
        }
        public override string ToString()
        {
            return "3301";
        }
        public override void Init()
        {
            Tools.Add(new ValueCurrectTool() { TableName = "XZQ_XZ", Fields = new string[] { "XZQDM", "XZQMC" }, Split = "/", Values = ExcelManager.XZQ.Select(e => string.Format("{0}/{1}", e.XZCDM, e.XZCMC)).ToList(), ID = "05007", Code = "3301" });
            Tools.Add(new ValueCurrectTool() { TableName = "XZQ_XZC", Fields = new string[] { "XZCDM", "XZCMC" }, Split = "/", Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZCDM, e.XZCMC)).ToList(), ID = "06007", Code = "3301" });
            Tools.Add(new ValueCurrectTool() { TableName = "DCDYTB", Fields = new string[] { "XZCDM", "XZCMC" }, Split = "/", Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZCDM, e.XZCMC)).ToList(), ID = "08006", Code = "3301" });
            Tools.Add(new ValueCurrectTool() { TableName = "NCCLJSYDZTQK", Fields = new string[] { "XZCDM", "XZCMC", "SSXZMC" }, Split = "/", Values = ExcelManager.XZDC.Select(e => string.Format("{0}/{1}/{2}", e.XZCDM, e.XZCMC, e.XZQ.XZCMC)).ToList(), ID = "09004" });
            Tools.Add(new ValueCurrectTool() { TableName = "CLZJD", Fields = new string[] { "XZCDM", "XZCMC" }, Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZCDM, e.XZCMC)).ToList(), Split = "/", ID = "10004" });
            Tools.Add(new ValueCurrectTool() { TableName = "JYXJSYD", Fields = new string[] { "XZCDM", "XZCMC" }, Split = "/", Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZCDM, e.XZCMC)).ToList(), ID = "11004" });
            Tools.Add(new ValueCurrectTool() { TableName = "GGGL_GGFWSSYD", Fields = new string[] { "XZCDM", "XZCMC" }, Split = "/", Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZCDM, e.XZCMC)).ToList(), ID = "12004" });
            Tools.Add(new ValueCurrectTool() { TableName = "QTCLJSYD", Fields = new string[] { "XZCDM", "XZCMC" }, Split = "/", Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZCDM, e.XZCMC)).ToList(), ID = "13004" });
        }
    }
}
