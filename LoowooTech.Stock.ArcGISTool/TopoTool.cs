using ESRI.ArcGIS.Geodatabase;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.ArcGISTool
{
    public class TopoTool:ArcGISBaseTool
    {
        public string Name
        {
            get
            {
                return string.Format("图层【{0}】重叠相交核查;",LayerName);
            }
        }
        public string LayerName { get; set; }
        private string _featureClassName
        {
            get
            {
                return string.Format("{0}_intersect", LayerName);
            }
        }
        public string Key { get; set; }
        

        public List<VillageMessage> Messages { get; set; } = new List<VillageMessage>();

        public bool Check(IWorkspace workspace)
        {
            IFeatureClass featureClass = workspace.GetFeatureClass(_featureClassName);
            if (featureClass == null)
            {
                Messages.Add(new VillageMessage
                {
                    Description = string.Format("获取要素类【{0}】失败", _featureClassName)
                });
                return false;
            }
            var fidName1 = string.Format("FID_{0}", LayerName);
            var fidName2 = string.Format("FID_{0}_1", LayerName);

            var key2 = string.Format("{0}_1", Key);
            var index1 = featureClass.Fields.FindField(Key);
            var index2 = featureClass.Fields.FindField(key2);
            if (index1 < 0 || index2 < 0)
            {
                Messages.Add(new VillageMessage
                {
                    Description = string.Format("在要素类【{0}】中未获取字段【{1}/{2}】的序号", _featureClassName, Key, key2)
                });
                return false;
            }

            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = string.Format("[{0}] <> [{1}]", fidName1, fidName2);
            IFeatureCursor featureCursor = featureClass.Search(queryFilter, false);
            IFeature feature = featureCursor.NextFeature();
            while (feature != null)
            {
                var keyValue1 = feature.get_Value(index1).ToString();
                var keyValue2 = feature.get_Value(index2).ToString();
                if (string.IsNullOrEmpty(keyValue1) == false && string.IsNullOrEmpty(keyValue2) == false)
                {
                    Messages.Add(new VillageMessage
                    {
                        Value = keyValue1,
                        Description = string.Format("【{0}】:{1} 与【{0}】：{2}存在重叠相交;", Key, keyValue1, keyValue2),
                        WhereClause = string.Format("[{0}] = '{1}'", Key, keyValue1)
                    });
                }

                feature = featureCursor.NextFeature();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);

            return true;
        }



    }
}
