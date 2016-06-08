using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class TableSeven
    {
        public TableSeven()
        {
            var list = new List<ITool>();
            list.Add(new ValueRangeTool() { TableName = "DCDYTB", CheckFieldName = "YSDM", Key = "MBBSM", Values = new string[] { "2008010100" }, ID = "7001(填写规则)" });
            list.Add(new ValueUniqueTool() { TableName = "DCDYTB", CheckFieldName = "TBBH", ID = "7002(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = "DCDYTB", CheckFieldName = "DCDYLX", Key = "MBBSM", Values = new string[] { "1", "2", "3" }, ID = "7003(填写规则)" });
        }
    }
}
