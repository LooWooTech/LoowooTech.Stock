using LoowooTech.Stock.Tool;

namespace LoowooTech.Stock.Rules
{
    public class TableSix:TableBase
    {
        public TableSix()
        {
            _tableName = "XZQJX";
            _key = "BSM";
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "YSDM", Key = _key, Values = new string[] { "1000600200" }, ID = "06001(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "JXLX", Key = _key, Values = new string[] { "250200", "250201", "250202", "250203", "620200", "630200", "640200", "650200", "660200", "670402", "670500" }, ID = "06002(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "JXXZ", Key = _key, Values = new string[] { "600001", "600002", "600003", "600004", "600009" }, ID = "06003(填写规则)" });
        }
    }
}
