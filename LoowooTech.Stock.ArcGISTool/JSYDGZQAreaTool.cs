using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.ArcGISTool
{
    public class JSYDGZQAreaTool
    {
        public string Name { get { return "建设用地管制区面积为公顷"; } }
        private string _featureClassName { get { return "JSYDGZQ"; } }
        private string _checkAreaFieldName { get { return "GZQMJ"; } }
        private string _key { get { return "BSM"; } }
        public List<VillageMessage> Messages { get; set; } = new List<VillageMessage>();

        /// <summary>
        /// 百分比
        /// </summary>
        public double? Relative { get; set; }
        public double? Absolute { get; set; }
        public bool Check(IWorkspace workspace)
        {
            if (workspace == null)
            {
                Messages.Add(new VillageMessage
                {
                    Description = "未获取ArcGIS Workspace工作空间"
                });
                return false;
            }
            var featureClass = workspace.GetFeatureClass(_featureClassName);
            if (featureClass == null)
            {
                Messages.Add(new VillageMessage
                {
                    Description = "未获取要素类：" + _featureClassName
                });
                return false;
            }
            var index = featureClass.Fields.FindField(_checkAreaFieldName);
            var keyIndex = featureClass.Fields.FindField(_key);
            if (index < 0)
            {
                Messages.Add(new VillageMessage
                {
                    Description = "未获取核对面积字段序号：" + _checkAreaFieldName
                });
                return false;
            }
            if (keyIndex < 0)
            {
                Messages.Add(new VillageMessage
                {
                    Description = "未获取字段序号：" + _key
                });
                return false;
            }
            IFeatureCursor featureCursor = featureClass.Search(null, false);
            IFeature feature = featureCursor.NextFeature();
            var a = .0;
            while (feature != null)
            {
                var keyValue = feature.get_Value(keyIndex).ToString();
                if (string.IsNullOrEmpty(keyValue) == false)
                {
                    IArea area = feature.Shape as IArea;
                    var currentArea = Math.Round(area.Area / 10000, 2);
                    var mjString = feature.get_Value(index).ToString();
                    if (double.TryParse(mjString, out a))
                    {
                        a = Math.Round(a, 2);//管制区面积填写的值
                        var abs = Math.Abs(currentArea-a);
                        var flag = false;
                        if (Relative.HasValue)
                        {
                            var pp = abs / currentArea;
                            flag = pp < Relative.Value;
                        }

                        if (Absolute.HasValue)
                        {
                            flag = abs < Absolute.Value;
                        }

                        if (flag == false)
                        {
                            Messages.Add(new VillageMessage
                            {
                                Value = keyValue,
                                CheckValue = string.Format("实际计算面积为【{0}】，填写数值为【{1}】", currentArea, a),
                                Description = string.Format("【{0}={1}】对应的数值不符合【{2}】", "BSM", keyValue, Name),
                                WhereClause = string.Format("[{0}] = '{1}'", "BSM", keyValue)
                            });
                        }
                    }
                    else
                    {
                        Messages.Add(new VillageMessage
                        {
                            Value = keyValue,
                            CheckValue = mjString,
                            Description = string.Format("【{0}={1}】对应的数值获取double类型失败", "BSM", keyValue),
                            WhereClause = string.Format("[{0}] = '{1}'", "BSM", keyValue)
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
