using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using LoowooTech.Stock.Common;

namespace LoowooTech.Stock.Rules
{
    public class TableNine:TableBase,ITable
    {
        public override string Name
        {
            get
            {
                return "检查 表9 农村存量建设用地总体情况基本属性结构表";
            }
        }

        public TableNine()
        {
            _tableName = "NCCLJSYDZTQK";
            _key = "BSM";
            list.Add(new FieldStructureTool() { TableName = _tableName, ID = "09000(结构规则)" });
            list.Add(new ValueCountTool() { TableName = _tableName, ID = "09001(填写规则)" });
            list.Add(new ValueUniqueTool() { TableName = _tableName, CheckFieldName = _key, ID = "09002(填写规则)",Code="3302" });
            list.Add(new ValueRangeTool() { TableName = _tableName, CheckFieldName = "YSDM", Key = _key, Values = new string[] { "2008010200" }, ID = "09003(填写规则)" });
            list.Add(new ValueCurrectTool() { TableName = _tableName, Fields = new string[] { "XZCDM", "XZCMC", "SSXZMC" }, Split = "/", Values = ExcelManager.XZDC.Select(e => string.Format("{0}/{1}/{2}", e.XZQDM, e.XZQMC, e.XZQ.XZQMC)).ToList(), ID = "09004(逻辑规则)" });
            list.Add(new ValueCompareTool() { TableName = _tableName, Key = _key, FieldArray1 =new string[]{ "XZCZYDMJ" }, FieldArray2 = new string[] { "DJCZYDMJ"  }, Compare = Compare.Above, ID = "09005(逻辑规则)" });
            list.Add(new ValueCompareTool() { TableName = _tableName, Key = _key, FieldArray1 = new string[] { "XZCZYDMJ" }, FieldArray2 = new string[] { "QQCZYDMJ" }, Compare = Compare.Above, ID = "09006" });
            list.Add(new ValueCompareTool() { TableName = _tableName, Key = _key, FieldArray1 = new string[] {"RKZS","LCRK" }, FieldArray2 = new string[] {"HJRK","LRRK" }, Compare = Compare.Equal, ID = "09007" });

        }
        private bool Logic(int[] array)
        {
            if (array.Length != 4)
            {
                return false;
            }
            return array[0] == array[1] + array[2] - array[3];

        }

        public override void Check(OleDbConnection connection)
        {
            base.Check(connection);
           
        }
    }
}
