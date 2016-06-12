using LoowooTech.Stock.Common;
using System;
using System.Data.OleDb;

namespace LoowooTech.Stock.Rules
{
    public class TableHeart
    {

        public static  void Program(string mdbfilePath)
        {
            var _connectionString= string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", mdbfilePath);
            using (var connection=new OleDbConnection(_connectionString))
            {
                connection.Open();
                #region  检查数据库是否存在要求的表
                var tableStructure = new TableStructure();
                Console.WriteLine(string.Format("开始检查{0}", tableStructure.Name));
                tableStructure.Check(connection);
                if (tableStructure.Erros.Count==0)
                {
                    Console.WriteLine(string.Format("符合{0}", tableStructure.Name.Replace("检查", "")));
                }
                else
                {
                    Console.WriteLine("存在如下错误：");
                    foreach(var error in tableStructure.Erros)
                    {
                        Console.WriteLine(error);
                    }

                }
                #endregion

                #region 检查存在的表
                foreach(var table in tableStructure.ExistTables)
                {
                    var className = XmlClass.GetClass(table);
                    if (string.IsNullOrEmpty(className))
                    {
                        Console.WriteLine(string.Format("缺失对表{0}的类名，无法进行检查", table));
                    }
                    else
                    {
                        object o = System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(className, false);
                        ITable instrument = o as ITable;
                        Console.WriteLine(string.Format("开始进行{0}", instrument.Name));
                        instrument.Check(connection);
                        if (instrument.Errors.Count == 0 && instrument.Results.Count == 0)
                        {
                            Console.WriteLine("表{0}符合检查规则！", instrument._tableName);
                        }
                        else
                        {
                            Console.WriteLine(string.Format("在检查表{0}存在如下错误：", instrument._tableName));
                            foreach(var error in instrument.Errors)
                            {
                                Console.WriteLine(error);
                            }
                            foreach(var entry in instrument.Results)
                            {
                                Console.WriteLine(entry.Key);
                                Console.WriteLine("对应的错误如下：");
                                foreach(var error in entry.Value)
                                {
                                    Console.WriteLine(error);
                                }
                            }
                        }
                    }
                }
                #endregion
                connection.Close();
            }
        }
    }
}
