using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public static class TableDCDYTB
    {
        private static List<Value> _dcdytbs { get; set; }

        /// <summary>
        /// 作用：获取行政村名称、代码 调查单元类型
        /// 作者：汪建龙
        /// 编写时间：2017年4月18日10:41:06
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public  static List<Value> GetDCDYTBs(OleDbConnection connection)
        {
            if (_dcdytbs != null && _dcdytbs.Count > 0)
            {
                return _dcdytbs;
            }
            if (connection != null)
            {
                if (_dcdytbs == null)
                {
                    _dcdytbs = new List<Value>();
                }
                _dcdytbs.Clear();
                if (connection.State == System.Data.ConnectionState.Broken)
                {
                    connection.Close();
                    connection.Open();
                }
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select XZCDM,XZCMC,DCDYLX from DCDYTB";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _dcdytbs.Add(new Value
                            {
                                XZCDM = reader[0].ToString(),
                                XZCMC = reader[1].ToString(),
                                DCDYLX = reader[2].ToString()
                            });
                        }
                    }
                }
            }
            return _dcdytbs;
        }

        private static List<XZC> _xzcs { get; set; }

        /// <summary>
        /// 作用：获取行政村名称、代码相关信息
        /// 作者：汪建龙
        /// 编写时间：2017年4月18日10:56:16
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static List<XZC> GetXZCs(OleDbConnection connection)
        {
            if (_xzcs != null && _xzcs.Count > 0)
            {
                return _xzcs;
            }
            if (connection != null)
            {
                if (_xzcs == null)
                {
                    _xzcs = new List<XZC>();
                }
                _xzcs.Clear();
                if (connection.State == System.Data.ConnectionState.Broken)
                {
                    connection.Close();
                    connection.Open();
                }
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select XZQDM,XZQMC from XZQ_XZC";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _xzcs.Add(new XZC
                            {
                                XZCDM = reader[0].ToString(),
                                XZCMC = reader[1].ToString()
                            });
                        }
                    }
                }

            }
            return _xzcs;
        }
    }
}
