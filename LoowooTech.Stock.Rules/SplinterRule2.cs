using ESRI.ArcGIS.Geodatabase;
using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class SplinterRule2:ArcGISBaseTool, Models.IRule
    {
        public override string RuleName { get { return "碎片多边形"; } }
        public override string ID { get { return "1402"; } }
        public override CheckProject2 CheckProject { get { return CheckProject2.碎片多边形; } }
        public bool Space { get { return true; } }



        public void Check()
        {
            Init();
            if (System.IO.File.Exists(MdbFilePath) == false)
            {
                return;
            }
            IWorkspace workspace = MdbFilePath.OpenAccessFileWorkSpace();
            if (workspace == null)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    Description = "无法打开ArcGIS 工作空间",
                    CheckProject = CheckProject
                });
                return;
            }
            var tools = new List<SplinterTool>()
            {
                #region 建设用地
                new SplinterTool{ FeatureClassName="JQDLTB",CompareValue=100,Absolute=20,Key="BSM",WhereClause="DLDM LIKE '2*'"},
                new SplinterTool{ FeatureClassName="GHYT",CompareValue=100,Absolute=20,Key="BSM",WhereClause="GHYTDM LIKE 'X2*' OR GHYTDM LIKE 'G2*'"},
                new SplinterTool{ FeatureClassName="TDGHDL",CompareValue=100,Absolute=20,Key="BSM",WhereClause="GHDLDM LIKE '2*'"},
                #endregion

                #region 设施农用地
                new SplinterTool{ FeatureClassName="JDQLTB",CompareValue=200,Absolute=20,Key="BSM",WhereClause="DLDM = '151'"},
                new SplinterTool{ FeatureClassName="GHYT",CompareValue=200,Absolute=20,Key="BSM",WhereClause="GHYTDM = 'X151' OR GHYTDM = 'G151'"},
                new SplinterTool{ FeatureClassName="TDGHDL",CompareValue=200,Absolute=20,Key="BSM",WhereClause="GHDLDM = '151'"},
                #endregion

                #region 农用地（除设施农用地）
                new SplinterTool{ FeatureClassName="JQDLTB",CompareValue=400,Absolute=20,Key="BSM",WhereClause="DLDM = '111' OR DLDM = '113' OR DLDM = '12' OR DLDM = '132' OR DLDM = '152' OR DLDM = '153' OR DLDM = '154' OR DLDM = '155'"},
                new SplinterTool{ FeatureClassName="GHYT",CompareValue=400,Absolute=20,Key="BSM",WhereClause=""},
                new SplinterTool{ FeatureClassName="TDGHDL",CompareValue=400,Absolute=20,Key="BSM",WhereClause="DLDM = '111' OR DLDM = '113' OR DLDM = '12' OR DLDM = '132' OR DLDM = '152' OR DLDM = '153' OR DLDM = '154' OR DLDM = '155'"},

                #endregion

                #region 其他地类
                new SplinterTool{ FeatureClassName="JQDLTB",CompareValue=600,Absolute=20,Key="BSM",WhereClause="DLDM = '131' OR DLDM = '31' OR DLDM = '32'"},
                new SplinterTool{ FeatureClassName="GHYT",CompareValue=600,Absolute=20,Key="BSM",WhereClause=""},
                new SplinterTool{ FeatureClassName="TDGHDL",CompareValue=600,Absolute=20,Key="BSM",WhereClause="DLDM = '131' OR DLDM = '31' OR DLDM = '32'"},
                #endregion

                #region 建设用地管制区
                new SplinterTool{ FeatureClassName="JSYDGZQ",CompareValue=10000,Absolute=20,Key="BSM"}
                #endregion
            };


            foreach(var tool in tools)
            {
                if (tool.Check(workspace) == false)
                {
                    QuestionManager2.Add(new Question2
                    {
                        Code = ID,
                        Name = RuleName,
                        TableName = tool.FeatureClassName,
                        CheckProject=CheckProject2.碎片多边形,
                        Description = string.Format("执行【{0}】失败", tool.Name)

                    });
                }
                if (tool.Messages.Count > 0)
                {
                    QuestionManager2.AddRange(tool.Messages.Select(e => new Question2
                    {
                        Code = ID,
                        Name = RuleName,
                        TableName = tool.FeatureClassName,
                        Description = e.Description,
                        LocationClause = e.WhereClause,
                        CheckProject = CheckProject2.碎片多边形
                    }).ToList());
                }
            }
        }
    }
}
