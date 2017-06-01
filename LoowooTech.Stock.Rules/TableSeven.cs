using LoowooTech.Stock.Tool;

namespace LoowooTech.Stock.Rules
{
    public class TableSeven:TableBase
    {
        public override string Name
        {
            get
            {
                return "检查 表7 行政区界线要素基本属性结构表";
            }
        }
        public TableSeven()
        {
            _tableName = "XZQJX";
            _key = "BSM";
            list.Add(new FieldStructureTool() { TableName = _tableName, ID = "07000(结构规则)" });
            list.Add(new ValueCountTool() { TableName = _tableName, ID = "07001(填写规则)" });
            list.Add(new ValueUniqueTool() { TableName = _tableName, CheckFieldName = _key, ID = "07002(填写规则)", Code = "3302" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "YSDM", Key = _key, Values = new string[] { "1000600200","1000600220","1000600230","1000600240","1000600250","1000600260" }, ID = "07003(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "JXLX", Key = _key, Values = new string[] { "630200", "640200", "650200", "660200", "670500" }, ID = "07004(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "JXXZ", Key = _key, Values = new string[] { "600001", "600002", "600003", "600004", "600009" }, ID = "07005(填写规则)" });
        }
    }
}
