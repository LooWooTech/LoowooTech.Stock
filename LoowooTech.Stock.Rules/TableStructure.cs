using LoowooTech.Stock.Common;
using System.Collections.Generic;
using System.Data.OleDb;

namespace LoowooTech.Stock.Rules
{
    /// <summary>
    /// 检查数据库中是否存在要求的表
    /// </summary>
    public class TableStructure
    {
        /// <summary>
        /// 获取的表名
        /// </summary>
        private List<string> _acquireTables { get; set; }
        /// <summary>
        /// 要求的表名
        /// </summary>
        private List<string> _requireTables { get; set; }
        public string Error { get; set; }
        public TableStructure()
        {
            Error = string.Empty;
        }

        public void Ready(OleDbConnection connection)
        {
            _requireTables = XmlClass.GetRequireTables();
            _acquireTables = MdbClass.GetTables(connection);
        }

        public void Check(OleDbConnection connection)
        {
            Ready(connection);
            foreach(var table in _requireTables)
            {
                if (_acquireTables.Contains(table))
                {

                }
                else
                {
                    Error += string.Format("缺失表：{0}；", table);
                }
            }
        }

    }
}
