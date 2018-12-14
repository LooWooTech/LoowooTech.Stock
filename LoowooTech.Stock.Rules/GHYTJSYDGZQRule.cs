using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class GHYTJSYDGZQRule:ArcGISBaseTool,Models.IRule
    {
        public override string RuleName { get { return "规划用途与建设用地管制区逻辑一致性检查"; } }
        public override string ID { get { return "3003"; } }
        public override CheckProject2 CheckProject { get { return CheckProject2.规划用途与建设用地管制区对比检查; } }
        public bool Space { get { return true; } }

        public void Check()
        {
            if (ExtractGHYT() == false)
            {
                return;
            }

            if (ArcExtensions2.ImportFeatureClass(string.Format("{0}\\{1}", ParameterManager2.MDBFilePath, "JSYDGZQ"), MdbFilePath, "JSYDGZQ", null) == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("导入建设用地管制区图层数据失败！")
                });
                return;
            }

            if (ArcExtensions2.Select(string.Format("{0}\\{1}", MdbFilePath, "GHYT"), string.Format("{0}\\{1}", MdbFilePath, "GHYT_G21"), "GHYTDM LIKE 'G21*'") == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("规划用途层中提取新增城乡建设用地数据失败！")
                });
            }
            else
            {
                if (ArcExtensions2.Select(string.Format("{0}\\{1}", MdbFilePath, "JSYDGZQ"), string.Format("{0}\\{1}", MdbFilePath, "JSYDGZQ_010"), "GZQLXDM = '010'") == false)
                {
                    QuestionManager2.Add(new Question2
                    {
                        Code = ID,
                        Name = RuleName,
                        CheckProject = CheckProject,
                        Description = string.Format("提取建设用地管制区中允许建设区图层失败")
                    });
                }
                else
                {
                    if (ArcExtensions2.Erase(string.Format("{0}\\{1}", MdbFilePath, "GHYT_G21"), string.Format("{0}\\{1}", MdbFilePath, "JSYDGZQ_010"), string.Format("{0}\\{1}", MdbFilePath, "GHYT_G21_Erase"), ParameterManager2.Tolerance) == false)
                    {
                        QuestionManager2.Add(new Question2
                        {
                            Code = ID,
                            Name = RuleName,
                            CheckProject = CheckProject,
                            Description = string.Format("新增城乡建设用地Erase允许建设区失败！")
                        });
                    }
                    else
                    {
                        var sql = "SELECT COUNT(*) FROM GHYT_G21_Erase";
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
                                        Description = string.Format("存在新增城乡建设用地不在允许建设区内")
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
                                    Description = string.Format("查询新增城乡建设用地是否与允许建设区内失败")
                                });
                            }
                        }
                    }
                }
            }

            if(ArcExtensions2.Select(string.Format("{0}\\{1}",MdbFilePath,"GHYT"),string.Format("{0}\\{1}",MdbFilePath,"GHYT_E"),"CJJX = 'E1' OR CJJX = 'E2'") == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("提取规划用图层中拟新增E2、虚拟新增E1数据失败")
                });
            }
            else
            {
                if(ArcExtensions2.Select(string.Format("{0}\\{1}",MdbFilePath,"GHYT"),string.Format("{0}\\{1}",MdbFilePath,"JSYDGZQ_021"),"GZQLXDM = '021' OR GZQLXDM = '020'") == false)
                {
                    QuestionManager2.Add(new Question2
                    {
                        Code = ID,
                        Name = RuleName,
                        CheckProject = CheckProject,
                        Description = string.Format("提取建设用地管制区中有条件建设区失败")
                    });
                }
                else
                {
                    if (ArcExtensions2.Erase(string.Format("{0}\\{1}", MdbFilePath, "GHYT_E"), string.Format("{0}\\{1}", MdbFilePath, "JSYDGZQ_021"),string.Format("{0}\\{1}",MdbFilePath,"GHYT_E_Erase"), ParameterManager2.Tolerance) == false)
                    {
                        QuestionManager2.Add(new Question2
                        {
                            Code = ID,
                            Name = RuleName,
                            CheckProject = CheckProject,
                            Description = string.Format("拟新增、虚拟新增Erase有条件建设区失败")
                        });
                    }
                    else
                    {
                        var sql = "SELECT COUNT(*) FROM GHYT_E_Erase";
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
                                        Description = string.Format("存在拟新增E2、虚拟新增E1不在有条件建设区")
                                    });
                                }
                            }
                        }
                    }
                }
            }
           

            if(ArcExtensions2.Select(string.Format("{0}\\{1}",MdbFilePath,"GHYT"),string.Format("{0}\\{1}",MdbFilePath,"GHYT_G111"),"GHYTDM = 'G111'") == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("提取规划用途层中示范区基本农田数据失败")
                });
            }
            else
            {
                if(ArcExtensions2.Select(string.Format("{0}\\{1}",MdbFilePath,"JSYDGZQ"),string.Format("{0}\\{1}",MdbFilePath,"JSYDGZQ_040"),"GZQLXDM = '040'") == false)
                {
                    QuestionManager2.Add(new Question2
                    {
                        Code = ID,
                        Name = RuleName,
                        CheckProject = CheckProject,
                        Description = string.Format("提取禁止建设区数据失败")
                    });
                }
                else
                {
                    if (ArcExtensions2.Erase(string.Format("{0}\\{1}", MdbFilePath, "GHYT_G111"), string.Format("{0}\\{1}", MdbFilePath, "JSYDGZQ_040"), string.Format("{0}\\{1}", MdbFilePath, "GHYT_G111_Erase"), ParameterManager2.Tolerance) == false)
                    {
                        QuestionManager2.Add(new Question2
                        {
                            Code = ID,
                            Name = RuleName,
                            CheckProject = CheckProject,
                            Description = string.Format("示范区基本农田Erase禁止建设区失败")
                        });
                    }
                    else
                    {
                        var sql = "SELECT COUNT(*) FROM GHYT_G111_Erase";
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
                                        Description = string.Format("存在示范区基本农田部不在禁止建设区")
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
