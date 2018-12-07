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
    public class TopologyRule2:ArcGISBaseTool, Models.IRule
    {
        public override string RuleName { get { return "拓扑关系"; } }
        public override string ID { get { return "1401"; } }
        public override CheckProject2 CheckProject { get { return CheckProject2.拓扑关系; } }
        public bool Space { get { return true; } }
        public void Check()
        {
            Init();
            if (System.IO.File.Exists(MdbFilePath) == false)
            {
                return;
            }
            var layers = new string[] {
                "JQDLTB",
                "GHYT",
                "TDGHDL",
                "JSYDGZQ"
            };
            IWorkspace workspace = MdbFilePath.OpenAccessFileWorkSpace();
            if (workspace == null)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Folder = "",
                    Description = string.Format("无法打开矢量数据的workspace,故无法质检")
                });
                return;
            }

            var tools = new List<TopoTool>();
            foreach (var layer in layers)
            {
                var path = string.Format("{0}\\{1}", MdbFilePath, layer);
                var in_features = string.Format("{0};{1}", path, path);
                var intersectName = string.Format("{0}_intersect", layer);
                var out_feature = string.Format("{0}\\{1}", MdbFilePath, intersectName);
                if (ArcExtensions2.Intersect(in_features, out_feature,ParameterManager2.Tolerance) == false)
                {
                    QuestionManager2.Add(new Question2
                    {
                        Code = ID,
                        Name = RuleName,
                        CheckProject = CheckProject,
                        Description = string.Format("图层【{0}】执行Intersect失败", layer)
                    });
                }else
                {
                    tools.Add(new TopoTool { LayerName = layer, Key = "BSM" });
                }
            }
            foreach(var tool in tools)
            {
                if (tool.Check(workspace)==false)
                {
                    QuestionManager2.Add(new Question2
                    {
                        Code = ID,
                        Name = RuleName,
                        CheckProject = CheckProject,
                        Description = string.Format("执行【{0}】失败", tool.Name)
                    });
                }
                if (tool.Messages.Count > 0)
                {
                    QuestionManager2.AddRange(tool.Messages.Select(e => new Question2
                    {
                        Code = ID,
                        Name = RuleName,
                        CheckProject = CheckProject,
                        Description = e.Description,
                        LocationClause=e.WhereClause
                    }).ToList());
                }
            }

           


        }


    
    }
}
