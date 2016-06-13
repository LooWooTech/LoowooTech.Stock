using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LoowooTech.Stock.ArcGISTool
{
    public class HeartClass
    {
        public string mdbFilePath { get; set; }
        public void Beat()
        {
            IWorkspace workspace = ArcClass.GetmdbWorkspace(mdbFilePath);
            IFeatureDataset featureDataset = ArcClass.GetFeatureDataset(workspace, "WJL");
            if (featureDataset != null)
            {
                ITopologyContainer topologyContainer = featureDataset as ITopologyContainer;
                ITopology topology = topologyContainer.CreateTopology("result", topologyContainer.DefaultClusterTolerance, -1, "");
                IFeatureClassContainer featureClassContainer = featureDataset as IFeatureClassContainer;
                IFeatureClass featureClass = featureClassContainer.get_ClassByName("Roads");
                topology.AddClass(featureClass, 5, 1, 1, false);
                ITopologyRuleContainer topologyRuleContainer = topology as ITopologyRuleContainer;
                ITopologyRule topologyRule = new TopologyRuleClass();
                topologyRule.TopologyRuleType = esriTopologyRuleType.esriTRTLineNoOverlap;
                topologyRule.OriginClassID = featureClass.ObjectClassID;
                topologyRule.Name = "Roads No Overlap";

                if (topologyRuleContainer.get_CanAddRule(topologyRule))
                {
                    topologyRuleContainer.AddRule(topologyRule);
                }

                topologyRule = new TopologyRuleClass();
                topologyRule.TopologyRuleType = esriTopologyRuleType.esriTRTLineNoDangles;
                topologyRule.OriginClassID = featureClass.ObjectClassID;
                topologyRule.AllOriginSubtypes = true;
                topologyRule.Name = "Roads No Dangles";

                // Add the rule to the Topology
                if (topologyRuleContainer.get_CanAddRule(topologyRule))
                {
                    topologyRuleContainer.AddRule(topologyRule);
                }
            }
            
        }

        private void Progarm()
        {
            IWorkspace workspace = ArcClass.GetmdbWorkspace(mdbFilePath);
            
            if (workspace == null)
            {
                return;
            }
            IFeatureDataset featureDataset = ArcClass.GetFeatureDataset(workspace, "WJL");
            ISchemaLock schemalock = featureDataset as ISchemaLock;
            try
            {
                schemalock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                ITopologyContainer topologyContainer = featureDataset as ITopologyContainer;
                ITopology topology = topologyContainer.CreateTopology("ttt", topologyContainer.DefaultClusterTolerance, -1, "");
                IFeatureClass featureClass = ArcClass.GetFeatureClass(workspace, "PARK");

                AddRuleToTopology(topology, esriTopologyRuleType.esriTRTAreaNoOverlap, "NO Block OverLap", featureClass);

                IGeoDataset geoDataset = topology as IGeoDataset;
                IEnvelope envelop = geoDataset.Extent;
                ValidateTopology(topology, envelop);

            }catch(COMException comEx)
            {

            }
        }

        private void AddRuleToTopology(ITopology topology,esriTopologyRuleType ruleType,string ruleName,IFeatureClass originClass,int originSubtype,IFeatureClass destinationClass,int destinationSubtype)
        {
            ITopologyRule topologyRule = new TopologyRuleClass();
            topologyRule.TopologyRuleType = ruleType;
            topologyRule.Name = ruleName;
            topologyRule.OriginClassID = originClass.FeatureClassID;
            topologyRule.AllOriginSubtypes = false;
            topologyRule.OriginSubtype = originSubtype;
            topologyRule.DestinationClassID = destinationClass.FeatureClassID;
            topologyRule.AllDestinationSubtypes = false;
            topologyRule.DestinationSubtype = destinationSubtype;

            ITopologyRuleContainer topologyRuleContainer = topology as ITopologyRuleContainer;
            if (topologyRuleContainer.get_CanAddRule(topologyRule))
            {
                topologyRuleContainer.AddRule(topologyRule);
            }
            else
            {
                throw new ArgumentException("Counld not add specified rule to the topology");
            }

        }

        private void AddRuleToTopology(ITopology topology, esriTopologyRuleType ruleType, string ruleName, IFeatureClass featureClass)
        {
            ITopologyRule topologyRule = new TopologyRuleClass();
            topologyRule.TopologyRuleType = ruleType;
            topologyRule.Name = ruleName;
            topologyRule.OriginClassID = featureClass.FeatureClassID;
            topologyRule.AllOriginSubtypes = true;
            ITopologyRuleContainer topologyRuleConatiner = topology as ITopologyRuleContainer;
            if (topologyRuleConatiner.get_CanAddRule(topologyRule))
            {
                topologyRuleConatiner.AddRule(topologyRule);
            }
            else
            {
                throw new ArgumentException("Counld not add specified rule to the topology");
            }
        }
        private void ValidateTopology(ITopology topology,IEnvelope envelope)
        {
            IPolygon locationpolygon = new PolygonClass();
            ISegmentCollection segmentCollection = locationpolygon as ISegmentCollection;
            segmentCollection.SetRectangle(envelope);
            IPolygon polgyon = topology.get_DirtyArea(locationpolygon);
            if (!polgyon.IsEmpty)
            {
                IEnvelope areaToValidate = polgyon.Envelope;
                IEnvelope areaValidated = topology.ValidateTopology(areaToValidate);
            }
        }
    }
}
