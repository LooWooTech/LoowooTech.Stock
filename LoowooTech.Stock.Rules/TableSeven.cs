using LoowooTech.Stock.Tool;

namespace LoowooTech.Stock.Rules
{
    public class TableSeven:TableBase
    {
        public override string Name
        {
            get
            {
                return "检查 表7 调查单元图斑要素基本属性结构表";
            }
        }
        public TableSeven()
        {
            _tableName = "DCDYTB";
            _key = "MBBSM";
            list.Add(new FieldStructureTool() { TableName = _tableName, ID = "07000(结构规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2008010100" }, ID = "07001(填写规则)" });
            list.Add(new ValueUniqueTool() { TableName = _tableName, CheckFieldName = "TBBH", ID = "07002(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "DCDYLX", Key = _key, Values = new string[] { "1", "2", "3" }, ID = "07003(填写规则)" });
        }
    }
}
