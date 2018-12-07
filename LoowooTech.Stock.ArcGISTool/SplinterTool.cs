using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.ArcGISTool
{
    public class SplinterTool
    {
        public string Name
        {
            get
            {
                var sb = new StringBuilder();
                if (string.IsNullOrEmpty(WhereClause) == false)
                {
                    sb.AppendFormat("当【{0}】时，", WhereClause);
                }
                sb.AppendFormat("要素类【{0}】中上图面积不小于【{1}】;",FeatureClassName,CompareValue);
                if (Absolute.HasValue)
                {
                    sb.AppendFormat("绝对容差：【{0}】", Absolute.Value);
                }

                return sb.ToString();
            }
        }
        public string FeatureClassName { get; set; }
        public string WhereClause { get; set; }
        
        public double? Relative { get; set; }
        public double? Absolute { get; set; }
        public double CompareValue { get; set; }
        private double MinValue
        {
            get
            {
                var a = CompareValue;
                if (Absolute.HasValue)
                {
                    a = a - Absolute.Value;
                }
                return a;
            }
        }
        public string Key { get; set; }
        public List<VillageMessage> Messages { get; set; } = new List<VillageMessage>();
        public bool Check(IWorkspace workspace)
        {
            if (workspace == null)
            {
                Messages.Add(new VillageMessage
                {
                    Description = "ArcGIS workspace 为null,无法检查" + Name
                });
                return false;
            }
            var featureClass = workspace.GetFeatureClass(FeatureClassName);
            if (featureClass == null)
            {

                return false;
            }

            var index = featureClass.Fields.FindField(Key);
            if (index <0)
            {

                return false;
            }
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = WhereClause;
            IFeatureCursor featureCursor = featureClass.Search(queryFilter, false);
            IFeature feature = featureCursor.NextFeature();
            while (feature != null)
            {
                var keyValue = feature.get_Value(index).ToString().Trim();
                if (string.IsNullOrEmpty(keyValue) == false)
                {
                    IArea area = feature.Shape as IArea;
                    if (area != null)
                    { 
                        if (area.Area < MinValue)
                        {
                            Messages.Add(new VillageMessage
                            {
                                Value = keyValue,
                                CheckValue = area.Area.ToString(),
                                Description = string.Format("【{0} ='{1}'】对应的数值不符合【{2}】", Key, keyValue, Name),
                                WhereClause = string.Format("[{0}] = '{1}'", Key, keyValue)
                            });
                        }
                    }
                    else
                    {
                        Messages.Add(new VillageMessage
                        {
                            Value = keyValue,
                            Description = string.Format("【{0} ='{1}'】转换IArea失败，未检查【{2}】", Key, keyValue, Name),
                            WhereClause = string.Format("[{0}] = '{1}'", Key, keyValue)
                        });
                    }
                }

                feature = featureCursor.NextFeature();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);
            return true;
        }
    }
}
