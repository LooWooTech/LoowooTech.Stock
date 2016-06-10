using LoowooTech.Stock.Tool;

namespace LoowooTech.Stock.Rules
{
    public class TableTen:TableBase
    {
        public TableTen()
        {
            _tableName = "GGGL-GGFWSSYD";
            _key = "MBBSM";
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2008020300" }, ID = "10001(填写规则)" });
            list.Add(new ValueUniqueTool() { TableName = _tableName, CheckFieldName = "DCBH", ID = "10002(填写规则)" });
            list.Add(new ValueUniqueTool() { TableName = _tableName, CheckFieldName = "TBBH", ID = "10003(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "TDQDFS", Key = _key, Values = new string[] { "1", "2", "3", "4", "5" }, ID = "10004(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "JZLX", Key = _key, Values = new string[] { "1", "2", "3", "4" }, ID = "10005(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "LYZT", Key = _key, Values = new string[] { "1", "2", "3" }, ID = "10006(填写规则)" });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "YT", "FWRS", "SYMJ" }, Is_Nullable = false, WhereCaluse = "LYZT='1'", ID = "10007(逻辑规则)",Key=_key });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "KZYY", "FQYY" }, Is_Nullable = true, WhereCaluse = "LYZT='1'", ID = "10008(逻辑规则)",Key=_key });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "KZYY" }, Is_Nullable = false, WhereCaluse = "LYZT='2'", ID = "10009(逻辑规则)",Key=_key });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "FQYY", "YT", "FWRS", "SYMJ" }, Is_Nullable = false, WhereCaluse = "LYZT='2'", ID = "10010(逻辑规则)",Key=_key });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "FQYY" }, Is_Nullable = false, WhereCaluse = "LYZT='3'", ID = "10011(逻辑规则)",Key=_key });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "KZYY", "YT", "FWRS", "SYMJ" }, Is_Nullable = true, WhereCaluse = "LYZT='3'", ID = "10012(逻辑规则)",Key=_key });
        }

    }
}
