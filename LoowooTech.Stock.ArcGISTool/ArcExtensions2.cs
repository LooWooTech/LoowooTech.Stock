using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.ConversionTools;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LoowooTech.Stock.ArcGISTool
{
    public static class ArcExtensions2
    {
        private static Geoprocessor _gp { get; set; }
        static ArcExtensions2()
        {
            _gp = new Geoprocessor();
        }

        public static bool Excute(IGPProcess tool)
        {
            try
            {
                _gp.Execute(tool, null);
            }catch(COMException comexc)
            {
                var error = string.Format("GP错误代码：{0}，详情：{1}", comexc.ErrorCode, comexc.Message);
                Console.WriteLine(error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 创建MDB文件
        /// </summary>
        /// <param name="folder">路径</param>
        /// <param name="name">mdb文件名</param>
        /// <returns></returns>
        public static bool CreateAccess(string folder,string name)
        {
            var tool = new CreatePersonalGDB();
            tool.out_folder_path = folder;
            tool.out_name = name;
            tool.out_version = "CURRENT";
            return Excute(tool);
        }



        public static bool ImportFeatureClass(string in_feature,string outpath,string outName,string whereClause)
        {
            var tool = new FeatureClassToFeatureClass();
            tool.in_features = in_feature;
            tool.out_path = outpath;
            tool.out_name = outName;
            tool.where_clause = whereClause;
            return Excute(tool);
        }

        public static bool Intersect(string in_features,string out_feature,string tolerance= "0.0001 METERS")
        {
            var tool = new Intersect();
            tool.in_features = in_features;
            tool.out_feature_class = out_feature;
            tool.join_attributes = "ALL";
            tool.output_type = "INPUT";
            tool.cluster_tolerance = tolerance;
            return Excute(tool);
        }

        public static bool Union(string in_features,string out_feature,string tolerance= "0.0001 METERS")
        {
            var tool = new Union();
            tool.in_features = in_features;
            tool.out_feature_class = out_feature;
            tool.join_attributes = "ALL";
            tool.cluster_tolerance = tolerance;
            return Excute(tool);
        }

        public static bool Select(string in_feature,string out_feature,string whereClause)
        {
            var tool = new Select();
            tool.in_features = in_feature;
            tool.out_feature_class = out_feature;
            tool.where_clause = whereClause;
            return Excute(tool);
        }

        public static bool Clip(string in_features,string clip_feature,string out_feature,string tolerance= "0.0001 METERS")
        {
            var tool = new ESRI.ArcGIS.AnalysisTools.Clip();
            tool.in_features = in_features;
            tool.clip_features = clip_feature;
            tool.out_feature_class = out_feature;
            tool.cluster_tolerance = tolerance;
            return Excute(tool);
        }

        public static bool Erase(string in_feature,string erase_feature,string out_feature,string tolerance= "0.0001 METERS")
        {
            var tool = new Erase();
            tool.in_features = in_feature;
            tool.erase_features = erase_feature;
            tool.out_feature_class = out_feature;
            tool.cluster_tolerance = tolerance;

            return Excute(tool);
        }
    }
}
