using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class ExcelHeart
    {
        private List<Question> _questions { get; set; }
        /// <summary>
        /// 问题
        /// </summary>
        public List<Question> Questions { get { return _questions; } }
        private string _folder { get; set; }
        /// <summary>
        /// 统计表格路径
        /// </summary>
        public string Folder { get { return _folder; } set { _folder = value; } }

        private string _mdbFilePath { get; set; }
        /// <summary>
        /// mdb数据库文件路径
        /// </summary>
        public string MDBFilePath { get { return _mdbFilePath; }set { _mdbFilePath = value; } }
        private List<XZC> _List { get; set; }
        public List<XZC> List { get { return _List; } }
        public ExcelHeart()
        {
            _questions = new List<Question>();
        }
        public void Program()
        {
            var _connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", _mdbFilePath);
            using (var connection=new OleDbConnection(_connectionString))
            {
                connection.Open();
                try
                {
                    var reader = ADOSQLHelper.ExecuteReader(connection, "Select XZCDM,XZCMC from XZQ_XZC");
                    if (reader != null)
                    {
                        var xzcdm = string.Empty;
                        var xzcmc = string.Empty;
                        while (reader.Read())
                        {
                            xzcdm = reader[0].ToString().Trim();
                            xzcmc = reader[1].ToString().Trim();
                            if (!_List.Any(e => e.XZCDM == xzcdm && e.XZCMC == xzcmc))
                            {
                                _List.Add(new XZC
                                {
                                    XZCDM = xzcdm,
                                    XZCMC = xzcmc
                                });
                            }
                        }
                    }
                }
                catch
                {
                    _questions.Add(new Question { Code = "", Name = "", Project = CheckProject.汇总表与数据库图层逻辑一致性, TableName = "XZQ_XZC", Description = "" });
                }

                connection.Close();
            }
        }
    }
}
