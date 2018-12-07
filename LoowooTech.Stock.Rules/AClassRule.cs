using ESRI.ArcGIS.Geodatabase;
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
    public class AClassRule:ClassBaseTool, Models.IRule
    {
        public override string RuleName { get { return "A类转换逻辑一致性检查"; } }
        public override string ID { get { return "2003"; } }
        public bool Space { get { return false; } }
        public override CheckProject2 CheckProject { get { return CheckProject2.A类转换过程合理性; } }
        public override void Check()
        {
            #region （1）基数转换前可调整地类是否归入
            #endregion

            //（2）A类转换后规划地类应为水田、旱地
         

            var tool = new ValueRangeTool2 { TableName = "JQDLTB", Key = "BSM", CheckFieldName = "DLDM", WhereClause = "ZHLX = 'A'", Values = new string[] { "111", "113" }, RelationName = "JQDLTB", ID = "" };
            Tools.Add(tool);
            base.Check();

            if(ExtractJQDLTB("ZHLX = 'A'") == true)
            {
                var obj = SearchRecord(MdbFilePath, "SELECT COUNT(*) FROM DLTB WHERE DLBM = '011' OR DLBM = '013'");
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
                                Description = string.Format("A类基数转换前的DLTB层存在【{0}】条记录为水田或旱地", count)
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
                            Description = string.Format("核查A类基数转换前是否为非耕地的时候，执行SQL语句失败,obj【{0}】", obj.ToString())
                        });
                    }
                }
               
            }

        }
    }
}
