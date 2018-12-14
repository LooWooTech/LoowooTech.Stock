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
    public class AreaRule:ArcGISBaseTool, Models.IRule
    {
        public override string RuleName { get { return "面积一致性"; } }
        public override string ID { get { return "1304"; } }
        public bool Space { get { return true; } }
        public override CheckProject2 CheckProject { get { return CheckProject2.面积一致性; } }
        public void Check()
        {
            Init();
            if (System.IO.File.Exists(MdbFilePath) == false)
            {
                return;
            }
            #region  检查基期地类图斑地类面积=图斑面积-应扣田坎计算面积   应扣田坎计算面积=图斑面积*田坎系数
            var tool1 = new JQDLTBAreaTool { Relative = ParameterManager2.Relative, Absolute = ParameterManager2.Absolute };
            if (tool1.Check(ParameterManager2.Connection)==false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject2.面积一致性,
                    TableName = "JQDLTB",
                    Description = "检查基期地类图斑层相关数据失败"
                });
               
            }
            if (tool1.Messages.Count > 0)
            {
                QuestionManager2.AddRange(tool1.Messages.Select(e => new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject2.面积一致性,
                    TableName = "JQDLTB",
                    Description = e.Description,
                    LocationClause = e.WhereClause,
                }).ToList());
            }
            #endregion

            #region  规划用途净面积核对
            if(ArcExtensions2.Select(string.Format("{0}\\{1}",MdbFilePath,"GHYT"),string.Format("{0}\\{1}",MdbFilePath,"GHYT_JBNT"),"GHYTDM = 'G111' OR GHYTDM = 'G112'") == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("提取规划用途层中的基本农田数据失败")
                });
            }
            else
            {
                if (ArcExtensions2.Intersect(string.Format("{0}\\{1};{0}\\{2}", MdbFilePath, "GHYT_JBNT", "JQDLTB"), string.Format("{0}\\{1}", MdbFilePath, "GHYT_JBNT_Intersect"), ParameterManager2.Tolerance) == false)
                {
                    QuestionManager2.Add(new Question2
                    {
                        Code = ID,
                        Name = RuleName,
                        CheckProject = CheckProject,
                        Description = string.Format("规划用途层中的基本农田数据与基期地类图斑数据相交Intersect失败")
                    });
                }
                else
                {
                    CheckJMJ();
                }
            }

            #endregion

            IWorkspace workspace = MdbFilePath.OpenAccessFileWorkSpace();
            if (workspace == null)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = "无法打开ArcGIS 工作空间",
                    Folder = "",
                });
                return;
            }

            if (System.IO.File.Exists(ParameterManager2.TDLYXZ) == true)//判断土地利用现状数据库文件是否存在  存在 导入行政区
            {
                var in_feature = string.Format("{0}\\{1}", ParameterManager2.TDLYXZ, "XZQ");

                var tools = new List<ExtentTool>();
                foreach(var xzc in ParameterManager2.XZCList)
                {
                    var where = string.Format("XZQDM = '{0}'", xzc.XZCDM);
                    var extentLayerName = "XZQ_" + xzc.XZCDM;
                    if (ArcExtensions2.ImportFeatureClass(in_feature, MdbFilePath, extentLayerName, where) == false)//将指定行政区XZQ导入到
                    {
                        QuestionManager2.Add(new Question2
                        {
                            Code = ID,
                            Name = RuleName,
                            CheckProject = CheckProject,
                            Description = string.Format("导入行政区【{0}（{1}）】信息失败",xzc.XZCMC,xzc.XZCDM),
                            Folder = ""
                        });
                    }
                    else
                    {
                        foreach(var table in ParameterManager2.Tables)
                        {
                            var checkLayerName = string.Format("{0}_{1}", table.Name, xzc.XZCDM);//JSYDGZQ_xxxxxxxxxxxx
                            if (ArcExtensions2.Select(string.Format("{0}\\{1}", MdbFilePath, table.Name), string.Format("{0}\\{1}", MdbFilePath, checkLayerName), where) == false)
                            {
                                QuestionManager2.Add(new Question2
                                {
                                    Code = ID,
                                    Name = RuleName,
                                    CheckProject = CheckProject,
                                    Description = string.Format("提取图层【{0}】中行政区为【{1}】失败", table.Name, xzc.XZCDM)
                                });
                            }
                            else
                            {
                                var unionLayerName = string.Format("{0}_Union", checkLayerName);
                                if (ArcExtensions2.Union(string.Format("{0}\\{1};{0}\\{2}", MdbFilePath, checkLayerName, extentLayerName), string.Format("{0}\\{1}", MdbFilePath, unionLayerName), ParameterManager2.Tolerance) == false)
                                {
                                    QuestionManager2.Add(new Question2
                                    {
                                        Code = ID,
                                        Name = RuleName,
                                        CheckProject = CheckProject,
                                        Description = string.Format("执行图层【{0}】与图层【{1}】之间的Union操作失败", checkLayerName, extentLayerName)
                                    });
                                }
                                else
                                {
                                    tools.Add(new ExtentTool
                                    {
                                        CheckLayerName = checkLayerName,
                                        ExtentLayerName = extentLayerName,
                                        Absolute = ParameterManager2.Absolute,
                                        Relative = ParameterManager2.Relative
                                    });
                                }
                               
                            }
                        }
                    }
                }

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
            else
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("土地利用现状数据库文件未指定，不做行政区范围与面积核查")
                });
            }

            #region 建设用地管制区面积 公顷
            var tool2 = new JSYDGZQAreaTool { Relative = ParameterManager2.Relative, Absolute = ParameterManager2.Absolute };
            if (tool2.Check(workspace) == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject2.面积一致性,
                    TableName = "JSYDGZQ",
                    Description = "检查建设用地管制区面积公顷失败"
                });
            }

            if (tool2.Messages.Count > 0)
            {
                QuestionManager2.AddRange(tool2.Messages.Select(e => new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject2.面积一致性,
                    TableName = "JSYDGZQ",
                    Description = e.Description,
                    LocationClause = e.WhereClause
                }).ToList());
            }
            #endregion


        


        }

        private void CheckJMJ()
        {
            var questions = new List<Question2>();
            using (var connection = new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", MdbFilePath)))
            {
                connection.Open();
                var list = new List<GHYTJMJ>();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT BSM,MJ,JMJ FROM GHYT_JBNT";
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new GHYTJMJ
                        {
                            BSM = reader[0].ToString(),
                            MJ = Math.Round(double.Parse(reader[1].ToString()),0),
                            JMJ = Math.Round(double.Parse(reader[2].ToString()),0)
                        });
                    }
                }
                using(var command = connection.CreateCommand())
                {
                    foreach (var item in list)
                    {
                        var tt = new List<TKXSEntry>();
                        command.CommandText = "SELECT TKXS,Shape_Area FROM GHYT_JBNT_Intersect";
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            tt.Add(new TKXSEntry
                            {
                                TKXS = Math.Round(double.Parse(reader[0].ToString()),4),
                                Area = double.Parse(reader[1].ToString())
                            });
                        }
                        item.TKXSList = tt;

                        var sum = tt.Sum(e => e.Area);
                        var tkxs = .0;
                        foreach(var t in tt)
                        {
                            tkxs += t.Area / sum * t.TKXS;
                        }
                        tkxs = Math.Round(tkxs, 4);
                        var jmj = item.MJ * (1 - tkxs);
                        var abs = Math.Abs(jmj - item.JMJ);
                        var flag = false;
                        if (ParameterManager2.Absolute.HasValue)
                        {
                            flag = abs < ParameterManager2.Absolute.Value;
                        }
                        if (ParameterManager2.Relative.HasValue)
                        {
                            var pp = abs / jmj;
                            flag = pp < ParameterManager2.Relative.Value;
                        }
                        if (flag == false)
                        {
                            questions.Add(new Question2
                            {
                                Code = ID,
                                Name = RuleName,
                                CheckProject = CheckProject,
                                Description = string.Format("[BSM = '{0}'] 对应的净面积填写误差不符合容差", item.BSM),
                                LocationClause = string.Format("[BSM] = '{0}'", item.BSM)
                            });
                        }
                    }
                }
                connection.Close();
            }
            QuestionManager2.AddRange(questions);
          
        }


    }
}
