using LoowooTech.Stock.WorkBench;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace LoowooTech.Stock
{
    internal static class RuleHelper
    {
        /// <summary>
        /// 将配置中的质检规则信息读取到TreeView里
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="configNode"></param>
        public static void LoadRules(TreeView treeView, XmlNode configNode)
        {
            treeView.Nodes.Clear();
            
            var node = configNode.SelectSingleNode("Rules");
            LoadNode(node, treeView.Nodes, treeView);
        }

        /// <summary>
        /// 递归方法，将配置中的质检规则信息读取到TreeView里
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="parentSubNodes"></param>
        /// <param name="treeView"></param>
        private static void LoadNode(XmlNode xmlNode, TreeNodeCollection parentSubNodes, TreeView treeView)
        {
            var ruleNodes = xmlNode.SelectNodes("Rule");
            if (ruleNodes.Count > 0)
            {
                for (var i = 0; i < ruleNodes.Count; i++)
                {
                    var node = ruleNodes[i];
                    var childNode = parentSubNodes.Add(node.Attributes["ID"].Value, string.Format("[{0}]{1}", node.Attributes["ID"].Value, node.Attributes["Title"].Value));
                    childNode.Tag = node.Attributes["ID"].Value;
                    childNode.ImageIndex = 0;
                    childNode.Checked = true;
                }
            }
            else
            {
                var ruleGroupNodes = xmlNode.SelectNodes("Category");
                for (var i = 0; i < ruleGroupNodes.Count; i++)
                {
                    var node = ruleGroupNodes[i];
                    var childNode = parentSubNodes.Add(node.Attributes["Name"].Value);
                    childNode.Checked = true;
                    childNode.ImageIndex = 3;
                    LoadNode(node, childNode.Nodes, treeView);
                    childNode.Expand();
                }
            }
        }

        /// <summary>
        /// 从treeView1里获取哪些检查规则被选中了
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="ids"></param>
        public static void GetCheckedRuleIDs(TreeNodeCollection nodes, List<int> ids)
        {
            foreach(TreeNode node in nodes)
            {
                if (node.Checked)
                {
                    if (node.Nodes.Count == 0 && node.Checked)
                    {
                        ids.Add(int.Parse(node.Tag.ToString()));
                    }
                    else
                    {
                        GetCheckedRuleIDs(node.Nodes, ids);
                    }
                }
            }
        }

        /// <summary>
        /// 将质检结果更新到treeView1的质检规则中，通过图标区别规则未检查，通过和未通过
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="results"></param>
        public static void UpdateCheckState(TreeNodeCollection nodes, Dictionary<string, ProgressResultTypeEnum> results)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Nodes.Count == 0)
                {
                    var code = node.Tag.ToString();
                    if (results.ContainsKey(code))
                    {
                        node.ImageIndex = results[code] == ProgressResultTypeEnum.Pass ? 1 : 2;
                    }
                    else
                    {
                        node.ImageIndex = 0;
                    }
                }
                else
                {
                    UpdateCheckState(node.Nodes, results);
                }
            }
        }
    }
}
