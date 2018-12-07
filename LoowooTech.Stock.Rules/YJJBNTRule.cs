using ESRI.ArcGIS.Geodatabase;
using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class YJJBNTRule:ArcGISBaseTool,Models.IRule
    {
        public override string RuleName { get { return "永久基本农田一致性检查"; } }
        public override string ID { get { return "6001"; }  }
        public override CheckProject2 CheckProject { get { return CheckProject2.村规划数据库与乡规划数据库永久基本农田空间范围是否一致; } }
        public bool Space { get { return true; } }
        public void Check()
        {
            if (System.IO.File.Exists(ParameterManager2.XGH) == false)
            {
                return;
            }
            if (ExtractGHYT("GHYTDM = 'G111' OR GHYTDM = 'G112'") == false)
            {
                return;
            }
            if (ArcExtensions2.ImportFeatureClass(string.Format("{0}\\{1}", ParameterManager2.XGH, "GHYT"), MdbFilePath, "GHYT_X", "GHYTDM = 'G111' OR GHYTDM = 'G112'") == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("导入乡规划中规划用途为基本农田数据失败！")
                });
                return;
            }

            var sql = "SELECT XZQDM FROM GHYT GROUP BY XZQDM";
            var list = Search(MdbFilePath, sql);

            var list2 = new List<string>();
            foreach(var xzqdm in list)
            {
                if(ArcExtensions2.Select(string.Format("{0}\\{1}",MdbFilePath,"GHYT"),string.Format("{0}\\{1}_{2}",MdbFilePath,"GHYT",xzqdm),string.Format("XZQDM = '{0}'", xzqdm)) == false)
                {
                    QuestionManager2.Add(new Question2
                    {
                        Code = ID,
                        Name = RuleName,
                        CheckProject = CheckProject,
                        Description = string.Format("提取行政区代码【{0}】的村规划中永久基本农田数据失败", xzqdm)
                    });
                }
                else
                {
                    if(ArcExtensions2.Select(string.Format("{0}\\{1}",MdbFilePath,"GHYT_X"),string.Format("{0}\\{1}_{2}",MdbFilePath,"GHYT_X",xzqdm),string.Format("XZQDM = '{0}'", xzqdm)) == false)
                    {
                        QuestionManager2.Add(new Question2
                        {
                            Code = ID,
                            Name = RuleName,
                            CheckProject = CheckProject,
                            Description = string.Format("提取行政区代码【{0}】的乡规划中永久基本农田数据失败", xzqdm)
                        });
                    }
                    else
                    {
                        list2.Add(xzqdm);//成功提取行政区代码的村规划数据与乡数据

                        #region 核对范围
                        if (ArcExtensions2.Union(string.Format("{0}\\{1}_{2};{0}\\{1}_X_{2}", MdbFilePath, "GHYT", xzqdm), string.Format("{0}\\{1}_Union_{2}", MdbFilePath, "GHYT", xzqdm), ParameterManager2.Tolerance) == false)
                        {
                            QuestionManager2.Add(new Question2
                            {
                                Code = ID,
                                Name = RuleName,
                                CheckProject = CheckProject,
                                Description = string.Format("行政区代码【{0}】的村规划永久基本农田数据与乡规划永久基本农田数据Union失败",xzqdm)
                            });
                        }
                        else
                        {
                            var sql1 = string.Format("SELECT COUNT(*) FROM GHYT_Union_{0} WHERE FID_GHYT_{0} < 0 OR FID_GHYT_X_{0} < 0", xzqdm);
                            var obj = SearchRecord(MdbFilePath, sql1);
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
                                            Description = string.Format("行政区代码【{0}】的村规划永久基本农田范围与乡规划永久基本农田范围不一致",xzqdm)
                                        });
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }

            }

            CheckArea(list2, MdbFilePath, "6003", CheckProject2.村规划数据库与乡规划数据库永久基本农田面积是否一致,ParameterManager2.Relative,ParameterManager2.Absolute);




  

        }


        /// <summary>
        /// 核对面积
        /// </summary>
        /// <param name="list"></param>
        /// <param name="mdbfile"></param>
        /// <param name="id"></param>
        /// <param name="checkProject"></param>
        /// <param name="relative"></param>
        /// <param name="absolute"></param>
        private void CheckArea(List<string> list,string mdbfile,string id,CheckProject2 checkProject,double? relative,double? absolute)
        {
            IWorkspace workspace = mdbfile.OpenAccessFileWorkSpace();
            if (workspace == null)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = id,
                    Name = RuleName,
                    CheckProject = checkProject,
                    Description = string.Format("无法打开ArcGIS 工作空间")
                });
                return;
            }

            var questions = new List<Question2>();
            foreach(var xzqdm in list)
            {
                var cghyt = workspace.GetFeatureClass(string.Format("GHYT_{0}", xzqdm));
                if (cghyt == null)
                {
                    questions.Add(new Question2
                    {
                        Code = id,
                        Name = RuleName,
                        CheckProject = checkProject,
                        Description = string.Format("无法获取行政区代码【{0}】的村规划中永久基本农田的要素类", xzqdm)
                    });
                    continue;
                }
                var xghyt = workspace.GetFeatureClass(string.Format("GHYT_X_{0}", xzqdm));


                if (xghyt == null)
                {
                    questions.Add(new Question2
                    {
                        Code = id,
                        Name = RuleName,
                        CheckProject = checkProject,
                        Description = string.Format("无法获取行政区代码【{0}】的乡规划中永久基本农田的要素类", xzqdm)
                    });
                    continue;
                }

                var cArea = Math.Round(ArcClass.GainArea(cghyt), 0);
                var xArea = Math.Round(ArcClass.GainArea(xghyt), 0);
                var abs = Math.Abs(cArea - xArea);
                var flag = false;
                if (absolute.HasValue)
                {
                    flag = abs < absolute.Value;
                }
                if (relative.HasValue)
                {
                    var pp = abs / xArea;
                    flag = pp < relative.Value;
                }

                if (flag == false)
                {
                    questions.Add(new Question2
                    {
                        Code = id,
                        Name = RuleName,
                        CheckProject = checkProject,
                        Description = string.Format("行政区代码【{0}】的村规划永久基本农田面积与乡规划永久基本农田面积不一致", xzqdm)
                    });
                }
            }
            QuestionManager2.AddRange(questions);
        }
    }
}
