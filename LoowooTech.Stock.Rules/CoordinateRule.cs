using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class CoordinateRule:IRule
    {
        public string RuleName { get { return "平面坐标系统是否采用‘1980’西安坐标系、3度带、带带号，检查高程系统是否采用‘1985’国家高程基准，检查投影方式是否采用高斯-克吕格投影"; } }
        public string ID { get { return "2201"; } }

        public void Check()
        {

            foreach (var className in ParameterManager.FeatureClassNames)
            {
                ArcGISManager.CheckCoordinate(className,RuleName);
            }
        }
    }
}
