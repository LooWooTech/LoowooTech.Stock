using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Common
{
    public static class ExcelManager
    {
        public static List<XZ> List { get; set; }
        private static List<XZ> _XZQ { get; set; }
        /// <summary>
        /// 行政区
        /// </summary>
        public static List<XZ> XZQ { get { return _XZQ == null ? _XZQ = List.Where(e => e.XZQDM.Length == 9).ToList() : _XZQ; }  }
        private static List<XZ> _XZC { get; set; }
        /// <summary>
        /// 行政村
        /// </summary>
        public static List<XZ> XZC { get { return _XZC == null ? _XZC = List.Where(e => e.XZQDM.Length == 12).ToList() : _XZC; } }
        private static Dictionary<XZ,List<XZ>> _dict { get; set; }
        public static Dictionary<XZ,List<XZ>> Dict { get { return _dict == null ? _dict = TransformDict() : _dict; } }
        private static List<XZDC> _XZDC { get; set; }
        public static List<XZDC> XZDC { get { return _XZDC == null ? _XZDC = TransformList() : _XZDC; } }
        public static void Init(string filePath)
        {
            var list = ExcelClass.GainXZ(filePath);
            List = list;
        }

        public static Dictionary<XZ,List<XZ>> TransformDict()
        {
            var dict = new Dictionary<XZ, List<XZ>>();
            foreach(var item in XZQ)
            {
                var values = XZC.Where(e => e.XZQDM.Substring(0, 9) == item.XZQDM).ToList();
                dict.Add(item, values);
            }
            return dict;
        }
        public static List<XZDC> TransformList()
        {
            var list = new List<XZDC>();
            foreach(var item in XZC)
            {
                var xzq = XZQ.FirstOrDefault(e => e.XZQDM.Substring(0, 9) == item.XZQDM);
                if (xzq != null)
                {
                    list.Add(new Models.XZDC
                    {
                        XZQDM = item.XZQDM,
                        XZQMC = item.XZQMC,
                        XZQ = xzq
                    });
                }
            }
            return list;
        }
    }
}
