using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Common
{
    public static class ExcelManager
    {
        public static List<XZC> List { get; set; }
        private static List<XZC> _XZQ { get; set; }
        /// <summary>
        /// 行政区
        /// </summary>
        public static List<XZC> XZQ { get { return _XZQ == null ? _XZQ = List.Where(e => e.XZCDM.Length == 9).ToList() : _XZQ; }  }
        private static List<XZC> _XZC { get; set; }
        /// <summary>
        /// 行政村
        /// </summary>
        public static List<XZC> XZC { get { return _XZC == null ? _XZC = List.Where(e => e.XZCDM.Length == 12).ToList() : _XZC; } }
        //private static Dictionary<XZC,List<XZC>> _dict { get; set; }
        //public static Dictionary<XZC,List<XZC>> Dict { get { return _dict == null ? _dict = TransformDict() : _dict; } }
        private static List<XZDC> _XZDC { get; set; }
        public static List<XZDC> XZDC { get { return _XZDC == null ? _XZDC = TransformList() : _XZDC; } }
        public static void Init(string filePath)
        {
            var list = ExcelClass.GainXZ(filePath);
            List = list;
        }

        public static Dictionary<XZC,List<XZC>> TransformDict()
        {
            var dict = new Dictionary<XZC, List<XZC>>();
            foreach(var item in XZQ)
            {
                var values = XZC.Where(e => e.XZCDM.Substring(0, 9) == item.XZCDM).ToList();
                dict.Add(item, values);
            }
            return dict;
        }
        public static List<XZDC> TransformList()
        {
            var list = new List<XZDC>();
            foreach(var item in XZC)
            {
                var xzq = XZQ.FirstOrDefault(e => e.XZCDM.Substring(0, 9) == item.XZCDM);
                if (xzq != null)
                {
                    list.Add(new Models.XZDC
                    {
                        XZCDM = item.XZCDM,
                        XZCMC = item.XZCMC,
                        XZQ = xzq
                    });
                }
            }
            return list;
        }
    }
}
