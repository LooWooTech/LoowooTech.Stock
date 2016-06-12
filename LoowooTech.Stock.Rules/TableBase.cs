using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Threading.Tasks;

namespace LoowooTech.Stock.Rules
{
    public class TableBase
    {
        public string _tableName { get; set; }
        protected string _key { get; set; }
        public virtual string Name { get; }
        protected List<ITool> list { get; set; }
        public List<string> Errors { get; set; }
        public  Dictionary<string,List<string>> Results { get; set; }
        public TableBase()
        {
            list = new List<ITool>();
            Errors = new List<string>();
            Results = new Dictionary<string, List<string>>();
        }

        public void Check(OleDbConnection connection)
        {
            ParallelCheck(connection);
        }

        private void ParallelCheck(OleDbConnection connection)
        {
            foreach (var tool in list)
            {
                Parallel.Invoke(() =>
                {
                    try
                    {
                        if (tool.Check(connection))
                        {
                            Results.Add(tool.Name, tool.Messages);
                        }
                        else
                        {
                            Errors.Add(string.Format("检查{0}失败", tool.Name));
                        }

                    }
                    catch (Exception ex)
                    {
                        Errors.Add(string.Format("检查{0}的时候，发生错误：{1}", tool.Name, ex.Message));
                    }
                });
            }
        }

        private void ParallelForCheck(OleDbConnection connection)
        {
            Parallel.ForEach(list, tool =>
            {
                try
                {
                    if (tool.Check(connection))
                    {
                        Results.Add(tool.Name, tool.Messages);
                    }
                    else
                    {
                        Errors.Add(string.Format("检查{0}失败!", tool.Name));
                    }
                }
                catch (Exception ex)
                {
                    Errors.Add(string.Format("检查{0}的时候，发生错误：{1}", tool.Name, ex.Message));
                }
            });
        }
    }


}
