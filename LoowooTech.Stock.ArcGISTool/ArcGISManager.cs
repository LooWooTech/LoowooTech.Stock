﻿using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessor;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.ArcGISTool
{
    public static class ArcGISManager
    {
        public static void CheckCoordinate(string className,string ruleName)
        {
            var info = string.Empty;
            var featureClass = ParameterManager.Workspace.GetFeatureClass(className);
            if (featureClass != null)//本工具只对存在的要素类进行坐标系核对，不存在不做坐标系核对操作
            {
                var spatialReference = SpatialReferenceManager.GetSpatialReference(featureClass);
                if (spatialReference.Name.Trim() != ParameterManager.CurrentSpatialReference.Name.Trim())
                {
                    info = string.Format("图层：{0}不符合2201（平面坐标系是否采用‘1980 西安坐标系’、3度带、带带号，检查高程系统是否采用‘1985 国家高程基准’，检查投影方式是否采用高斯-克吕格投影）", className);
                    QuestionManager.Add(new Question { Code = "2201", Name = ruleName, Project = CheckProject.数学基础, TableName = className, Description = info });
                }
            }
        }
        public static void Topo(string className)
        {
            var featurePath = string.Format("{0}/{1}", ParameterManager.MDBFilePath, className);
            var outFeatureName = string.Format("{0}_intersect", className);
            var intersectPath = string.Format("{0}/{1}", ParameterManager.MDBFilePath, outFeatureName);
            if (Cross(string.Format("{0};{1}", featurePath, featurePath), intersectPath))
            {
                var intersectfeatureClass = ParameterManager.Workspace.GetFeatureClass(outFeatureName);
                if (intersectfeatureClass == null)
                {
                    QuestionManager.Add(new Question { Code = "4101", Name = "拓扑关系", Project = CheckProject.拓扑关系, Description = "无法获取拓扑结果" });
                }
                else
                {
                    Run(intersectfeatureClass, className, "XZCMC", "TBBH");
                }
            }
            else
            {
                QuestionManager.Add(new Question { Code = "4101", Name = "拓扑关系", Project = CheckProject.拓扑关系, Description = "执行intersect发生错误" });
            }
        }
        private static bool DeleteFeatureClass(string in_feature)
        {
            Delete tool = new Delete();
            tool.in_data = in_feature;
            return Excute(tool);

        }
        private static bool Excute(IGPProcess tool)
        {
            Geoprocessor gp = new Geoprocessor();
            gp.OverwriteOutput = true;
            try
            {
                gp.Execute(tool, null);
            }
            catch
            {
                object sev1 = 2;
                gp.AddError(gp.GetMessages(ref sev1));
                return false;
            }
            return true;
        }

        private static bool Cross(string in_features, string out_feature)
        {
            Intersect tool = new Intersect();
            tool.in_features = in_features;
            tool.out_feature_class = out_feature;
            tool.join_attributes = "ALL";
            tool.output_type = "INPUT";
            return Excute(tool);
        }
        private static void Run(IFeatureClass featureClass, string fidName, string titleName1, string titleName2)
        {
            var fid1 = featureClass.Fields.FindField(string.Format("FID_{0}", fidName));
            var fid2 = featureClass.Fields.FindField(string.Format("FID_{0}_1", fidName));

            var title11 = featureClass.Fields.FindField(titleName1);
            var title12 = featureClass.Fields.FindField(titleName1 + "_1");

            var title21 = featureClass.Fields.FindField(titleName2);
            var title22 = featureClass.Fields.FindField(titleName2 + "_1");


            IFeatureCursor featureCursor = featureClass.Search(null, false);
            IFeature feature = featureCursor.NextFeature();

            while (feature != null)
            {
                try
                {
                    var valOne = feature.get_Value(fid1);
                    var valTwo = feature.get_Value(fid2);
                    if (valOne != valTwo)//存在不相等，则表示存在相交
                    {
                        var val = feature.get_Value(title11).ToString();
                        var str1 = string.Format("{0}:【{1}】--{2}：【{3}】存在图斑相交", titleName1, val, titleName2, feature.get_Value(title21));
                        QuestionManager.Add(new Question { Code = "4101", Name = "拓扑关系", Project = CheckProject.拓扑关系, TableName = fidName, BSM = val, Description = str1 });
                    }
                }
                catch (Exception ex)
                {
                    var error = string.Format("获取对比的FID组发生错误,错误信息：{0}", ex.Message);
                    QuestionManager.Add(new Question { Code = "4101", Name = "拓扑关系", Project = CheckProject.拓扑关系, Description = error });
                }


                feature = featureCursor.NextFeature();
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);
        }
    }
}