using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class JQZHQHRule:ArcGISBaseTool,Models.IRule
    {
        public override string RuleName { get { return "基数转换前后地类转换关系正确性"; } }
        public override string ID { get { return "2002"; } }
        public override CheckProject2 CheckProject { get { return CheckProject2.基数转换前后地类转换关系正确性; } }
        public bool Space { get { return true; } }

        
        public void Check()
        {
            if (System.IO.File.Exists(ParameterManager2.TDLYXZ) == false)
            {
                return;
            }
            if (ExtractJQDLTB("ZHLX IS NULL") == false)//JQDLTB  基数转换类型为空的基期地类图斑
            {
                return;
            }
            if (ArcExtensions2.ImportFeatureClass(string.Format("{0}\\{1}", ParameterManager2.TDLYXZ, "DLTB"), MdbFilePath, "DLTB_A",null) == false)//DLTB_A  变更调查数据
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("导入土地利用现状数据库中的DLTB层失败")
                });
                return;
            }




            if (ArcExtensions2.Intersect(string.Format("{0}\\{1};{0}\\{2}", MdbFilePath, "JQDLTB", "DLTB_A"), string.Format("{0}\\{1}", MdbFilePath, "JQDLTB_Intersect"), ParameterManager2.Tolerance) == false)//基期地类图斑与变更调查数据Intersect
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("JQDLTB与DLTB层Intersect")
                });
                return;
            }


            using (var connection=new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", MdbFilePath)))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT DLDM,DLBM FROM JQDLTB_Intersect";
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var dldm = reader[0].ToString();
                        var dlbm = reader[1].ToString();
                        if (ParameterManager2.DLDY.ContainsKey(dldm))
                        {
                            var ranges = ParameterManager2.DLDY[dldm];
                            if (ranges.Contains(dlbm) == false)
                            {

                            }
                        }
                    }
                }

                connection.Close();
            }
            
        }
    }
}
