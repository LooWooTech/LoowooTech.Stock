using LoowooTech.Stock.Common;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Threading.Tasks;

namespace LoowooTech.Stock.Rules
{
    public class TableHeart
    {
        private const string DCDYTB = "DCDYTB";
        public static List<string> _errors { get; set; }

        public static  void Program(string mdbfilePath,string[] ids)
        {
            var _connectionString= string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", mdbfilePath);
            var info = string.Empty;
            using (var connection=new OleDbConnection(_connectionString))
            {
                connection.Open();
                LogManager.Log("开始检查");
                #region  检查数据库是否存在要求的表
                var tableStructure = new TableStructure();
                LogManager.Log(string.Format("开始检查{0}", tableStructure.Name));
                tableStructure.Check(connection);
                if (tableStructure.Erros.Count==0)
                {
                    info = string.Format("符合{0}", tableStructure.Name.Replace("检查", ""));
                    LogManager.Log(info);
                }
                else
                {
                    QuestionManager.AddRange(tableStructure.Erros.Select(e => new Models.Question { Code = "2101", Name = "矢量图层完整",Project=Models.CheckProject.图层完整性, Description = e }).ToList());
                }
                #endregion
                if (tableStructure.ExistTables.Contains(DCDYTB))
                {
                    //DCDYTBManager.Init(connection);
                }
                else
                {
                    LogManager.Record("不存在或者未找到表DCDYTB，未获取图斑相关信息");
                }
                var rule = new RuleManager() { IDS = ids };
                rule.Program(connection);


                #region 检查图斑面积
                LogManager.Log("核对图斑面积");

                var tbTools = new List<GainAreaTool>() {
                    new GainAreaTool { TableName="CLZJD",AreaFields=new string[] { "JZZDMJ", "FSYDMJ" } ,Denominator=10000},
                    new GainAreaTool { TableName="JYXJSYD",AreaFields=new string[] { "JSYDMJ" },Denominator=10000 },
                    new GainAreaTool { TableName="GGGL_GGFWSSYD",AreaFields=new string[] { "JSYDMJ" },Denominator=10000 },
                    new GainAreaTool { TableName="QTCLJSYD",AreaFields=new string[] { "JSYDMJ" },Denominator=10000 }
                };
                try
                {
                    Parallel.ForEach(tbTools, tool =>
                    {
                        tool.Gain(connection);
                    });
                    DCDYTBManager.Program();

                }
                catch(AggregateException ae)
                {
                    foreach(var exp in ae.InnerExceptions)
                    {
                        LogManager.Log(exp.ToString());

                    }
                }catch(Exception ex)
                {
                    LogManager.Log(ex.ToString());
                    LogManager.Record(ex.ToString());
                }
              

                Console.WriteLine("完成核对图斑面积一致性;");
                #endregion

                connection.Close();
            }
        }
    }
}
