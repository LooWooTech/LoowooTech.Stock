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
            var nodes = XmlManager.GetList("/Tables/Table@[Name=" + tableName + "]/Field", XmlEnum.Field); //_configXml.SelectNodes("/Tables/Table@[Name=" + tableName + "]/Field");
            if (nodes != null)
            {
                var type = FieldType.Int;
                var length = 0;
                for (var i = 0; i < nodes.Count; i++)
                {
                    var str = nodes[i].Attributes["Type"].Value;
                    if (str == "Date")
                    {
                        str = "DateTime";
                    }
                    if (!string.IsNullOrEmpty(str))
                    {
                        if (Enum.TryParse(str, out type))
                        {
                            list.Add(new Field()
                            {
                                Name = nodes[i].Attributes["Name"].Value,
                                Title = nodes[i].Attributes["Title"].Value,
                                Length = int.TryParse(nodes[i].Attributes["Length"].Value, out length) ? length : 0,
                                Type = type
                            });
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
