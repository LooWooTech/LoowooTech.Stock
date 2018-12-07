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
    public class GHYTJQDLTBRule:ArcGISBaseTool,Models.IRule
    {
        public override string RuleName { get { return "规划用途与基数图斑逻辑一致性检查"; } }
        public override string ID { get { return "3001"; } }
        public override CheckProject2 CheckProject { get { return CheckProject2.规划用途数据与基期图斑数据地类逻辑一致性检查; } }
        public bool Space { get { return true; } }

        public void Check()
        {
            if (ExtractGHYT() == false)
            {
                return;
            }
            if (ArcExtensions2.ImportFeatureClass(string.Format("{0}\\{1}", ParameterManager2.MDBFilePath, "JQDLTB"), MdbFilePath, "JQDLTB", null) == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("导入基期地类图斑层失败！")
                });
                return;
            }

            if(ArcExtensions2.Select(string.Format("{0}\\{1}",MdbFilePath,"GHYT"),string.Format("{0}\\{1}_X",MdbFilePath,"GHYT"),"GHYTDM LIKE 'X*'") == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("提取规划用途层中为现状用地数据失败！")
                });
            }
            else
            {
                if (ArcExtensions2.Intersect(string.Format("{0}\\{1};{0}\\{2}", MdbFilePath, "GHYT_X", "JQDLTB"), string.Format("{0}\\{1}", MdbFilePath, "JQDLTB_X"), ParameterManager2.Tolerance) == false)
                {
                    QuestionManager2.Add(new Question2
                    {
                        Code = ID,
                        Name = RuleName,
                        CheckProject = CheckProject,
                        Description = string.Format("规划用途层中现状用途层与基期地类图斑相交失败！")
                    });
                }
                else
                {
                    var sql = "SELECT COUNT(*) FROM JQDLTB_X WHERE MID(GHYTDM,2)!=DLDM";
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
                                    Description = string.Format("规划用途中现状用途的存在【{0}】个地块与基期地类图斑层地类不一致", count)
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
                                Description = string.Format("执行SQL【{0}】获取obj转换int失败", sql)
                            });
                        }
                    }
                }
            }


            if(ArcExtensions2.Select(string.Format("{0}\\{1}",MdbFilePath,"GHYT"),string.Format("{0}\\{1}_G",MdbFilePath,"GHYT"),"GHYTDM LIKE 'G*'") == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("提取规划用途中为新增用途数据失败！")
                });
            }
            else
            {
                if (ArcExtensions2.Intersect(string.Format("{0}\\{1};{0}\\{2}", MdbFilePath, "GHYT_G", "JQDLTB"), string.Format("{0}\\{1}", MdbFilePath, "JQDLTB_G"), ParameterManager2.Tolerance) == false)
                {
                    QuestionManager2.Add(new Question2
                    {
                        Code = ID,
                        Name = RuleName,
                        CheckProject = CheckProject,
                        Description = string.Format("规划用途层为新增用途层与基期地类图斑相交失败！")
                    });
                }
                else
                {
                    var sql = "SELECT COUNT(*) FROM JQDLTB_G WHERE MID(GHYTDM,2)=DLDM";
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
                                    Description = string.Format("规划用途中新增用途存在【{0}】个地块在空间位置上与基期图斑层地类一致", count)
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
                                Description = string.Format("执行SQL【{0}】获取obj转换int失败", sql)
                            });
                        }
                    }
                }
            }

        }

    }
}
