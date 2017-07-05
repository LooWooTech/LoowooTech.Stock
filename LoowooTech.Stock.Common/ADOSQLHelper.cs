using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Common
{
    public static class ADOSQLHelper
    {
        private static readonly object _syncRoot = new object();
        public static OleDbDataReader ExecuteReader(OleDbConnection connection,string sqlText)
        {
            lock (_syncRoot)
            {
                try
                {
                    if (connection != null)
                    {
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
                            command.CommandText = sqlText;
                            var reader = command.ExecuteReader();
                            return reader;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    var info = string.Format("执行SQL语句：{0}发生错误，错误信息：{1}", sqlText, ex.Message);
                    LogManager.Log(info);
                    QuestionManager.Add(new Models.Question { Code = "1201", Project = Models.CheckProject.数据库查询, Description = info });
                }
                catch (OleDbException ex)
                {
                    var info = string.Format("执行SQL语句:{0}发生错误，错误信息：{1}", sqlText, ex.Message);
                    LogManager.Log(info);
                    QuestionManager.Add(new Models.Question { Code = "1201", Project = Models.CheckProject.数据库查询, Description = info });
                }

                return null;
            }
           
        }
        public static object ExecuteScalar(OleDbConnection connection,string sqlText)
        {
            lock (_syncRoot)
            {
                try
                {
                    if (connection != null)
                    {
                        if (connection.State == System.Data.ConnectionState.Broken)
                        {
                            connection.Close();
                            connection.Open();
                        }
                        if (connection.State == System.Data.ConnectionState.Closed)
                        {
                            if (connection.State != System.Data.ConnectionState.Connecting)
                            {
                                connection.Open();
                            }
                        }
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = sqlText;
                            var val = command.ExecuteScalar();
                            return val;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    var info = string.Format("执行SQL语句：{0}发生错误，错误信息：{1}", sqlText, ex.Message);
                    LogManager.Log(info);
                    QuestionManager.Add(new Models.Question { Code = "1201", Project = Models.CheckProject.数据库查询, Description = info });
                }
                catch (OleDbException ex)
                {
                    var info = string.Format("执行SQL语句:{0}发生错误，错误信息：{1}", sqlText, ex.Message);
                    LogManager.Log(info);
                    QuestionManager.Add(new Models.Question { Code = "1201", Project = Models.CheckProject.数据库查询, Description = info });
                }

                return null;
            }

            
        }
    }
}
