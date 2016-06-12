using LoowooTech.Stock.Common;
using System.Collections.Generic;
using System.Data.OleDb;

namespace LoowooTech.Stock.Rules
{
    /// <summary>
    /// 检查数据库中是否存在要求的表
    /// </summary>
    public class TableStructure:ITable
    {
        /// <summary>
        /// 获取的表名
        /// </summary>
        private List<string> _acquireTables { get; set; }
        /// <summary>
        /// 要求的表名
        /// </summary>
        private List<string> _requireTables { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> ExistTables { get; set; }
        public List<string> Erros { get; set; }
        public string Name
        {
            get
            {
                return "检查数据库结构定义";
            }
        }
        public TableStructure()
        {
            Erros = new List<string>();
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
                    ExistTables.Add(table);
                }
                else
                {
                    Erros.Add(string.Format("缺失表：{0}；", table));
                }
            }
        }

    }
}
