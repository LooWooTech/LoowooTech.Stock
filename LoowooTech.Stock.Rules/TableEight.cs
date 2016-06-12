using LoowooTech.Stock.Tool;

namespace LoowooTech.Stock.Rules
{
    public class TableEight:TableBase,ITable
    {
        public override string Name
        {
            get
            {
                return "检查 表8 存量宅基地要素基本属性结构表";
            }
        }
        public TableEight()
        {
            _tableName = "CLZJD";
            _key = "MBBSM";
            list.Add(new FieldStructureTool() { TableName = _tableName, ID = "08000(结构规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2008010200" }, ID = "08001(填写规则)" });
            list.Add(new ValueUniqueTool() { TableName = _tableName, CheckFieldName = "DCBH", ID = "08002(填写规则)" });
            list.Add(new ValueUniqueTool() { TableName = _tableName, CheckFieldName = "TBBH", ID = "08003(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "QSLY", Key = _key, Values = new string[] { "1", "2", "3", "4" }, ID = "08004(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "JZLX", Key = _key, Values = new string[] { "1", "2", "3", "4" }, ID = "08005(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "LYZT", Key = _key, Values = new string[] { "1", "2", "3" }, ID = "08006(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "NHLX", Key = _key, Values = new string[] { "1", "2", "3", "4" }, ID = "08007(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "KZYY", Key = _key, Values = new string[] { "1", "2", "3", "4", "5", "" }, ID = "08008(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "FQYY", Key = _key, Values = new string[] { "1", "2", "3", "4", "" }, ID = "08009(填写规则)" });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "RJJZYS" }, Key = _key, WhereCaluse = "LYZT='1'", Is_Nullable = false, ID = "08010(逻辑规则)" });//必填
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "KZYY", "FQYY", "QTYY" },Key=_key, WhereCaluse = "LYZT='1'", Is_Nullable = true, ID = "08011(逻辑规则)" });//为空
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "KZYY" }, Key=_key ,WhereCaluse = "LYZT='2'", Is_Nullable = false, ID = "08012(逻辑规则)" });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "RJJZYS", "FQYY" }, Key=_key, WhereCaluse = "LYZT='2'", Is_Nullable = true, ID = "08013(逻辑规则)" });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "FQYY" },Key=_key, WhereCaluse = "LJZT='3'", Is_Nullable = false, ID = "08014(逻辑规则)" });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "RJJZYS" },Key=_key, WhereCaluse = "LJZT='3'", Is_Nullable = true, ID = "08015(逻辑规则)" });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "QTYY" },Key=_key, WhereCaluse = "KZYY='5'||FQYY='4'", Is_Nullable = false, ID = "08016(逻辑规则)" });
        }
    }
}
