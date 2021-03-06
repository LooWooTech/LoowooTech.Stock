﻿using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Models;

namespace LoowooTech.Stock.Rules
{
    public class TopologyRule:IRule
    {
        public string RuleName { get { return "拓扑关系"; } }
        public string ID { get { return "4101"; } }
        public bool Space { get { return true; } }
        public void Check()
        {
            foreach(var className in ParameterManager.TopoFeatures)
            {
                ArcGISManager.Topo(className);
                ArcGISManager.XZQ("XZQ_XZC", className);
            }
        }
 

    }
}
