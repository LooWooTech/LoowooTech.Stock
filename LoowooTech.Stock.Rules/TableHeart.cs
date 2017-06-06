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
        private const string DCDYTB = "DCDYTB";
        public static List<string> _errors { get; set; }

        public static  void Program(string mdbfilePath,string[] ids)
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
                if (tableStructure.ExistTables.Contains(DCDYTB))
                {
                    DCDYTBManager.Init(connection);
                }
                var rule = new RuleManager() { IDS = ids };
                rule.Program(connection);


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

                connection.Close();
            }
        }
    }
}
