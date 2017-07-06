using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Common
{
    public static class ExcelManager
    {
        //public static List<XZC> List { get; set; }
        private static List<XZC> _XZQ { get; set; }
        /// <summary>
        /// 行政区
        /// </summary>
        public static List<XZC> XZQ { get { return _XZQ == null ? _XZQ = GainXZQ() : _XZQ; }  }
        private static List<XZC> _XZC { get; set; }
        /// <summary>
        /// 行政村
        /// </summary>
        public static List<XZC> XZC { get { return _XZC == null ? _XZC = GainXZC() : _XZC; } }
        private static List<XZDC> _XZDC { get; set; }
        public static List<XZDC> XZDC { get { return _XZDC == null ? _XZDC = TransformList() : _XZDC; } }

        private  static Dictionary<string,List<XZC>> _dict { get; set; }
        public static Dictionary<string,List<XZC>> Dict { get { return _dict; } }

        public static void Init(string filePath)
        {
            _dict = ExcelClass.GainXZ(filePath);
            //var list = ExcelClass.GainXZ(filePath);
            //List = list;
        }

        private static List<XZC> GainXZQ()
        {
            var list = new List<XZC>();
           
            if (_dict != null)
            {
                foreach(var key in _dict.Keys)
                {
                    var array = key.Split(',');
                    list.Add(new Models.XZC
                    {
                        XZCMC = array[0],
                        XZCDM = array[1]
                    });
                }
            }
            return list;
        }
        private static List<XZC> GainXZC()
        {
            var list = new List<XZC>();
            if (_dict != null)
            {
                foreach(var range in _dict.Values)
                {
                    list.AddRange(range);
                }
            }
            return list;
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
            foreach(var entry in _dict)
            {
                var array = entry.Key.Split(',');
                var xzq = new XZC
                {
                    XZCMC = array[0],
                    XZCDM = array[1]
                };
                list.AddRange(entry.Value.Select(e => new Models.XZDC { XZCDM = e.XZCDM, XZCMC = e.XZCMC, XZQ = xzq }));
            }

            return list;
        }
    }
}
