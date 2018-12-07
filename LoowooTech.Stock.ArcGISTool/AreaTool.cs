using ESRI.ArcGIS.Geodatabase;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.ArcGISTool
{
    public class AreaTool
    {
        public string Name
        {
            get
            {
                var sb = new StringBuilder(string.Format("图层【{0}】与图层【{1}】面积一致;", CheckFeatureClassName, CurrentFeatureClassName));

                return sb.ToString();
            }
        }
        public string CheckWhereClause { get; set; }
        public string CurrentWhereClause { get; set; }
        public string CheckFeatureClassName { get; set; }
        public string CurrentFeatureClassName { get; set; }
        public double? Absolute { get; set; }
        public double? Relative { get; set; }
        public List<VillageMessage> Messages { get; set; } = new List<VillageMessage>();
        public bool Check(IWorkspace workspace)
        {
            IFeatureClass checkFeatureClass = workspace.GetFeatureClass(CheckFeatureClassName);
            if (checkFeatureClass == null)
            {
                Messages.Add(new VillageMessage
                {
                    Description = string.Format("未获取要素类【{0}】,无法进行核查【{1}】", CheckFeatureClassName, Name)
                });
                return false;
            }
            IFeatureClass currentFeatureClass = workspace.GetFeatureClass(CurrentFeatureClassName);
            if (currentFeatureClass == null)
            {
                Messages.Add(new VillageMessage
                {
                    Description = string.Format("未获取要素类【{0}】，无法进行核查【{1}】", CurrentFeatureClassName, Name)
                });
                return false;
            }
            var checkArea =Math.Round(ArcClass.GainArea(checkFeatureClass, CheckWhereClause),0);
            var currentArea = Math.Round(ArcClass.GainArea(currentFeatureClass, CurrentWhereClause), 0);
            var abs = Math.Abs(checkArea - currentArea);
            var flag = false;
            if (Absolute.HasValue)
            {
                flag = abs < Absolute.Value;
            }
            if (Relative.HasValue)
            {
                var pp = abs / currentArea;
                flag = pp < Relative.Value;
            }
            if (flag == false)
            {
                Messages.Add(new VillageMessage
                {
                    Value = abs.ToString(),
                    Description = string.Format("【{0}】不符合", Name)
                });
            }
            return true;
        }
    }
}
