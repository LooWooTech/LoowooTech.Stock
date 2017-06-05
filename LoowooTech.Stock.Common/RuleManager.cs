using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Common
{
    public static class RuleManager
    {
        private static List<Rule> _list { get; set; }
        /// <summary>
        /// 所有的质检规则
        /// </summary>
        public static List<Rule> List { get { return _list == null ? _list = GetRules() : _list; } }
        private static List<Rule> GetRules()
        {
            var nodes = XmlManager.GetList("/Tables/Rules/Category", XmlEnum.Field);
            if (nodes == null || nodes.Count == 0)
            {
                return null;
            }
            var list = new List<Rule>();
            for(var i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                var category = node.Attributes["Name"].Value;
                if (string.IsNullOrEmpty(category))
                {
                    continue;
                }
                var children = node.SelectNodes("/Rule");
                if (children == null || children.Count == 0)
                {
                    continue;
                }
                for(var j = 0; j < children.Count; j++)
                {
                    var entry = children[j];
                    list.Add(new Rule
                    {
                        ID = entry.Attributes["ID"].Value,
                        Title = entry.Attributes["Title"].Value,
                        Category=category
                    });
                }
               
            }

            return list;
        }

        private static List<string> _ids { get; set; }

        /// <summary>
        /// 当前需要质检的规则ID
        /// </summary>
        public static List<string> IDS { get { return _ids; } set { _ids = value; } }

    }
}
