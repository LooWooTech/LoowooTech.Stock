using LoowooTech.Stock.Models;
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

        public static int ExecuteNoQuery(OleDbConnection connection,string sqlText)
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
                            var rows = command.ExecuteNonQuery();
                            return rows;
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

                return 0;
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
        public static string[] GetValues(OleDbDataReader reader,int start,int length)
        {
            var values = new string[length];
            for(var i = 0; i < length; i++)
            {
                var obj = reader[start + i];
                if (obj != null)
                {
                    values[i] = obj.ToString();
                }
                
            }
            return values;
        }

        public static string GetWhereClause(string[] fields,string[] values)
        {
            if (fields.Length != values.Length)
            {
                return string.Empty;
            }
            var sb = new StringBuilder(string.Format("[{0}] ='{1}'", fields[0], values[0]));
            if (fields.Length > 1)
            {
                for (var i = 1; i < fields.Length; i++)
                {
                    sb.AppendFormat(" AND [{0}] ='{1}'", fields[i], values[i]);
                }
            }
            return sb.ToString();
        }


        public static List<string> GetUniqueValue(string file,string tableName,string field)
        {
            var list = new List<string>();
            using (var connection=new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", file)))
            {
                connection.Open();
                var sqlText = string.Format("Select {0} FROM {1} GROUP BY {0}", field, tableName);
                var reader = ExecuteReader(connection, sqlText);
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var val = reader[0].ToString();
                        list.Add(val);

                    }
                }
                connection.Close();
            }

            return list;
        }

        public static List<SearchInfo> Query(string file,List<SearchInfo> infos)
        {
            using (var connection=new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", file)))
            {
                connection.Open();
                foreach(var item in infos)
                {
                    item.Value = ExecuteScalar(connection, item.SQL);
                }

                connection.Close();
            }
            return infos;
        }
    }
}
