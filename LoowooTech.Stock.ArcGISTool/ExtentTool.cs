using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.ArcGISTool
{
    public class ExtentTool
    {
        public string Name
        {
            get
            {
                var sb = new StringBuilder(string.Format("图层【{0}】所在行政区代码【{1}】的范围与面积核对;", _tableName, _XZQDM));
                if (Relative.HasValue)
                {
                    sb.AppendFormat("相对容差【{0}%】;", Relative.Value * 100);
                }
                if (Absolute.HasValue)
                {
                    sb.AppendFormat("绝对容差【{1}】;", Absolute.Value);
                }
                return sb.ToString();
            }
        }
        public string CheckLayerName { get; set; }

        private string[] _array { get { return CheckLayerName.Split('_'); } }
        private string _tableName { get { return _array[0]; } }
        private string _XZQDM { get { return _array[1]; } }
        private string UnionLayerName { get; set; }
        public string ExtentLayerName { get; set; }

        public double? Absolute { get; set; }
        public double? Relative { get; set; }

        public List<VillageMessage> Messages { get; set; } = new List<VillageMessage>();

        public bool Check(IWorkspace workspace)
        {
            IFeatureClass UnionFeatureClass = workspace.GetFeatureClass(UnionLayerName);
            if (UnionFeatureClass != null)
            {
                CheckUnion(UnionFeatureClass);
            }
            else
            {
                Messages.Add(new VillageMessage
                {
                    Description = string.Format("未获取Union图层，无法进行范围核对")
                });
            }
            IFeatureClass CheckFeatureClass = workspace.GetFeatureClass(CheckLayerName);
            IFeatureClass ExtentFeatureClass = workspace.GetFeatureClass(ExtentLayerName);
            if (CheckFeatureClass == null)
            {
                Messages.Add(new VillageMessage
                {
                    Description = string.Format("无法获取图层【{0}】要素类,未检查面积一致性", CheckLayerName)
                });
                return false;
            }
            if (ExtentFeatureClass == null)
            {
                Messages.Add(new VillageMessage
                {
                    Description = string.Format("无法获取图层【{0}】要素类，未检查面积一致性", ExtentLayerName)
                });

                return false;
            }
            var checkArea = Math.Round(ArcClass.GainArea(CheckFeatureClass), 0);
            var extentArea = Math.Round(ArcClass.GainArea(ExtentFeatureClass), 0);
            var abs = Math.Abs(checkArea - extentArea);
            var flag = false;
            if (Absolute.HasValue)
            {
                flag = abs < Absolute.Value;
            }
            if (Relative.HasValue)
            {
                var pp = abs / extentArea;
                flag = pp < Relative.Value;
            }

            if (flag == false)
            {
                Messages.Add(new VillageMessage
                {
                    CheckValue=abs.ToString(),
                    Description = string.Format("图层【{0}】所在行政区代码【{1}】内面积不一致；",_tableName, _XZQDM)
                });
            }
            return true;
        }

    
        private void CheckUnion(IFeatureClass featureClass)
        {
            var fid1 = string.Format("FID_{0}", CheckLayerName);
            var fid2 = string.Format("FID_{0}", ExtentLayerName);
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = string.Format("[{0}] < 0 OR [{1}] < 0", fid1, fid2);
            IFeatureCursor featureCursor = featureClass.Search(queryFilter, false);
            ICursor cursor = featureCursor as ICursor;
            IDataStatistics dataStatistics = new DataStatisticsClass();
            dataStatistics.Cursor = cursor;
            dataStatistics.Field = fid1;
            IStatisticsResults statisticsResults = dataStatistics.Statistics;
            if (statisticsResults.Count > 0)
            {
                Messages.Add(new VillageMessage
                {
                    Description = string.Format("图层【{0}】中行政区代码【{1}】范围不符",_tableName,_XZQDM)
                });
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);

        }


        
    }
}
