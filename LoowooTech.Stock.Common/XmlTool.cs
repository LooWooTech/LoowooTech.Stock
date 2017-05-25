using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace LoowooTech.Stock.Common
{
    public class XmlTool
    {
        private  XmlDocument _configXml { get; set; }
        public XmlTool(string xmlfilePath)
        {
            _configXml = new XmlDocument();
            _configXml.Load(xmlfilePath);
        }
        public  List<string> Get(string selectString,string attributeName)
        {
            var list = new List<string>();
            var nodes = _configXml.SelectNodes(selectString);
            if (nodes != null)
            {
                for(var i = 0; i < nodes.Count; i++)
                {
                    list.Add(nodes[i].Attributes[attributeName].Value);
                }
            }
            return list;
        }
        public List<string> GetChildren(string queryString,string attributeName)
        {
            var list = new List<string>();
            var node = _configXml.SelectSingleNode(queryString);
            if (node != null)
            {
                for(var i = 0; i < node.ChildNodes.Count; i++)
                {
                    list.Add(node.ChildNodes[i].Attributes[attributeName].Value);
                }
            }
            return list;
        }

        public bool Exist(string queryString)
        {
            var node = GetSingle(queryString);
            return node != null;
        }

        public XmlNode GetSingle(string queryString)
        {
            return _configXml.SelectSingleNode(queryString);
        }

        public XmlNodeList GetList(string queryString)
        {
            return _configXml.SelectNodes(queryString);
        }
    }
}
