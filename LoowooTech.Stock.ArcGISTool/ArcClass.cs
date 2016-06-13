using ESRI.ArcGIS.ConversionTools;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.ArcGISTool
{
    public static class ArcClass
    {
        public static IWorkspace GetmdbWorkspace(string mdbFilePath)
        {
            AccessWorkspaceFactory accessWorkspaceFactory = new AccessWorkspaceFactory();
            IWorkspace workspace = accessWorkspaceFactory.OpenFromFile(mdbFilePath, 0);
            return workspace;
        }

        public static IWorkspace GetShapefileWorkspace(string shapefilePath)
        {
            IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactory();
            IWorkspace workspace = workspaceFactory.OpenFromFile(shapefilePath, 0);
            return workspace;
        }
        /// <summary>
        /// 获取要素类
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="featureClassName"></param>
        /// <returns></returns>
        public static IFeatureClass GetFeatureClass(IWorkspace workspace,string featureClassName)
        {
            IFeatureWorkspace featureworkspace = workspace as IFeatureWorkspace;
            IFeatureClass featureClass = null;
            try
            {
                featureClass = featureworkspace.OpenFeatureClass(featureClassName);
            }catch(Exception ex)
            {
                throw new Exception(string.Format("获取要素类{0}失败，错误信息：{1}", featureClassName, ex.Message));
            }

            return featureClass;
        }
        /// <summary>
        /// 获取要素集
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="featureDatasetName"></param>
        /// <returns></returns>
        public static IFeatureDataset GetFeatureDataset(IWorkspace workspace,string featureDatasetName)
        {
            IFeatureWorkspace featureWorkspace = workspace as IFeatureWorkspace;
            IFeatureDataset featureDataset = null;
            try
            {
                featureDataset = featureWorkspace.OpenFeatureDataset(featureDatasetName);
            }catch(Exception ex)
            {
                throw new Exception(string.Format("获取要素集{0}失败，错误信息：{1}", featureDatasetName, ex.Message));
            }
            return featureDataset;
        }

        /// <summary>
        /// 创建要素类
        /// </summary>
        /// <param name="featureWorkspace"></param>
        /// <param name="name"></param>
        /// <param name="esriGeometryType"></param>
        /// <param name="fielddict"></param>
        /// <returns></returns>
        public static IFeatureClass CreateFeatureClass(IFeatureWorkspace featureWorkspace, string name, esriGeometryType esriGeometryType, Dictionary<string, GISField> fielddict)
        {
            IFields fields = new FieldsClass();
            IFieldsEdit fieldsedit = fields as IFieldsEdit;
            IField field = new FieldClass();
            IFieldEdit fieldedit = field as IFieldEdit;
            fieldedit.Name_2 = "shape";
            fieldedit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            IGeometryDef geometryDef = new GeometryDefClass();
            IGeometryDefEdit pGeometryDefEdit = geometryDef as IGeometryDefEdit;
            pGeometryDefEdit.GeometryType_2 = esriGeometryType;

            ISpatialReferenceFactory pSpatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            ISpatialReference pSpatialReference = pSpatialReferenceFactory.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
            pSpatialReference.SetDomain(-6000000, 6000000, -6000000, 6000000);
            pGeometryDefEdit.SpatialReference_2 = pSpatialReference;
            fieldedit.GeometryDef_2 = geometryDef;
            fieldsedit.AddField(field);

            field = new FieldClass();
            fieldedit = field as IFieldEdit;
            fieldedit.Name_2 = "FID";
            fieldedit.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldsedit.AddField(field);

            foreach (var entry in fielddict)
            {
                if (entry.Key.ToUpper() == "SHAPE" || entry.Key.ToUpper() == "OBJECTID")
                {
                    continue;
                }
                field = new FieldClass();
                fieldedit = field as IFieldEdit;
                fieldedit.Name_2 = entry.Key;
                fieldedit.Type_2 = entry.Value.Type;
                fieldsedit.AddField(field);
            }


            IFeatureClassDescription featureClassDescription = new FeatureClassDescriptionClass();
            IObjectClassDescription objectClassDescription = featureClassDescription as IObjectClassDescription;
            IFeatureClass featureClass = featureWorkspace.CreateFeatureClass(name, fields, objectClassDescription.InstanceCLSID, objectClassDescription.ClassExtensionCLSID, esriFeatureType.esriFTSimple, "shape", "");
            return featureClass;
        }
        /// <summary>
        /// 创建要素集
        /// </summary>
        /// <param name="featureWorkspace"></param>
        /// <param name="featureDataName"></param>
        /// <returns></returns>
        public static IFeatureDataset CreateFeatureDataset(IFeatureWorkspace featureWorkspace,string featureDataName)
        {
            ISpatialReferenceFactory pSpatialReferebceFactory = new SpatialReferenceEnvironmentClass();
            ISpatialReference pSpatialReference = pSpatialReferebceFactory.CreateProjectedCoordinateSystem((int)esriSRProjCS4Type.esriSRProjCS_Xian1980_3_Degree_GK_Zone_40);
            IFeatureDataset featureDataset = null;
            try
            {
                featureDataset = featureWorkspace.CreateFeatureDataset(featureDataName, pSpatialReference);
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("创建要素集{0}失败，错误信息：{1}", featureDataName, ex.Message));
            }
            return featureDataset;
        }

        public static bool ImportFeatureClass(IFeatureClass inputFeatureClass,IFeatureClass outputFeatureClass,string outputName)
        {
            Geoprocessor gp = new Geoprocessor();
            FeatureClassToFeatureClass tool = new FeatureClassToFeatureClass();
            tool.in_features = inputFeatureClass;
            tool.out_path = "";
            tool.out_name = outputName;
            try
            {
                gp.Execute(tool, null);
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("导入要素类失败，错误信息：{0}", ex.Message));
            }

            return true;
        }
    }
}
