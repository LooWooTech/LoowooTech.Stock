using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class TableSix
    {
        public TableSix()
        {
            var list = new List<ITool>();
            list.Add(new ValueRangeTool() { TableName = "XZQJX", CheckFieldName = "YSDM", Key = "BSM", Values = new string[] { "1000600200" }, ID = "6001(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = "XZQJX", CheckFieldName = "JXLX", Key = "BSM", Values = new string[] { "250200", "250201", "250202", "250203", "620200", "630200", "640200", "650200", "660200", "670402", "670500" }, ID = "6002(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = "XZQJX", CheckFieldName = "JXXZ", Key = "BSM", Values = new string[] { "600001", "600002", "600003", "600004", "600009" }, ID = "6003(填写规则)" });
        }
    }
}
