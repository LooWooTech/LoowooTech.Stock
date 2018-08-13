using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Models
{
    public class DCDYTB:XZC
    {
        public string BSM { get; set; }
        public string TBBH { get; set; }
        public string DCDYLX { get; set; }
        /// <summary>
        /// 用户填写的面积
        /// </summary>
        public double MJ { get; set; }
        /// <summary>
        /// 图斑实际的面积
        /// </summary>
        public double Area { get; set; }
        public bool Right
        {
            get
            {
                return Math.Abs(Area - MJ) < 1;
            }
        }

        /// <summary>
        /// 20180731   增加土地用途
        /// </summary>
        public string TDYT { get; set; }
    }

    public class TB:XZC
    {
        public string TBBH { get; set; }
        public double MJ { get; set; }

    }
}
