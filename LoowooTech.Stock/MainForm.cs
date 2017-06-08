using ESRI.ArcGIS.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace LoowooTech.Stock
{

    public partial class MainForm : Form
    {

        private static readonly Dictionary<string, Type> toolDictionary = new Dictionary<string, Type>
        {
            {"ZoomIn", typeof(ControlsMapZoomInToolClass) },
            {"ZoomOut", typeof(ControlsMapZoomOutToolClass) },
            {"Globe", typeof(ControlsMapFullExtentCommandClass) },
            {"Previous", typeof(ControlsMapZoomToLastExtentBackCommandClass) },
            {"Next", typeof(ControlsMapZoomToLastExtentForwardCommandClass) },
            {"Pan", typeof(ControlsMapZoomToLastExtentBackCommandClass) },
            {"Identity", typeof(ControlsMapIdentifyToolClass) }
        };

        static MainForm()
        {
            
        }

        public MainForm()
        {
            InitializeComponent();
           
        }

        

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadRules();


        }

        private void LoadRules()
        {
            treeView1.Nodes.Clear();
            var doc = new XmlDocument();
            doc.Load(Application.StartupPath + @"\Rules.xml");
            var node = doc.SelectSingleNode("Rules");
            LoadNode(node, treeView1.Nodes);
        }

        private void LoadNode(XmlNode xmlNode, TreeNodeCollection parentSubNodes)
        {
            var ruleNodes = xmlNode.SelectNodes("Rule");
            if(ruleNodes.Count>0)
            {
                for(var i=0;i<ruleNodes.Count;i++)
                {
                    var node = ruleNodes[i];
                    var childNode = parentSubNodes.Add(node.Attributes["Code"].Value, string.Format("[{0}]{1}", node.Attributes["Code"].Value, node.Attributes["Name"].Value));
                    childNode.Checked = true;
                }
            }
            else
            {
                var ruleGroupNodes = xmlNode.SelectNodes("Category");
                for(var i=0;i<ruleGroupNodes.Count;i++)
                {
                    var node = ruleGroupNodes[i];
                    var childNode = parentSubNodes.Add(string.Format("[{0}]{1}", node.Attributes["Code"].Value, node.Attributes["Name"].Value));
                    childNode.Checked = true;
                    LoadNode(node, childNode.Nodes);
                    childNode.Expand();
                }
            }
        }

        private void ToolButton_Click(object sender, EventArgs e)
        {
            if (sender is Control)
            {
                var btn = sender as Control;
                if (btn.Tag != null && toolDictionary.ContainsKey(btn.Tag.ToString()))
                {
                    var type = toolDictionary[btn.Tag.ToString()];
                    var o = Activator.CreateInstance(type);
                    var cmd = o as ESRI.ArcGIS.SystemUI.ICommand;
                    if (o != null)
                    {
                        cmd.OnCreate(axMapControl1.Object);
                        var tool = o as ESRI.ArcGIS.SystemUI.ITool;
                        if (tool != null)
                        {
                            axMapControl1.CurrentTool = tool;
                        }
                        else
                        {
                            cmd.OnClick();
                        }
                    }
                }
            }
        }


        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            if(sender is Control)
            {
                var btn = sender as Control;
                if(btn.Tag != null && toolDictionary.ContainsKey(btn.Tag.ToString()))
                {
                    var type = toolDictionary[btn.Tag.ToString()];
                    var o = Activator.CreateInstance(type);
                    var cmd = o as ESRI.ArcGIS.SystemUI.ICommand;
                    if (o != null)
                    {
                        cmd.OnCreate(axMapControl1.Object);
                        var tool = o as ESRI.ArcGIS.SystemUI.ITool;
                        if(tool != null)
                        {
                            axMapControl1.CurrentTool = tool;
                        }
                        else
                        {
                            cmd.OnClick();
                        }
                    }
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
    
   
}
