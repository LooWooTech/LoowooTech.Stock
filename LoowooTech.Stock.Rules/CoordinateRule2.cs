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
    public class CoordinateRule2:ArcGISBaseTool, Models.IRule
    {
        public override string RuleName { get { return "数学基础"; } }
        public override string ID { get { return "1202"; } }
        public override CheckProject2 CheckProject { get { return CheckProject2.数学基础; } }
        public bool Space { get { return true; } }

        public void Check()
        {
            Init();
            if (System.IO.File.Exists(MdbFilePath) == false)
            {
                return;
            }
            IWorkspace workspace = MdbFilePath.OpenAccessFileWorkSpace();
            if (workspace == null)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Folder = "",
                    Description = string.Format("无法打开矢量数据的workspace,故无法检查规则")
                });
                return;
            }
            var tables = ParameterManager2.Tables.Where(e => e.IsSpace == true).Select(e => e.Name).ToList();
            var results = ArcGISManager.CheckCoordinate2(workspace, tables);
            if (results.Count > 0)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("【{0}】不符合坐标系", string.Join("、", results.ToArray())),
                    Folder = ""
                });
            }
        }
    }
}
