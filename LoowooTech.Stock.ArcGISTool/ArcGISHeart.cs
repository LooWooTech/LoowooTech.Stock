using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessor;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.ArcGISTool
{
    public class ArcGISHeart
    {
        private const string TABLENAME = "DCDYTB";
        private string _mdbFilePath { get; set; }
        public string MDBFilePath { get { return _mdbFilePath; }set { _mdbFilePath = value; } }
        private List<string> _featureClassNames { get; set; }//mdb中的featureClass系统中要求的
        /// <summary>
        /// MDB文件中的要求的图层名称
        /// </summary>
        public List<string> FeatureClassNames { get { return _featureClassNames; }set { _featureClassNames = value; } }
        private IWorkspace _workspace { get; set; }
        private List<string> _messages { get; set; }
        private List<Question> _questions { get; set; }
        private ISpatialReference _currentSpatialReference { get; set; }
        public ISpatialReference CurrentSpatialReference { get { return _currentSpatialReference == null ? _currentSpatialReference = SpatialReferenceManager.Get40SpatialReference() : _currentSpatialReference; } }

        public ArcGISHeart()
        {
            _messages = new List<string>();
            _questions = new List<Question>();
        }

        public void Program()
        {
            _workspace = MDBFilePath.OpenAccessFileWorkSpace();
            if (_workspace == null)
            {
                _messages.Add("获取Access的workspace失败");
                return;
            }
            var str = string.Empty;
            foreach(var className in FeatureClassNames)
            {
                
                var featureClass = _workspace.GetFeatureClass(className);
                if (featureClass == null)
                {
                    str = string.Format("未获取图层：{0}，无法进行图层相关检查", className);
                    _messages.Add(str);
                    _questions.Add(new Question() { Code = "2101", Name = "图层完整性", Project = CheckProject.图层完整性, TableName = className, Description = str });
                    continue;
                }
                var spatialReference = SpatialReferenceManager.GetSpatialReference(featureClass);//检查图层坐标系
                if (spatialReference.Name.Trim() != CurrentSpatialReference.Name.Trim())
                {
                    str = string.Format("图层：{0}不符合2201（平面坐标系是否采用‘1980 西安坐标系’、3度带、带带号，检查高程系统是否采用‘1985 国家高程基准’，检查投影方式是否采用高斯-克吕格投影）",className);
                    _messages.Add(str);
                    _questions.Add(new Question { Code = "2201", Name = "数学基础", Project = CheckProject.数学基础, TableName = className, Description = str });
                }

                if (className == TABLENAME)//检查DCDYTB中是否存在相交
                {
                    var featurePath = string.Format("{0}/{1}", _mdbFilePath, className);
                    var outFeatureName = string.Format("{0}_intersect", className);
                    var intersectPath = string.Format("{0}/{1}", _mdbFilePath, outFeatureName);
                    if (Cross(string.Format("{0};{1}", featurePath, featurePath), intersectPath))
                    {
                        var intersectfeatureClass = _workspace.GetFeatureClass(outFeatureName);
                        if (intersectfeatureClass == null)
                        {
                            _messages.Add(string.Format("无法获取要素类:【{0}】,故无法验证图斑是否相交"));

                        }
                        else
                        {
                            Run(intersectfeatureClass, TABLENAME, "XZCMC", "TBBH");
                            DeleteFeatureClass(intersectPath);
                        }

                    }
                    else
                    {
                        _messages.Add(string.Format("验证表【{0}】中是否存在图斑相交，执行intersect发生错误"));
                    }

                }

            }

            QuestionManager.AddRange(_questions);
        }

        private bool Excute(IGPProcess tool)
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

        private bool Cross(string in_features,string out_feature)
        {
            Intersect tool = new Intersect();
            tool.in_features = in_features;
            tool.out_feature_class = out_feature;
            tool.join_attributes = "ALL";
            tool.output_type = "INPUT";
            return Excute(tool);
        }

        private bool DeleteFeatureClass(string in_feature)
        {
            Delete tool = new Delete();
            tool.in_data = in_feature;
            return Excute(tool);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="fidName"></param>
        /// <param name="titleName"></param>
        private void Run(IFeatureClass featureClass,string fidName,string titleName1,string titleName2)
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
                        Console.WriteLine(str1);
                        _messages.Add(str1);
                        _questions.Add(new Question { Code = "4101", Name = "拓扑关系", Project = CheckProject.拓扑关系, TableName = fidName, BSM = val, Description = str1 });
                    }
                }
                catch(Exception ex)
                {
                    var error = string.Format("获取对比的FID组发生错误,错误信息：{0}", ex.Message);
                    Console.WriteLine(error);
                    _messages.Add(error);
                }
           
                
                feature = featureCursor.NextFeature();
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);
        }
    }
}
