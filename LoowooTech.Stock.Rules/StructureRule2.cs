using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class StructureRule2:IRule
    {
        public string RuleName { get { return "结构符合性"; } }
        public string ID { get { return "1301"; } }
        public bool Space { get { return false; } }
        public void Check()
        {
            if (ParameterManager2.Connection == null)
            {

                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject2.结构符合性,
                    Description = string.Format("无法获取数据库连接字符串信息，无法进行检查"),
                    Folder = ""
                });
                return;
            }

            foreach(var table in ParameterManager2.Tables)
            {
                var tool = new FieldStructureTool2 { CheckTable = table };
                tool.Check(ParameterManager2.Connection);
                var mes = tool.Message;
                if (string.IsNullOrEmpty(mes) == false)
                {
                    QuestionManager2.Add(new Question2
                    {
                        Code = ID,
                        Name = RuleName,
                        Description = mes,
                        CheckProject = CheckProject2.结构符合性,
                        TableName = table.Name,
                        Folder = ""
                    });
                }
            }

        }
    }
}
