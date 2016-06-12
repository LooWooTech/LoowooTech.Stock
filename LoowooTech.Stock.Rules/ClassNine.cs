using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class ClassNine:TableBase,ITable
    {
        public override string Name
        {
            get
            {
                return "检查  表9 经营性建设用地要素基本属性结构表";
            }
        }
        public ClassNine()
        {
            _tableName = "JYXJSYD";
            _key = "MBBSM";
            list.Add(new FieldStructureTool() { TableName = _tableName, ID = "09000(结构规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2008020200" }, ID = "09001(填写规则)" });
            list.Add(new ValueUniqueTool() { TableName = _tableName, CheckFieldName = "DCBH", ID = "09002(填写规则)" });
            list.Add(new ValueUniqueTool() { TableName = _tableName, CheckFieldName = "TBBH", ID = "09003(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "TDQDFS", Key = _key, Values = new string[] { "1", "2", "3", "4", "5" }, ID = "09004(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "JZLX", Key = _key, Values = new string[] { "1", "2", "3", "4" }, ID = "09005(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "LYZT", Key = _key, Values = new string[] { "1", "2", "3" }, ID = "09006(填写规则)" });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "YT", "NSSE", "CYRS" }, Key = _key, WhereCaluse = "LYZT='1'", Is_Nullable = false, ID = "09007(逻辑规则)" });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "KZYY", "FQYY" }, Key = _key, WhereCaluse = "LYZT='1'", Is_Nullable = true, ID = "09008(逻辑规则)" });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "KZYY" }, Key = _key, WhereCaluse = "LYZT='2'", Is_Nullable = false, ID = "09009(逻辑规则)" });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "FQYY", "YT", "NSSE", "CYRS" }, Key = _key, WhereCaluse = "LYZT='2'", Is_Nullable = true, ID = "09010(逻辑规则)" });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "FQYY" }, Key = _key, WhereCaluse = "LYZT='3'", Is_Nullable = false, ID = "09011(逻辑规则)" });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "KZYY", "YT", "NSSE", "CYRS" }, Key = _key, WhereCaluse = "LYZT='3'", Is_Nullable = true, ID = "09012(逻辑规则)" });
        }
    }
}
