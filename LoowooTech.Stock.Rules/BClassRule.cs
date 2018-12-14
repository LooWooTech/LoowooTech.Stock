using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using LoowooTech.Stock.Tool;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class BClassRule:ClassBaseTool,IRule
    {
        public override string RuleName { get { return "B类转换逻辑一致性检查"; } }
        public override string ID { get { return "2004"; } }
        public bool Space { get { return true; } }
        public override CheckProject2 CheckProject { get { return CheckProject2.B类转换过程合理性; } }

        public override void Check()
        {
            Tools.Add(new ValueNullTool2 { TableName = "JQDLTB", Key = "BSM", Is_Nullable = false, CheckFields = new string[] { "PZWH" }, WhereClause = "ZHLX = 'B'" });
            Tools.Add(new ValueRangeTool2 { TableName = "JQDLTB", Key = "BSM", CheckFieldName = "DLDM", Values = new string[] { "111", "113" }, WhereClause = "ZHLX = 'B'" });
            base.Check();




            if (ExtractJQDLTB("ZHLX = 'B'") == true)
            {
                var sql = "SELECT COUNT(*) FROM DLTB WHERE DLBM = '011' OR DLBM = '013'";
                var obj = SearchRecord(MdbFilePath, sql);
                if (obj != null)
                {
                    var count = 0;
                    if (int.TryParse(obj.ToString(), out count))
                    {
                        if (count > 0)
                        {
                            QuestionManager2.Add(new Question2
                            {
                                Code = ID,
                                Name = RuleName,
                                CheckProject = CheckProject,
                                Description = string.Format("B类基数转换前的DLTB中存在【{0}】条记录为耕地", count)
                            });
                        }
                    }
                    else
                    {
                        QuestionManager2.Add(new Question2
                        {
                            Code = ID,
                            Name = RuleName,
                            CheckProject = CheckProject,
                            TableName = "JQDLTB",
                            Description = string.Format("核查B类基数转换前是否为非耕地的时候，执行SQL语句失败,SQL【{0}】", sql)
                        });
                    }
                }
               
            }

            

        }
    }
}
