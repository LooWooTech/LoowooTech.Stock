using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class FBaseRule:Models.IRule
    {
        public virtual string RuleName { get; }
        public virtual string ID { get;  }
        public virtual CheckProject2 CheckProject { get; set; }
        public bool Space { get { return false; } }

        public virtual string ExcelName { get; }
        protected VillageExcel ExcelInfo { get; set; }
        public virtual void Check()
        {
            Config();
            if (ExcelInfo == null||ExcelInfo.Fields==null||ExcelInfo.Fields.Count==0)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("未读取到配置文件中相关【{0}】信息", ExcelName)
                });
                return;
            }
            var excelfile = System.IO.Path.Combine(ParameterManager2.Folder, "2.规划表格数据", string.Format("表{0}.{1}.xlsx", ExcelInfo.Name, ExcelInfo.Title));
            if (System.IO.File.Exists(excelfile) == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("未找到文件【表{0}.{1}.xlsx】", ExcelInfo.Name, ExcelInfo.Title)
                });
                return;
            }
            IWorkbook workbook = excelfile.OpenExcel();
            if (workbook == null)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("打开Excel【表{0}.{1}.xlsx】workbook失败", ExcelInfo.Name, ExcelInfo.Title)
                });
                return;
            }
            var sheet = workbook.GetSheetAt(0);
            if (sheet == null)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("获取Excel【表{0}.{1}.xlsx】中的Sheet为空", ExcelInfo.Name, ExcelInfo.Title)
                });
                return;
            }

        }
        private void Config()
        {
            var node = ParameterManager2.ConfigXml.SelectSingleNode(string.Format("/Config/Excels/Excel[@Name='{0}']", ExcelName));
            if (node != null)
            {
                var entry = new VillageExcel
                {
                    Name = node.Attributes["Name"].Value,
                    Unit = node.Attributes["Unit"].Value,
                    Title = node.Attributes["Title"].Value
                };
                var nodes = node.SelectNodes("Field");
                if (nodes.Count > 0)
                {
                    var fields = new List<ExcelField2>();
                    for(var i = 0; i < nodes.Count; i++)
                    {
                        var no = nodes[i];
                        var a = 0;
                        var item = new ExcelField2
                        {
                            Index = int.Parse(no.Attributes["Index"].Value),
                            Unit = no.Attributes["Unit"].Value,
                            Title = no.Attributes["Title"].Value,
                            SQL = no.Attributes["SQL"].Value
                        };
                        if (no.Attributes["X"] != null&& int.TryParse(no.Attributes["X"].Value, out a))
                        {
                            item.X = a;
                        }
                        if(no.Attributes["Y"]!=null&&int.TryParse(no.Attributes["Y"].Value,out a))
                        {
                            item.Y = a;
                        }
                        if (no.Attributes["Indexs"] != null)
                        {
                            item.Indexs = ArrayExtensions.TranlateInt(no.Attributes["Indexs"].Value.Split(','));
                        }

                        fields.Add(item);
                    }

                    entry.Fields = fields;
                }

                ExcelInfo = entry;
            }
        }

    }
}
