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
        public override void Init()
        {
            Tools.Add(new ValueCurrectTool() { TableName = "XZQ_XZ", Fields = new string[] { "XZQDM", "XZQMC" }, Split = "/", Values = ExcelManager.XZQ.Select(e => string.Format("{0}/{1}", e.XZQDM, e.XZQMC)).ToList(), ID = "05007", Code = "3301" });
            Tools.Add(new ValueCurrectTool() { TableName = "XZQ_XZC", Fields = new string[] { "XZQDM", "XZQMC" }, Split = "/", Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZQDM, e.XZQMC)).ToList(), ID = "06007", Code = "3301" });
            Tools.Add(new ValueCurrectTool() { TableName = "DCDYTB", Fields = new string[] { "XZCDM", "XZCMC" }, Split = "/", Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZQDM, e.XZQMC)).ToList(), ID = "08006", Code = "3301" });
            Tools.Add(new ValueCurrectTool() { TableName = "NCCLJSYDZTQK", Fields = new string[] { "XZCDM", "XZCMC", "SSXZMC" }, Split = "/", Values = ExcelManager.XZDC.Select(e => string.Format("{0}/{1}/{2}", e.XZQDM, e.XZQMC, e.XZQ.XZQMC)).ToList(), ID = "09004" });
            Tools.Add(new ValueCurrectTool() { TableName = "CLZJD", Fields = new string[] { "XZCDM", "XZCMC" }, Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZQDM, e.XZQMC)).ToList(), Split = "/", ID = "10004" });
            Tools.Add(new ValueCurrectTool() { TableName = "JYXJSYD", Fields = new string[] { "XZCDM", "XZCMC" }, Split = "/", Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZQDM, e.XZQMC)).ToList(), ID = "11004" });
            Tools.Add(new ValueCurrectTool() { TableName = "GGGL_GGFWSSYD", Fields = new string[] { "XZCDM", "XZCMC" }, Split = "/", Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZQDM, e.XZQMC)).ToList(), ID = "12004" });
            Tools.Add(new ValueCurrectTool() { TableName = "QTCLJSYD", Fields = new string[] { "XZCDM", "XZCMC" }, Split = "/", Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZQDM, e.XZQMC)).ToList(), ID = "13004" });
        }
    }
}
