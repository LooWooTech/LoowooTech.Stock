using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Common
{
    public static class ExcelParameterManager
    {
        private static string _time { get; set; }
        public static string Time { get { return _time; }set { _time = value; } }
        private static readonly object _syncRoot = new object();

        /// <summary>
        /// 所有表格对应的Excel列的值
        /// 读取到的Excel中所有单元格的值
        /// </summary>
        private static Dictionary<string,List<FieldValue>> _excelListDict { get; set; }
        /// <summary>
        /// 读取到的表格中的所有单元格的信息以及值
        /// </summary>
        public static Dictionary<string,List<FieldValue>> ExcelListDict { get { return _excelListDict; } }

        /// <summary>
        /// 读取到Excel中每一个乡镇对应的单元格值列表
        /// </summary>
        private static Dictionary<string,Dictionary<string,List<FieldValue>>> _excelDict { get; set; }
        /// <summary>
        /// 读取Excel中行政村代码对应的一行单元格的信息以及值
        /// </summary>
        public static Dictionary<string,Dictionary<string,List<FieldValue>>> ExcelDict { get { return _excelDict; } }

        /// <summary>
        /// 通过读取数据库中读取的正确的值列表
        /// </summary>
        private static Dictionary<string,List<FieldValue>> _accessListDict { get; set; }
        /// <summary>
        /// 分析读取数据库中 单元格正确的信息以及值
        /// </summary>
        public static Dictionary<string,List<FieldValue>> AccessListDict { get { return _accessListDict; } }
        /// <summary>
        /// 行政村代码对应的数据库分析得到的值列表
        /// </summary>
        private static Dictionary<string,Dictionary<string,List<FieldValue>>> _accessDict { get; set; }
        /// <summary>
        /// 分析读取数据库文件 得到行政村代码对应的一行单元格的正确值
        /// </summary>
        public static Dictionary<string,Dictionary<string,List<FieldValue>>> AccessDict { get { return _accessDict; } }

        static ExcelParameterManager()
        {
            _excelListDict = new Dictionary<string, List<FieldValue>>();
            _excelDict = new Dictionary<string, Dictionary<string, List<FieldValue>>>();
            _accessListDict = new Dictionary<string, List<FieldValue>>();
            _accessDict = new Dictionary<string, Dictionary<string, List<FieldValue>>>();
        }

        public static void Clear()
        {
            lock (_syncRoot)
            {
                _excelListDict.Clear();
                _excelDict.Clear();
                _accessDict.Clear();
                _accessListDict.Clear();
            }
        }

        public static void AddAccess(string key,List<FieldValue> list,Dictionary<string,List<FieldValue>> dict)
        {
            lock (_syncRoot)
            {
                if (_accessListDict.ContainsKey(key))
                {
                    _accessListDict[key] = list;
                }
                else
                {
                    _accessListDict.Add(key, list);
                }

                if (_accessDict.ContainsKey(key))
                {
                    _accessDict[key] = dict;
                }
                else
                {
                    _accessDict.Add(key, dict);
                }
            }
        }

        public static void AddExcel(string key, List<FieldValue> list, Dictionary<string, List<FieldValue>> dict)
        {
            lock (_syncRoot)
            {
                if (_excelListDict.ContainsKey(key))
                {
                    _excelListDict[key] = list;
                }
                else
                {
                    _excelListDict.Add(key, list);
                }

                if (_excelDict.ContainsKey(key))
                {
                    _excelDict[key] = dict;
                }
                else
                {
                    _excelDict.Add(key, dict);
                }
            }
        }


    }
}
