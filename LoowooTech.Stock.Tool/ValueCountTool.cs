using LoowooTech.Stock.Common;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public class ValueCountTool:ValueBaseTool,ITool
    {
        public string WhereClause { get; set; }
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(WhereClause))
                {
                    return string.Format("表：{0}在条件：{1}下数据量不能为空，请核对！", TableName, WhereClause);
                }
                return string.Format("表：{0}数据量不能为空，请核对");
            }

        }

        public bool Check(OleDbConnection connection)
        {
            var obj = ADOSQLHelper.ExecuteScalar(connection, string.IsNullOrEmpty(WhereClause) ? string.Format("Select Count(*) from {0}", TableName) : string.Format("Select Count(*) from {0} where {1}", TableName, WhereClause));
            var count = 0;
            if(int.TryParse(obj.ToString(),out count))
            {
                if (count == 0)
                {
                    var info = string.IsNullOrEmpty(WhereClause) ? string.Format("表：{0}数据量为空", TableName) : string.Format("表：{0}在条件：{1}下数据量为空", TableName, WhereClause);
                    Console.WriteLine(info);
                    QuestionManager.Add(new Models.Question { Code = "2101", Name = Name, Project = Models.CheckProject.图层完整性, TableName = TableName, Description = info });
                }
                return true;
            }
            return false;
        }
    }
}
