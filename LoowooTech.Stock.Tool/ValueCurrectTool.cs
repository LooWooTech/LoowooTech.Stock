using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    /// <summary>
    /// 作用：指定某几个字段，查询是否符合值
    /// 作者：汪建龙
    /// 编写时间：2017年5月4日13:55:52
    /// </summary>
    public class ValueCurrectTool:ValueBaseTool, ITool
    {
        public string[] Fields { get; set; }
        public string Split { get; set; }
        public List<string> Values { get; set; }
        public string Name
        {
            get
            {

                return string.Format("规则{0}：表‘{1}’中字段‘{2}’组成的值相互对应" ,ID,TableName,string.Join("、",Fields));
            }
        }
        public string Key { get; set; }
        public string Code { get; set; }

        public bool Check(OleDbConnection connection)
        {
            var reader = ADOSQLHelper.ExecuteReader(connection, string.Format("Select {0},{1} from {2}", string.Join(",", Fields),Key, TableName));
            if (reader != null)
            {
                var array = new string[Fields.Length];
                var info = string.Empty;
                while (reader.Read())
                {
                    array = new string[Fields.Length];
                    for (var i = 0; i < Fields.Length; i++)
                    {
                        array[i] = reader[i] != null ? reader[i].ToString() : string.Empty;
                        //array[i] = Fields[i].ToString();
                    }
                    var val = string.Join(Split, array);
                    if (!Values.Contains(val))
                    {
                        info = string.Format("不存在：{0}", val);
                        Messages.Add(info);
                        _questions.Add(
                            new Question
                            {
                                Code = Code,
                                Name = Name,
                                Project = CheckProject.属性正确性,
                                TableName = TableName,
                                BSM=reader[Fields.Length].ToString(),
                                Description = info,
                                ShowType=ShowType.Space,
                                WhereClause=string.Format("[{0}] ='{1}'",Key,reader[Fields.Length].ToString())
                            });
                    }
                }
                QuestionManager.AddRange(_questions);
                return true;
            }
          
            return false;
        }
    }
}
