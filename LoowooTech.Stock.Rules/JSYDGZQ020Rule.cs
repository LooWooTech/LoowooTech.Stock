using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class JSYDGZQ020Rule:ArcGISBaseTool,Models.IRule
    {
        public override string RuleName { get { return "有条件建设区一致性检查"; } }
        public override string ID { get { return "6006"; } }
        public override CheckProject2 CheckProject { get { return CheckProject2.村规划020是否在乡规划020内; } }
        public bool Space { get { return true; } }
        public void Check()
        {
            if (System.IO.File.Exists(ParameterManager2.XGH) == false)
            {
                return;
            }

            if (ExtractJSYDGZQ("GZQLXDM = '020'") == false)
            {
                return;
            }
            //JSYDGZQ  村规划有条件建设区

            var temps = ParameterManager2.XZCList.Select(e => string.Format("XZQDM = '{0}'", e.XZCDM)).ToArray();
            var where = string.Join(" OR ", temps);
            if (ArcExtensions2.Select(string.Format("{0}\\{1}", ParameterManager2.XGH, "XZQ"), string.Format("{0}\\{1}", MdbFilePath, "XZQ"), where) == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("提取行政区矢量数据失败！")
                });
                return;
            }

            //XZQ 村规划行政区范围

            if (ArcExtensions2.Select(string.Format("{0}\\{1}", ParameterManager2.XGH, "JSYDGZQ"), string.Format("{0}\\{1}", MdbFilePath, "JSYDGZQ_X"), "GZQLXDM = '020'") == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("提取乡规划中有条件建设区数据失败")
                });
                return;
            }

            //JSYDGZQ_X 乡规划中所有的有条件建设区

            if (ArcExtensions2.Clip(string.Format("{0}\\{1}", MdbFilePath, "JSYDGZQ_X"), string.Format("{0}\\{1}", MdbFilePath, "XZQ"), string.Format("{0}\\{1}", MdbFilePath, "JSYDGZQ_X_XZQ"), ParameterManager2.Tolerance) == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("提取行政区范围内的乡规划中的有条件建设区数据失败")
                });
                return;
            }

            //JSYDGZQ_X_XZQ 乡规划中当前核对行政区范围内的有条件建设区

            if (ArcExtensions2.Erase(string.Format("{0}\\{1}", MdbFilePath, "JSYDGZQ"), string.Format("{0}\\{1}", MdbFilePath, "JSYDGZQ_X_XZQ"), string.Format("{0}\\{1}", MdbFilePath, "JSYDGZQ_Erase"), ParameterManager2.Tolerance) == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("村规划中有条件建设区数据Erase乡规划中有条件建设区数据失败")
                });

            }
            else
            {
                var sql = "SELECT COUNT(*) FROM JSYDGZQ_Erase";
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
                                Description = string.Format("村规划有条件建设区范围大于乡规划有条件建设区范围")
                            });
                        }
                    }
                }
            }
        }
    }
}
