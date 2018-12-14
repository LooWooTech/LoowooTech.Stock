using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Rules
{
    public class JSYDGZQ010Rule:ArcGISBaseTool,Models.IRule
    {
        public override string RuleName { get { return "允许建设区一致性检查"; } }
        public override string ID { get { return "6007"; } }
        public override CheckProject2 CheckProject { get { return CheckProject2.村规划允许建设区不超过乡规划范围; } }
        public bool Space { get { return true; } }
        public void Check()
        {
            if (System.IO.File.Exists(ParameterManager2.XGH) == false)
            {
                return;
            }

            if(ExtractJSYDGZQ("GZQLXDM = '010'") == false)
            {
                return;
            }

            var temps = ParameterManager2.XZCList.Select(e => string.Format("XZQDM = '{0}'", e.XZCDM)).ToArray();
            var where = string.Join(" OR ", temps);
            if (ArcExtensions2.Select(string.Format("{0}\\{1}", ParameterManager2.XGH, "XZQ"), string.Format("{0}\\{1}", MdbFilePath, "XZQ"), where) == false)  //XZQ 村规划行政区范围
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("提取行政区矢量数据失败！")
                });
                return;
            }

          

            if (ArcExtensions2.Select(string.Format("{0}\\{1}", ParameterManager2.XGH, "JSYDGZQ"), string.Format("{0}\\{1}", MdbFilePath, "JSYDGZQ_X"), "GZQLXDM = '010'") == false) //JSYDGZQ_X 乡规划中所有的允许建设区
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("提取乡规划中有条件建设区数据失败")
                });
                return;
            }

           

            if (ArcExtensions2.Clip(string.Format("{0}\\{1}", MdbFilePath, "JSYDGZQ_X"), string.Format("{0}\\{1}", MdbFilePath, "XZQ"), string.Format("{0}\\{1}", MdbFilePath, "JSYDGZQ_X_XZQ"), ParameterManager2.Tolerance) == false)//JSYDGZQ_X_XZQ 本次核查村规划范围的允许建设区
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("提取行政区范围内的乡规划中的有条件建设区数据失败")
                });
                return;
            }




        }
    }
}
