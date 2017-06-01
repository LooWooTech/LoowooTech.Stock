using LoowooTech.Stock.Common;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;

namespace LoowooTech.Stock.Rules
{
    public class TableThirteen:TableBase,ITable
    {
        public override string Name
        {
            get
            {
                return "检查 表13 农村其他存量建设用地利用现状调查表基本属性结构表";
            }
        }
        public TableThirteen()
        {
            _tableName = "QTCLJSYD";
            _key = "BSM";
            list.Add(new FieldStructureTool() { TableName = _tableName, ID = "13000(结构规则)" });
            list.Add(new ValueCountTool() { TableName = _tableName, ID = "13001(填写规则)" });
            list.Add(new ValueUniqueTool() { TableName = _tableName, CheckFieldName = _key, ID = "13002(填写规则)", Code = "3302" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2008010600" }, ID = "13003(填写规则)" });
            list.Add(new ValueCurrectTool() { TableName = _tableName, Fields = new string[] { "XZCDM", "XZCMC" }, Split = "/", Values = ExcelManager.XZC.Select(e => string.Format("{0}/{1}", e.XZQDM, e.XZQMC)).ToList(), ID = "13004(逻辑规则)" });
            list.Add(new ValueUniqueTool() { TableName = _tableName, CheckFieldName = "DCBH", WhereFieldName = "XZCDM", ID = "13005(填写规则)",Code="3201" });
            list.Add(new ValueCurrectTool() { TableName = _tableName, Fields = new string[] { "XZCDM", "TBBH" }, Values = DCDYTBManager.List.Select(e => string.Format("{0}/{1}", e.XZCDM, e.TBBH)).ToList(), Split = "/", ID = "13006(逻辑规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "JZLX", Key = _key, Values = new string[] { "1", "2", "3", "4" }, ID = "13007(填写规则)" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "LYZT", Key = _key, Values = new string[] { "1", "2", "3" }, ID = "13008(填写规则)" });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "YXSYMJ" }, Key = _key, WhereCaluse = "LYZT='1'", ID = "13009(逻辑规则)", Is_Nullable = false });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "KZYY", "FQYY" }, Key = _key, WhereCaluse = "LYZT='1'", ID = "13010(逻辑规则)", Is_Nullable = true });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "KZYY" }, Key = _key, WhereCaluse = "LYZT='2'", ID = "13011(逻辑规则)", Is_Nullable = false });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "FQYY" }, Key = _key, WhereCaluse = "LYZT='2'", ID = "13012(逻辑规则)", Is_Nullable = true });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "FQYY" }, Key = _key, WhereCaluse = "LYZT='3'", ID = "13013(逻辑规则)", Is_Nullable = false });
            list.Add(new ValueNullTool() { TableName = _tableName, CheckFields = new string[] { "KZYY" }, Key = _key, WhereCaluse = "LYZT='3'", ID = "13014(逻辑规则)", Is_Nullable = true });
        }

        public override void Check(OleDbConnection connection)
        {
            base.Check(connection);
            var gain = new GainAreaTool() { TableName = _tableName, AreaFields = new string[] { "JSYDMJ" } };
            gain.Gain(connection);
        }
    }
}
