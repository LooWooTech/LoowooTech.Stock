using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Xml;

namespace LoowooTech.Stock.Common
{
    public static class XmlClass
    {
        public static List<string> GetRequireTables()
        {
            return XmlManager.Get("/Tables/Table", "Name", XmlEnum.Field); //Get("/Tables/Table", "Name");
        }
        public static List<Field> GetField(string tableName)
        {
            var list = new List<Field>();
            var nodes = XmlManager.GetList("/Tables/Table[@Name='" + tableName + "']/Field", XmlEnum.Field); //_configXml.SelectNodes("/Tables/Table@[Name=" + tableName + "]/Field");
            if (nodes != null)
            {
                var type = FieldType.Int;
                var length = 0;
                for (var i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    var str = node.Attributes["Type"].Value;
                    if (str == "Date")
                    {
                        str = "DateTime";
                    }
                    if (!string.IsNullOrEmpty(str))
                    {
                        if (Enum.TryParse(str, out type))
                        {
                            var item = new Field()
                            {
                                Name = node.Attributes["Name"].Value,
                                Title = node.Attributes["Title"].Value,
                                Length = int.TryParse(node.Attributes["Length"].Value, out length) ? length : 0,
                                Type = type
                            };
                            if (node.Attributes["Min"] != null&& int.TryParse(node.Attributes["Min"].Value, out length))
                            {
                                item.Min = length;
                            }
                            list.Add(item);
                        }
                    }
                   
                }
            }
            return list;
        }
        public static string GetClass(string tableName)
        {
            var className = string.Empty;
            var node = XmlManager.GetSingle("/Tables/Table@[Name=" + tableName + "]",XmlEnum.Field);//_configXml.SelectSingleNode("/Tables/Table@[Name=" + tableName + "]");
            if (node != null)
            {
                className = node.Attributes["Class"].Value;
            }
            return className;
        }
    }
}
