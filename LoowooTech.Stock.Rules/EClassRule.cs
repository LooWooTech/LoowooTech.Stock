using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class EClassRule:ClassBaseTool,Models.IRule
    {
        public override string RuleName { get { return "E类转换逻辑一致性检查"; } }
        public override string ID { get { return "2006"; } }
        public override CheckProject2 CheckProject { get { return CheckProject2.E类转换过程合理性; } }
        public bool Space { get { return true; } }
        public override void Check()
        {
            if(ExtractJQDLTB("ZHLX = 'E'") == true)
            {
                var sql= "SELECT COUNT(*) FROM DLTB WHERE DLBM != '205'";
                var obj = SearchRecord(MdbFilePath,sql);
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
                                Description = string.Format("E类转换前地类存在【{0}】条记录为非风景名称及特殊用地", count)
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
                            Description = string.Format("核查E类基数转换前是否为风景名胜及特殊用地的时候，执行SQL语句失败，SQL【{0}】", sql)
                        });
                    }
                }


            }
        }
    }
}
