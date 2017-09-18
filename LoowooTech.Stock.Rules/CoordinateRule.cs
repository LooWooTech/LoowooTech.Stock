using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    /// <summary>
    /// 作用：核对矢量数据的坐标系
    /// 备注：本检查存在错误，不做定位显示
    /// </summary>
    public class CoordinateRule:IRule
    {
        public string RuleName { get { return "平面坐标系统是否采用‘1980’西安坐标系、3度带、带带号，检查高程系统是否采用‘1985’国家高程基准，检查投影方式是否采用高斯-克吕格投影"; } }
        public string ID { get { return "2201"; } }
        public bool Space { get { return true; } }

        public void Check()
        {

            foreach (var className in ParameterManager.FeatureClassNames)
            {
                ArcGISManager.CheckCoordinate(className,RuleName);
            }
        }
    }
}
