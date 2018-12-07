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
    public class FClassRule : ClassBaseTool, Models.IRule
    {
        public override string RuleName { get { return "F类转换逻辑一致性检查"; } }
        public override string ID { get { return "2007"; } }
        public override CheckProject2 CheckProject { get { return CheckProject2.F类转换过程合理性; } }
        public bool Space { get { return true; } }
        public override void Check()
        {
            Tools.Add(new ValueRangeTool2 { TableName = "JQDLTB", Key = "BSM", CheckFieldName = "DLDM", Values = new string[] { "211" }, WhereClause = "ZHLX = 'F'" });
            base.Check();
            if(ExtractJQDLTB("ZHLX = 'F'") == true)
            {
                var sql = "SELECT COUNT(*) FROM DLTB WHERE DLBM LIKE '01*' OR DLBM LIKE '02*' OR DLBM LIKE '03*' OR DLBM LIKE '04*' OR DLBM = '104' OR (DLBM LIKE '11*' AND DLBM != '113' AND DLBM != '118') OR DLBM LIKE '12*'";
                var obj = SearchRecord(MdbFilePath, sql);
                if (obj != null)
                {
                    var count = 0;
                    if(int.TryParse(obj.ToString(),out count))
                    {
                        if (count > 0)
                        {
                            QuestionManager2.Add(new Question2
                            {
                                Code = ID,
                                Name = RuleName,
                                CheckProject = CheckProject,
                                Description = string.Format("F类基数转换前的DLTB中存在【{0}】条记录为非建设用地", count)
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
                            Description = string.Format("核查F类基数转换钱是否为及为建设用地的时候，执行SQL【{0}】失败", sql)
                        });
                    }
                }

            }
        }
    }
}
