using LoowooTech.Stock.Common;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;

namespace LoowooTech.Stock.Rules
{
    public class TableHeart
    {
        public static List<string> _errors { get; set; }

        public static  void Program(string mdbfilePath)
        {
            var _connectionString= string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", mdbfilePath);
            using (var connection=new OleDbConnection(_connectionString))
            {
                connection.Open();
                Console.WriteLine("开始检查");
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
                    QuestionManager.AddRange(tableStructure.Erros.Select(e => new Models.Question { Code = "2101", Name = "矢量图层完整",Project=Models.CheckProject.图层完整性, Description = e }).ToList());
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

                #region 检查图斑面积

                Console.WriteLine("正在核对图斑面积一致性;");

                try
                {
                    DCDYTBManager.Program();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
              

                Console.WriteLine("完成核对图斑面积一致性;");
                #endregion

                #region 检查图斑编号

                //var combination = new ValueCombinationTool() { CheckField = "TBBH", Tables = new string[] { "CLZJD", "JYXJSYD", "GGGL_GGFWSSYD", "QTCLYD" }, ID = "00000(逻辑规则)" };
                //Console.WriteLine(string.Format("开始检查：{0}", combination.Name));
                //try
                //{
                //    combination.Check(connection);
                //    if (combination.Messages.Count == 0)
                //    {
                //        Console.WriteLine(string.Format("顺利通过{0}的检查", combination.Name));
                //    }
                //    else
                //    {
                //        Console.WriteLine(string.Format("检查{0}，存在如下错误：", combination.Name));
                //        foreach(var error in combination.Messages)
                //        {
                //            Console.WriteLine(error);
                //        }
                //    }
                //}catch(Exception ex)
                //{
                //    Console.WriteLine(string.Format("在进行检查{0}发生错误，错误信息：{1}", combination.Name, ex.Message));
                //}

                //Console.WriteLine("完成本次检查");
                #endregion
                connection.Close();
            }
        }
    }
}
