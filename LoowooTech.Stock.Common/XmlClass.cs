using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Xml;

namespace LoowooTech.Stock.Common
{
    public static class XmlClass
    {
        private static XmlDocument _configXml { get; set; }

        static XmlClass()
        {
            _configXml = new XmlDocument();
            _configXml.Load(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.Configuration.ConfigurationManager.AppSettings["FIELD"]));
        }

        private static List<string> Get(string selectString, string fieldName)
        {
            var list = new List<string>();
            var nodes = _configXml.SelectNodes(selectString);
            if (nodes != null)
            {
                for (var i = 0; i < nodes.Count; i++)
                {
                    list.Add(nodes[i].Attributes[fieldName].Value);
                }
            }
            return list;
        }


        public static List<string> GetRequireTables()
        {
            return Get("/Tables/Table", "Name");
        }


        public static List<Field> GetField(string tableName)
        {
            var list = new List<Field>();
            var nodes = _configXml.SelectNodes("/Tables/Table@[Name=" + tableName + "]/Field");
            if (nodes != null)
            {
                var type = FieldType.Int;
                var length = 0;
                for (var i = 0; i < nodes.Count; i++)
                {
                    if (Enum.TryParse(nodes[i].Attributes["Type"].Value, out type))
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
            return list;
        }
    }
}
