using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class ExcelBase
    {
        /// <summary>
        /// 表格名字
        /// </summary>
        public string ExcelName { get; set; }
        /// <summary>
        /// 数据库表名
        /// </summary>
        private string _tableName { get; set; }
        public string TableName { get { return string.IsNullOrEmpty(_tableName) ? _tableName = XmlManager.GetSingle(string.Format("/Tables/Excel[@Name='{0}']", ExcelName), "TableName", XmlEnum.Field) : _tableName; } }
        public ExcelBase()
        {
            
        }

        public virtual void Check(OleDbConnection connection)
        {
           
        }
    }
}
