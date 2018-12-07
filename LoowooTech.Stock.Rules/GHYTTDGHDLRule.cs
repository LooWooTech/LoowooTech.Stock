using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class GHYTTDGHDLRule:ArcGISBaseTool,Models.IRule
    {
        public override string RuleName { get { return "规划用途和土地规划地类逻辑一致性检查"; } }
        public override string ID { get { return "3002"; } }
        public override CheckProject2 CheckProject { get { return CheckProject2.规划用途数据与土地规划地类数据地类逻辑一致性检查; } }
        public bool Space { get { return true; } }
        public void Check()
        {
            if (ExtractGHYT() == false)
            {
                return;
            }
            if (ArcExtensions2.ImportFeatureClass(string.Format("{0}\\{1}", ParameterManager2.MDBFilePath, "TDGHDL"), MdbFilePath, "TDGHDL", null) == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("导入土地规划地类图层失败")
                });
                return;
            }
            if (ArcExtensions2.Select(string.Format("{0}\\{1}",MdbFilePath,"GHYT"),string.Format("{0}\\{1}",MdbFilePath,"GHYT_XG"),"GHYTDM LIKE 'G*' OR GHYTDM LIKE 'X*'") == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("提取规划用途层中现状和新增数据失败！")
                });
                return;
            }

            var sql = "SELECT MID(GHYTDM,2) FROM GHYT_XG GROUP BY MID(GHYTDM,2)";
            var list = Search(MdbFilePath, sql);
            if (list.Count == 0)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("执行SQL[{0}]获取结果为空", sql)
                });
                return;
            }
            var workspace = MdbFilePath.OpenAccessFileWorkSpace();
            if (workspace == null)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("打开ArcGIS 工作空间失败！")
                });
                return;
            }
            var ghytFeatureClass = workspace.GetFeatureClass("GHYT_XG");
            if (ghytFeatureClass == null)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("获取要素类【GHYT_XG】失败！")
                });
                return;
            }
            var tdghdlFeatureClass = workspace.GetFeatureClass("TDGHDL");
            if (tdghdlFeatureClass == null)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("获取要素类【TDGHDL】失败！")
                });
                return;
            }
            var messages = new List<string>();
            foreach(var item in list)
            {
                var ghytArea = Math.Round(ArcClass.GainArea(ghytFeatureClass, string.Format("GHYTDM = 'X{0}' OR GHYTDM = 'G{0}'", item)), 2);
                var tdghdlArea = Math.Round(ArcClass.GainArea(tdghdlFeatureClass, string.Format("GHDLDM = '{0}'", item)), 2);
                var abs = Math.Abs(ghytArea - tdghdlArea);
                var flag = false;
                if (ParameterManager2.Absolute.HasValue)
                {
                    flag = abs < ParameterManager2.Absolute.Value;
                }
                if (ParameterManager2.Relative.HasValue)
                {
                    var pp = abs / tdghdlArea;
                    flag = pp < ParameterManager2.Relative.Value;
                }
                if (flag == false)
                {
                    messages.Add(string.Format("规划用途中规划用途代码为【X{0}】+【G{0}】的面积和与土地规划地类层中地类面积和不相等", item));
                }

            }
            QuestionManager2.AddRange(messages.Select(e => new Question2
            {
                Code = ID,
                Name = RuleName,
                CheckProject = CheckProject,
                Description = e
            }).ToList());
      

        }
    }
}
