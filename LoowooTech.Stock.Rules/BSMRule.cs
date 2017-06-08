using LoowooTech.Stock.Models;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class BSMRule:ValueBaseRule,IRule
    {
        public override string RuleName
        {
            get
            {
                return "标识码唯一性检查";
            }
        }
        public override string ID
        {
            get
            {
                return "3302";
            }
        }
        public override void Init()
        {
            Tools.Add(new ValueUniqueTool() { TableName = "XZQ_XZ", CheckFieldName = _key, ID = "05002", Code = "3302" });
            Tools.Add(new ValueUniqueTool() { TableName = "XZQ_XZC", CheckFieldName = _key, ID = "06002", Code = "3302" });
            Tools.Add(new ValueUniqueTool() { TableName = "XZQJX", CheckFieldName = _key, ID = "07002", Code = "3302" });
            Tools.Add(new ValueUniqueTool() { TableName = "DCDYTB", CheckFieldName = _key, ID = "08002", Code = "3302" });
            Tools.Add(new ValueUniqueTool() { TableName = "NCCLJSYDZTQK", CheckFieldName = _key, ID = "09002", Code = "3302" });
            Tools.Add(new ValueUniqueTool() { TableName = "CLZJD", CheckFieldName = _key, ID = "10002", Code = "3302" });
            Tools.Add(new ValueUniqueTool() { TableName = "JYXJSYD", CheckFieldName = _key, ID = "11002", Code = "3302" });
            Tools.Add(new ValueUniqueTool() { TableName = "GGGL_GGFWSSYD", CheckFieldName = _key, ID = "12002", Code = "3302" });
            Tools.Add(new ValueUniqueTool() { TableName = "QTCLJSYD", CheckFieldName = _key, ID = "13002", Code = "3302" });
        }
    }
}
