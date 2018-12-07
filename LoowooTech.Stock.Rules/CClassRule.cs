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
    public class CClassRule:ClassBaseTool,Models.IRule
    {
        public override string RuleName { get { return "C类转换逻辑一致性检查"; } }
        public override string ID { get { return "2005"; } }
        public override CheckProject2 CheckProject { get { return CheckProject2.C类转换过程合理性; } }

        public bool Space { get { return true; } }
        public override void Check()
        {
            var tools = new List<IVTool>()
            {
                new ValueNullTool2{ TableName="JQDLTB",Key="BSM",Is_Nullable=true,CheckFields=new string[]{ "PZWH"},WhereClause="ZHLX = 'C'" },
                new ValueRangeTool2{ TableName="JQDLTB",Key="BSM", CheckFieldName="DLDM",Values=new string[]{ "2121","2122","2123","2125","2126","2124","221","226","213","231","232","211"},WhereClause="ZHLX = 'C'" }
            };
            Tools.AddRange(tools);
            base.Check();
          
            if(ExtractJQDLTB("ZHLX = 'C'") == true)
            {
                var sql = "SELECT COUNT(*) FROM DLTB WHERE DLBM LIKE '20*' OR (DLBM LIKE '10*' AND DLBM != '104') OR DLBM = '113'";
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
                                Description = string.Format("C类基数转换前的DLTB中存在【{0}】条记录为建设用地", count)
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
                            Description = string.Format("核查C类基数转换前是否为非建设用地的时候，执行SQL语句失败，SQL【{0}】", sql)
                        });
                    }
                }
              

            }
        }
    }
}
