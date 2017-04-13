using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.ArcGISTool
{
    public static class ArcGISFileHelper
    {
        public static IFeatureClass GetShpFeatureClass(string directory, string fileName)
        {
            IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactory();
            IWorkspace workspace = workspaceFactory.OpenFromFile(directory, 0);
            IFeatureClass featureClass = workspace.GetFeatureClass(fileName);
            IWorkspaceFactoryLockControl factorylock = workspaceFactory as IWorkspaceFactoryLockControl;
            if (factorylock.SchemaLockingEnabled)
            {
                factorylock.DisableSchemaLocking();
            }
            return featureClass;
        }
        public static IFeatureClass GetShpFeatureClass(this string filePath)
        {
            return GetShpFeatureClass(System.IO.Path.GetDirectoryName(filePath), System.IO.Path.GetFileNameWithoutExtension(filePath));
        }
    }
}
