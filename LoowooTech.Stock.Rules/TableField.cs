using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;

namespace LoowooTech.Stock.Rules
{
    /// <summary>
    /// 检查某张表的字段
    /// </summary>
    public class TableField
    {
        /// <summary>
        /// 获取的字段
        /// </summary>
        private List<Field> _acquireFields { get; set; }
        /// <summary>
        /// 要求的字段
        /// </summary>
        private List<Field> _requireFields { get; set; }
        public string Error { get; set; }

        private string _tableName { get; set; }
        public TableField(string tableName)
        {
            Error = string.Empty;
            _tableName = tableName;
        }

        public void Ready(OleDbConnection connection)
        {
            _requireFields = XmlClass.GetField(_tableName);
            _acquireFields = MdbClass.GetFields(connection, _tableName);
        }

        public void Check(OleDbConnection connection)
        {
            Ready(connection);
            var dict = _acquireFields.ToDictionary(e => e.Name, e => e);
            Field temp = null;
            foreach(var field in _requireFields)
            {
                if (dict.ContainsKey(field.Name))
                {
                    temp = dict[field.Name];
                    if (temp != field)
                    {
                        Error += string.Format("要求字段：{0} 信息不符；", field.Name);
                    }
                }
                else
                {
                    Error += string.Format("要求字段：{0} 不存在；", field.Name);
                }
            }
        }
    }
}
