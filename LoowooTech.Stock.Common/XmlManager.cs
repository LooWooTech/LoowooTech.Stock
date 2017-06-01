using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace LoowooTech.Stock.Common
{
    public static class XmlManager
    {
        /// <summary>
        /// 行政区代码表
        /// </summary>
        private static XmlTool _cityTool { get; set; }
        /// <summary>
        /// 数据库表字段名称、类型、长度
        /// </summary>
        private static XmlTool _fieldTool { get; set; }
        /// <summary>
        /// 数据完整性
        /// </summary>
        private  static XmlTool _dataTool { get; set; }

        static XmlManager()
        {
            _cityTool = new XmlTool(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.Configuration.ConfigurationManager.AppSettings["CodeCity"]));
            _fieldTool = new XmlTool(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.Configuration.ConfigurationManager.AppSettings["FIELD"]));
            _dataTool = new XmlTool(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.Configuration.ConfigurationManager.AppSettings["DataTree"]));
        }


        public static bool Exist(string queryString,XmlEnum xml)
        {
            switch (xml)
            {
                case XmlEnum.City:
                    return _cityTool.Exist(queryString);
                case XmlEnum.DataTree:
                    return _dataTool.Exist(queryString);
                case XmlEnum.Field:
                    return _fieldTool.Exist(queryString);
            }
            return false;
        }
        public static List<string> Get(string queryString,string attributeName,XmlEnum xml)
        {
            switch (xml)
            {
                case XmlEnum.City:
                    return _cityTool.Get(queryString,attributeName);
                case XmlEnum.DataTree:
                    return _dataTool.Get(queryString, attributeName);
                case XmlEnum.Field:
                    return _fieldTool.Get(queryString, attributeName);
            }
            return null;
        }
        public static List<string> GetChildren(string queryString,string attributeName,XmlEnum xml)
        {
            switch (xml)
            {
                case XmlEnum.City:
                    return _cityTool.GetChildren(queryString, attributeName);
                case XmlEnum.DataTree:
                    return _dataTool.GetChildren(queryString, attributeName);
                case XmlEnum.Field:
                    return _fieldTool.GetChildren(queryString, attributeName);
            }
            return null;
        }
        public static XmlNode GetSingle(string queryString,XmlEnum xml)
        {
            switch (xml)
            {
                case XmlEnum.City:
                    return _cityTool.GetSingle(queryString);
                case XmlEnum.DataTree:
                    return _dataTool.GetSingle(queryString);
                case XmlEnum.Field:
                    return _fieldTool.GetSingle(queryString);
            }
            return null;
        }
        public static string GetSingle(string queryString,string attribute,XmlEnum xml)
        {
            var node = GetSingle(queryString, xml);
            if (node != null)
            {
                return node.Attributes[attribute].Value;
            }
            return string.Empty;
        }
        public static XmlNodeList GetList(string queryString,XmlEnum xml)
        {
            switch (xml)
            {
                case XmlEnum.City:
                    return _cityTool.GetList(queryString);
                case XmlEnum.DataTree:
                    return _dataTool.GetList(queryString);
                case XmlEnum.Field:
                    return _fieldTool.GetList(queryString);
            }
            return null;
        }

    }

    public enum XmlEnum
    {
        City,
        Field,
        DataTree
    }
}
