using LoowooTech.Stock.Common;
using System;
using System.Data.OleDb;

namespace LoowooTech.Stock.Rules
{
    public class CESHI
    {
        public static void Program(string mdbfilePath)
        {
            var _connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", mdbfilePath);
            using (var connection=new OleDbConnection(_connectionString))
            {
                connection.Open();
                var structure = new TableStructure();
                Console.WriteLine(string.Format("开始检查{0}", structure.Name));
                structure.Check(connection);


                var list = MdbClass.GetFields(connection, "GYYD");
                connection.Close();
            }
        }
    }
}
