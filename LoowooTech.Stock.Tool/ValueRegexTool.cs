using LoowooTech.Stock.Common;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public class ValueRegexTool:ValueBaseTool2,IVTool
    {
        public string Name
        {
            get
            {
                return string.IsNullOrEmpty(WhereClause) ?
                    string.Format("规则{0}：表【{1}】中字段【{2}】应满足【{3}】", ID, TableName, CheckFiledName, RegexString)
                    : string.Format("规则{0}：当【{1}】时，表【{2}】中字段【{3}】应满足【{4}】", ID, WhereClause, TableName, CheckFiledName, RegexString);
            }
        }

        public string CheckFiledName { get; set; }
        public string RegexString { get; set; }
        public string WhereClause { get; set; }

        private string _SQL
        {
            get
            {
                return string.IsNullOrEmpty(WhereClause) ?
                    string.Format("SELECT {0},{1} FROM {2}", Key, CheckFiledName, TableName)
                    : string.Format("SELECT {0},{1} FROM {2} WHERE {3}", Key, CheckFiledName, TableName, WhereClause);
            }
        }

        public bool Check(OleDbConnection connection)
        {
            var reader = ADOSQLHelper.ExecuteReader(connection, _SQL);
            if (reader == null)
            {
                return false;
            }
            while (reader.Read())
            {

            }
            return true;
        }
    }
}
