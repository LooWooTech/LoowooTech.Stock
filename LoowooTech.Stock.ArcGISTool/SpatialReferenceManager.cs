using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.ArcGISTool
{
    public static class SpatialReferenceManager
    {
        public static ISpatialReference CreateSpatialReference(this string strProFile)
        {
            ISpatialReferenceFactory pSpatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            ISpatialReference pSpatialReference = pSpatialReferenceFactory.CreateESRISpatialReferenceFromPRJFile(strProFile);
            return pSpatialReference;
        }

        public static ISpatialReference CreateGeographicCoordinate(esriSRProjCS4Type gcsType)
        {
            ISpatialReferenceFactory pSpatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            ISpatialReference pSpatialReference = pSpatialReferenceFactory.CreateGeographicCoordinateSystem((int)gcsType);
            return pSpatialReference;
        }

        public static ISpatialReference CreateProjectedCoordinate(esriSRProjCS4Type pcsType)
        {
            ISpatialReferenceFactory2 pSpatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            ISpatialReference pSpatialReference = pSpatialReferenceFactory.CreateProjectedCoordinateSystem((int)pcsType);
            return pSpatialReference;
        }

        public static ISpatialReference CreateUnKnownSpatialReference()
        {
            ISpatialReference pSpatialReference = new UnknownCoordinateSystemClass();
            pSpatialReference.SetDomain(0, 99999999, 0, 99999999);//设置空间范围  
            return pSpatialReference;
        }

        public static ISpatialReference GetSpatialReference(IFeatureDataset pFeatureDataset)
        {
            IGeoDataset pGeoDataset = pFeatureDataset as IGeoDataset;
            ISpatialReference pSpatialReference = pGeoDataset.SpatialReference;
            return pSpatialReference;
        }

        public static ISpatialReference GetSpatialReference(IFeatureLayer pFeatureLayer)
        {
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            IGeoDataset pGeoDataset = pFeatureClass as IGeoDataset;
            ISpatialReference pSpatialReference = pGeoDataset.SpatialReference;
            return pSpatialReference;
        }

        /// <summary>
        /// 作用：获取要素类的坐标系
        /// 作者：汪建龙
        /// 编写时间：2017年4月10日13:32:17
        /// </summary>
        /// <param name="pFeatureClass"></param>
        /// <returns></returns>
        public static ISpatialReference GetSpatialReference(IFeatureClass pFeatureClass)
        {
            IGeoDataset pGeoDataset = pFeatureClass as IGeoDataset;
            ISpatialReference pSpatialReference = pGeoDataset.SpatialReference;
            return pSpatialReference;
        }

        public static void AlterSpatialReference(IFeatureDataset pFeatureDataset, ISpatialReference pSpatialReference)
        {
            IGeoDataset pGeoDataset = pFeatureDataset as IGeoDataset;
            IGeoDatasetSchemaEdit pGeoDatasetSchemaEdit = pGeoDataset as IGeoDatasetSchemaEdit;
            if (pGeoDatasetSchemaEdit.CanAlterSpatialReference == true)
                pGeoDatasetSchemaEdit.AlterSpatialReference(pSpatialReference);
        }

        public static void AlterSpatialReference(IFeatureClass pFeatureClass, ISpatialReference pSpatialReference)
        {
            IGeoDataset pGeoDataset = pFeatureClass as IGeoDataset;
            IGeoDatasetSchemaEdit pGeoDatasetSchemaEdit = pGeoDataset as IGeoDatasetSchemaEdit;
            if (pGeoDatasetSchemaEdit.CanAlterSpatialReference == true)
                pGeoDatasetSchemaEdit.AlterSpatialReference(pSpatialReference);
        }

        /// <summary>
        /// 作用：获取shpefile文件的坐标系
        /// 作者：汪建龙
        /// 编写时间：2017年4月10日13:30:59
        /// </summary>
        /// <param name="filePath">shapefile文件全路径</param>
        /// <returns></returns>
        public static ISpatialReference GetShpSpatialReference(this string filePath)
        {
            var featureClass = filePath.GetShpFeatureClass();
            if (featureClass != null)
            {
                IGeoDataset geoDataset = featureClass as IGeoDataset;
                return geoDataset.SpatialReference;
            }
            return null;
        }


        /// <summary>
        /// 作用：获取高斯克吕哥西安80  40坐标系
        /// </summary>
        /// <returns></returns>
        public static ISpatialReference Get40SpatialReference()
        {
            return CreateSpatialReference(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Coordinate Systems/Projected Coordinate Systems/Gauss Kruger/Xian 1980/Xian 1980 3 Degree GK Zone 40.prj"));
        }
    }
}
