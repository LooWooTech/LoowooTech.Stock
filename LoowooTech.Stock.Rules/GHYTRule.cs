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
    public class GHYTRule:ArcGISBaseTool,Models.IRule
    {
        public override string RuleName { get { return "拆旧与建新数据对比检查"; } }
        public override string ID { get { return "3004"; } }
        public override CheckProject2 CheckProject { get { return CheckProject2.拟拆旧面积与村规划拟建新面积对比检查; } }
        public bool Space { get { return true; } }
        public void Check()
        {
            if (ExtractGHYT() == false)
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
                    Description = string.Format("无法打开ArcGIS 工作空间")
                });
                return;
            }
            IFeatureClass featureClass = workspace.GetFeatureClass("GHYT");
            if (featureClass == null)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("获取GHYT要素类失败")
                });
                return;
            }
            var CArea = Math.Round(ArcClass.GainArea(featureClass, "CJJX = 'C1' OR CJJX = 'C2'"), 0);
            var E2Area = Math.Round(ArcClass.GainArea(featureClass, "CJJX = 'E2'"), 0);
            if (CArea < E2Area)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("拆旧区面积不符合大于等于村规划拟新建区面积")
                });
            }
        }
    }
}
