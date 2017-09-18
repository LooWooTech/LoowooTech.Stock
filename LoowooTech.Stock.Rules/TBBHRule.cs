using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    /// <summary>
    /// 作用：验证图斑与行政村一致性 行政村内是否存在该图斑
    /// 
    /// </summary>
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
            Tools.Add(new ValueCurrectTool() { TableName = "CLZJD", Key="BSM", Fields = new string[] { "XZCDM", "TBBH" }, Values = DCDYTBManager.List.Select(e => string.Format("{0}/{1}", e.XZCDM, e.TBBH)).ToList(), Split = "/", ID = "10006" });
            Tools.Add(new ValueCurrectTool() { TableName = "JYXJSYD",Key="BSM", Fields = new string[] { "XZCDM", "TBBH" }, Values = DCDYTBManager.List.Select(e => string.Format("{0}/{1}", e.XZCDM, e.TBBH)).ToList(), Split = "/", ID = "11006" });
            Tools.Add(new ValueCurrectTool() { TableName = "GGGL_GGFWSSYD",Key="BSM", Fields = new string[] { "XZCDM", "TBBH" }, Values = DCDYTBManager.List.Select(e => string.Format("{0}/{1}", e.XZCDM, e.TBBH)).ToList(), Split = "/", ID = "12006" });
            Tools.Add(new ValueCurrectTool() { TableName = "QTCLJSYD",Key="BSM", Fields = new string[] { "XZCDM", "TBBH" }, Values = DCDYTBManager.List.Select(e => string.Format("{0}/{1}", e.XZCDM, e.TBBH)).ToList(), Split = "/", ID = "13006" });
        }
    }
}
