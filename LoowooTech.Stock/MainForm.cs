using ESRI.ArcGIS.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LoowooTech.Traffic.TForms
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
    }
    
   
}
