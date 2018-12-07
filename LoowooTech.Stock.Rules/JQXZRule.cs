using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class JQXZRule:ArcGISBaseTool,Models.IRule
    {
        public override string RuleName { get { return "基期与现状数据对比检查"; } }
        public override string ID { get { return "2001"; } }
        public override CheckProject2 CheckProject { get { return CheckProject2.基期与现状数据对比检查; } }
        public bool Space { get { return true; } }


        public void Check()
        {
            Init2();
            if (System.IO.File.Exists(MdbFilePath) == false)
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
                    Description = string.Format("导入JQDLTB图层失败")
                });
                return;
            }
            

            if (System.IO.File.Exists(ParameterManager2.TDLYXZ) == true)
            {
                var in_feature = string.Format("{0}\\{1}", ParameterManager2.TDLYXZ, "XZQ");
                var array = ParameterManager2.XZCList.Select(e => string.Format("[XZQDM] = '{0}'", e.XZCDM)).ToArray();
                var where = string.Join(" OR ", array);
                if (ArcExtensions2.ImportFeatureClass(in_feature, MdbFilePath, "XZQ", where) == false)//首先导入行政区
                {
                    QuestionManager2.Add(new Question2
                    {
                        Code = ID,
                        Name = RuleName,
                        CheckProject = CheckProject,
                        Description = string.Format("导入行政区信息失败")
                    });
                }
                else
                {
                    var dltb = string.Format("{0}\\{1}", ParameterManager2.TDLYXZ, "DLTB");
                    var clip_feature = string.Format("{0}\\{1}", MdbFilePath, "XZQ");
                    var out_feature = string.Format("{0}\\{1}", MdbFilePath, "DLTB");
                    if (ArcExtensions2.Clip(dltb, clip_feature, out_feature, ParameterManager2.Tolerance) == false)
                    {
                        QuestionManager2.Add(new Question2
                        {
                            Code = ID,
                            Name = RuleName,
                            CheckProject = CheckProject,
                            Description = string.Format("通过行政区提取DLTB层失败")
                        });
                    }
                    else
                    {
                        var workspace = MdbFilePath.OpenAccessFileWorkSpace();
                        if (workspace == null)
                        {
                            QuestionManager2.Add(new Question2
                            {
                                Code = ID,
                                Name = RuleName,
                                CheckProject = CheckProject,
                                Description = string.Format("无法打开ArcGIS 工作空间")
                            });
                            return;
                        }
                        var tools = new List<AreaTool>();
                        tools.Add(new AreaTool { CheckFeatureClassName = "JQDLTB", CurrentFeatureClassName = "DLTB", Relative = ParameterManager2.Relative, Absolute = ParameterManager2.Absolute });
                        tools.Add(new AreaTool { CheckFeatureClassName = "JQDLTB", CheckWhereClause = "DLDM = '111' OR DLDM = '113'", CurrentFeatureClassName = "DLTB", CurrentWhereClause = "DLBM LIKE '01*'", Relative = ParameterManager2.Relative, Absolute = ParameterManager2.Absolute });
                        tools.Add(new AreaTool { CheckFeatureClassName = "JQDLTB", CheckWhereClause = "DLDM LIKE '21*'", CurrentFeatureClassName = "DLTB", CurrentWhereClause = "DLBM LIKE '05*' OR DLBM LIKE '06*' OR DLBM LIKE '07*' OR DLBM LIKE '08*' OR DLBM LIKE '09*' OR DLBM LIKE '10*' OR DLBM = '1109' OR DLBM DLBM = '1201'", Relative = ParameterManager2.Relative, Absolute = ParameterManager2.Absolute });

                        foreach(var tool in tools)
                        {
                            if (tool.Check(workspace) == false)
                            {
                                QuestionManager2.Add(new Question2
                                {
                                    Code = ID,
                                    Name = RuleName,
                                    CheckProject = CheckProject,
                                    Description = string.Format("执行检查【{0}】失败", tool.Name)
                                });
                            }

                            if (tool.Messages.Count > 0)
                            {
                                QuestionManager2.AddRange(tool.Messages.Select(e => new Question2
                                {
                                    Code = ID,
                                    Name = RuleName,
                                    CheckProject = CheckProject,
                                    Description = e.Description
                                }).ToList());
                            }
                        }
                    }

                }

            }
            else
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("土地利用现状数据库文件未指定，不做检查")
                });
            }
        }
    }
}
