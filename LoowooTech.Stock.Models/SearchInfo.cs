using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Models
{
    public class SearchInfo
    {
        public Field Field { get; set; }
        public string WhereClause { get; set; }
        public string TableName { get; set; }
        public string SQL
        {
            get
            {
                if (string.IsNullOrEmpty(WhereClause))
                {
                    return string.Format("SELECT SUM({0}) FROM {1}", Field.Name, TableName);
                }
                return string.Format("SELECT SUM({0}) FROM {1} WHERE {2}", Field.Name, TableName, WhereClause);
            }
        }
        public object Value { get; set; }
        public string Title
        {
            get
            {
                var val = Value.ToString();
                if (Field.Type == FieldType.Float)
                {
                    var index = val.IndexOf(".");
                    if (index > -1&&val.Length>index+3)
                    {
                        val = val.Substring(0,index+3);
                    }
                  
                }
                return string.Format("字段【{0}({1})】合计值：{2}", Field.Name,Field.Title, val);
            }
        }
    }
}
