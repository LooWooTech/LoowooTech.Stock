using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Common
{
    public static class ADOSQLHelper
    {
        public static OleDbDataReader ExecuteReader(OleDbConnection connection,string sqlText)
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
                    connection.Close();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sqlText;
                    var reader = command.ExecuteReader();
                    return reader;
                }
            }
            return null;
        }

        public static object ExecuteScalar(OleDbConnection connection,string sqlText)
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
                    connection.Close();
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sqlText;
                    var val= command.ExecuteScalar();
                    return val;
                }
            }
            return null;
            
        }


    }
}
