using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.ArcGISTool
{
    /// <summary>
    /// 作用：核对坐标系 本工具只对存在的要素类进行坐标系核对，不存在不做坐标系核对操作
    /// </summary>
    public class CoordinateRule: Models.IRule
    {
        public string RuleName { get; set; }
        public string ID { get; set; }
        
        public void Check()
        {
            var info = string.Empty;
            foreach(var className in ParameterManager.FeatureClassNames)
            {
                var featureClass =ParameterManager.Workspace.GetFeatureClass(className);
                if (featureClass != null)//本工具只对存在的要素类进行坐标系核对，不存在不做坐标系核对操作
                {
                    var spatialReference = SpatialReferenceManager.GetSpatialReference(featureClass);
                    if (spatialReference.Name.Trim() != ParameterManager.CurrentSpatialReference.Name.Trim())
                    {
                        info= string.Format("图层：{0}不符合2201（平面坐标系是否采用‘1980 西安坐标系’、3度带、带带号，检查高程系统是否采用‘1985 国家高程基准’，检查投影方式是否采用高斯-克吕格投影）", className);
                        QuestionManager.Add(new Question { Code = "2201", Name = RuleName, Project = CheckProject.数学基础, TableName = className, Description = info });
                    }
                }
            }
        }
    }
}
