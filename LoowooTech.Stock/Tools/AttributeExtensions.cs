using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tools
{
    public class AttributeExtensions
    {
        public static IArray Identity(IFeatureClass featureclass,IGeometry geometry,string whereClause)
        {
            if (geometry == null)
            {
                return null;
            }
            if (geometry.GeometryType == esriGeometryType.esriGeometryPoint)
            {
                ITopologicalOperator topop = geometry as ITopologicalOperator;
                double buffer = 0.0;
                if(double.TryParse(System.Configuration.ConfigurationManager.AppSettings["BUFFER"],out buffer))
                {
                    geometry = topop.Buffer(buffer);
                }
            }
            IFeatureLayer featureLayer = new FeatureLayerClass();
            featureLayer.FeatureClass = featureclass;
            IFeatureLayerDefinition featureLayerDefinition = featureLayer as IFeatureLayerDefinition;
            featureLayerDefinition.DefinitionExpression = whereClause;
            IFeatureLayer newfeatureLayer = featureLayerDefinition.CreateSelectionLayer(featureclass.AliasName, false, null, whereClause);
            IIdentify identify = featureLayer as IIdentify;
            IArray identifyObjs = identify.Identify(geometry);
            return identifyObjs;
        }
    }
}
