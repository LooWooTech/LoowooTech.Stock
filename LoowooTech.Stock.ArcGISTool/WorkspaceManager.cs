using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.ArcGISTool
{
    public static class WorkspaceManager
    {
        public static IFeatureClass GetFeatureClass(this IWorkspace workspcae, string featureClassName)
        {
            if (workspcae == null)
            {
                Console.WriteLine("workspace为null,无法获取要素类.........");
                return null;
            }
            IFeatureWorkspace featureWorkspace = workspcae as IFeatureWorkspace;
            IFeatureClass featureClass = null;
            try
            {
                featureClass = featureWorkspace.OpenFeatureClass(featureClassName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("获取要素类：{0}发成错误，错误信息:{1}", featureClassName, ex.Message));
                return null;
            }
            return featureClass;
        }
        /// <summary>
        /// 作用：打开CAD文件Workspace
        /// 作者：汪建龙
        /// 编写时间：2016年11月22日12:54:02
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static IWorkspace OpenCADWorkSpace(string directory)
        {
            IWorkspaceFactory workspaceFactory = new CadWorkspaceFactory();
            IWorkspace workspace = workspaceFactory.OpenFromFile(directory, 0);
            return workspace;
        }
        /// <summary>
        /// 作用：获取CAD文件单独某一个类型的要素类
        /// 作者：汪建龙
        /// 编写时间：2016年11月22日12:57:12
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="fileName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IFeatureClass GetCADFeatureClass(this IWorkspace workspace, string fileName, string type)
        {
            IFeatureWorkspace featureWorkspace = workspace as IFeatureWorkspace;
            if (featureWorkspace != null)
            {
                IFeatureClass featureClass = featureWorkspace.OpenFeatureClass(string.Format("{0}:{1}", fileName, type));
                return featureClass;
            }
            return null;
        }

        /// <summary>
        /// 作用：打开shapefile文件 并获取workspace
        /// 作者：汪建龙
        /// 编写时间：2017年4月11日15:37:02
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static IWorkspace OpenShapefileWorkSpace(string filePath)
        {
            IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactory();
            IWorkspace workspace = workspaceFactory.OpenFromFile(filePath,0);
            return workspace;
        }

        /// <summary>
        /// 作用：打开mdb文件并获取workspace
        /// 作者：汪建龙
        /// 编写时间：2017年4月11日15:39:36
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static IWorkspace OpenAccessFileWorkSpace(this string filePath)
        {
            IWorkspaceFactory workspaceFactory = new AccessWorkspaceFactoryClass();
            IWorkspace workspace = workspaceFactory.OpenFromFile(filePath,0);
            return workspace;
        }

        public static List<IFeatureClass> GetFeatureClasss(IWorkspace workspace)
        {
            var list = new List<IFeatureClass>();
            IFeatureWorkspace featureWorkspace = workspace as IFeatureWorkspace;
            IEnumDataset enumDataset = workspace.get_Datasets(esriDatasetType.esriDTFeatureClass);
            enumDataset.Reset();
            IDataset dataset = enumDataset.Next();
            while (dataset != null)
            {
                if(dataset is IFeatureDataset)
                {


                }else if(dataset is IFeatureClass)
                {

                }

                dataset = enumDataset.Next();
            }
            return list;
        }
    }
}
