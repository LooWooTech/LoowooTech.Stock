using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.DataSourcesFile;

namespace LoowooTech.Stock
{
    internal static class FileListHelper
    {
        public static bool LoadRasterData(string filePath,AxMapControl mapControl)
        {

            try
            {
                var factory = new RasterWorkspaceFactory();
                IRasterWorkspace rasterWorkspace = factory.OpenFromFile(System.IO.Path.GetDirectoryName(filePath), 0) as IRasterWorkspace;
                var fileName = System.IO.Path.GetFileName(filePath);
                 IRasterDataset rasterDataset = rasterWorkspace.OpenRasterDataset(fileName);
                if (rasterDataset == null)
                {
                    return false;
                }
                IRasterLayer rasterLayer = new RasterLayer();
                rasterLayer.CreateFromDataset(rasterDataset);
                #region  影像去黑边
                IRasterRGBRenderer2 rasterRGBRender = rasterLayer.Renderer as IRasterRGBRenderer2;
                IRasterStretch2 pRasterStretch2 = rasterRGBRender as IRasterStretch2;
                double[] value = new double[3] { 0.0, 0.0, 0.0 };
                pRasterStretch2.BackgroundValue = value;
                pRasterStretch2.Background = true;
                #endregion
                mapControl.AddLayer(rasterLayer, mapControl.LayerCount);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool LoadMapData(string mdbPath, AxMapControl mapControl, XmlNode configNode)
        {
            var factory = new AccessWorkspaceFactory();
            try
            {
                mapControl.LoadMxFile(string.Format("{0}\\Map.mxd", Application.StartupPath));
                var ws = factory.OpenFromFile(mdbPath, 0) as IFeatureWorkspace;

                var node = configNode.SelectSingleNode("Layers");
                var nodes = node.SelectNodes("Layer");
                foreach(XmlNode layerNode in nodes)
                {
                    ReplaceDataSource( layerNode.Attributes["NameInMap"].Value, layerNode.Attributes["Name"].Value, ws, mapControl);
                }
                mapControl.Refresh();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool LoadMapData2(string mdbPath, AxMapControl mapControl, XmlNode configNode)
        {
            var factory = new AccessWorkspaceFactory();
            try
            {
                mapControl.LoadMxFile(string.Format("{0}\\Map2.mxd", Application.StartupPath));
                var ws = factory.OpenFromFile(mdbPath, 0) as IFeatureWorkspace;

                var node = configNode.SelectSingleNode("Layers");
                var nodes = node.SelectNodes("Layer");
                foreach (XmlNode layerNode in nodes)
                {
                    ReplaceDataSource(layerNode.Attributes["NameInMap"].Value, layerNode.Attributes["Name"].Value, ws, mapControl);
                }
                mapControl.Refresh();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool LoadKZBJ(string filePath,AxMapControl mapControl)
        {
            IWorkspaceFactory factory = new ShapefileWorkspaceFactory();
            IWorkspace workspace = factory.OpenFromFile(System.IO.Path.GetDirectoryName(filePath), 0);
            IFeatureWorkspace featureWorkspace = workspace as IFeatureWorkspace;
            IFeatureClass featureClass = featureWorkspace.OpenFeatureClass(System.IO.Path.GetFileName(filePath));
            mapControl.AddLayerFromFile(string.Format("{0}\\扩展边界.lyr", Application.StartupPath), 3);
            for(var i = 0; i < mapControl.LayerCount; i++)
            {
                var layer = mapControl.get_Layer(i);
                if (layer.Name == "扩展边界")
                {
                    var fl = layer as IFeatureLayer;
                    fl.FeatureClass = featureClass;
                    return true;
                }
            }

            return false;
        }

        private static void ReplaceDataSource(string layerName, string layerNameInMap, IFeatureWorkspace ws, AxMapControl mapControl)
        {
            for(var i=0;i<mapControl.LayerCount;i++)
            {
                var layer = mapControl.get_Layer(i);
                if(layer.Name == layerNameInMap)
                {
                    var fl = layer as IFeatureLayer;
                    var fc = ws.OpenFeatureClass(layerName);
                    fl.FeatureClass = fc;
                }
            }
        }

        


        public static bool LoadFileList(string basePath, XmlNode configNode, TreeView treeView, ref string mdbPath)
        {
            treeView.Nodes.Clear();
            var newNode = treeView.Nodes.Add(string.Empty);
            return LoadFileList(basePath, configNode.SelectSingleNode("Folders"), newNode, ref mdbPath);
        }

        private static bool LoadFileList(string basePath, XmlNode folderNode, TreeNode node, ref string mdbPath)
        {
            node.ImageIndex = 0;
            if(Directory.Exists(basePath) == false)
            {
                node.Text = folderNode.Attributes["Name"].Value;
                return false;
            }

            var lastPath = basePath;
            var pos = basePath.LastIndexOf(@"\");
            if (pos > -1) lastPath = lastPath.Substring(pos + 1);
            node.Text = lastPath;

            if (folderNode.Attributes["Regex"] != null)
            {
                if(Regex.IsMatch(lastPath, folderNode.Attributes["Regex"].Value) == false)
                {
                    return false;
                }
            }

            node.ImageIndex = 1;
            
            var fileNodes = folderNode.SelectNodes("File");
            var files = Directory.GetFiles(basePath);
            
            foreach(XmlNode fileNode in fileNodes)
            {
                var regex = fileNode.Attributes["Regex"] == null ? fileNode.Attributes["Name"].Value : fileNode.Attributes["Regex"].Value;

                var list = files.Where(x => Regex.IsMatch(Path.GetFileName(x).Replace("（", "(").Replace("）", ")"), regex)).ToList();

                var multiple = fileNode.Attributes["Multiple"] != null && fileNode.Attributes["Multiple"].Value == "True";
                if (list.Count == 0)
                {
                    var newNode = node.Nodes.Add(fileNode.Attributes["Name"].Value);
                    newNode.ImageIndex = 0;
                }
                else
                {
                    foreach(var file in list)
                    {
                        if(file.ToLower().EndsWith(".mdb"))
                        {
                            mdbPath = file;
                        }
                        var newNode = node.Nodes.Add(Path.GetFileName(file));
                        
                        newNode.ImageIndex = 1;
                        node.Tag = file;
                        if (multiple == false) break;
                    }
                }
            }
            
            var folderNodes = folderNode.SelectNodes("Folder");
            var ret = true;
            foreach(XmlNode fNode in folderNodes)
            {
                if (fNode.Attributes["Name"] != null)
                {
                    var newNode = node.Nodes.Add(fNode.Attributes["Name"].Value);
                    if (LoadFileList(Path.Combine(basePath, fNode.Attributes["Name"].Value), fNode, newNode, ref mdbPath) == false)
                    {
                        ret = false;
                    }
                }
               
            }
            return ret;
        }
    }
}
