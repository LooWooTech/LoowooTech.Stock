using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.ArcGISTool
{
    public class ArcGISBaseTool
    {
        /// <summary>
        /// 确保 唯一
        /// </summary>
        public virtual string ID { get; }

        public virtual string RuleName { get; }
        public virtual CheckProject2 CheckProject { get; }
        public string CreateNewAccess()
        {
            var fileName = string.Format("temp-{0}", ID);
            var folder = ParameterManager2.InitFolder;
            var filePath = string.Empty;
            if (ArcExtensions2.CreateAccess(folder, fileName) == true)
            {
                filePath = System.IO.Path.Combine(folder, string.Format("{0}.mdb", fileName));
            }

            return filePath;
        }

        /// <summary>
        /// 拷贝mdb文件
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <returns></returns>
        public string CopyFile(string sourceFile)
        {
            var newFilePath = System.IO.Path.Combine(ParameterManager2.InitFolder, string.Format("temp-{0}.mdb", ID));
            System.IO.File.Copy(sourceFile, newFilePath, true);
            return newFilePath;
        }

        protected string MdbFilePath { get; set; }
        protected void Init()
        {
            var goalFilePath = CopyFile(ParameterManager2.MDBFilePath);
            if (System.IO.File.Exists(goalFilePath) == false)
            {
                QuestionManager2.Add(new Models.Question2
                {
                    Code = ID,
                    Name = RuleName,
                    Description = "复制MDB文件失败，文件不存在",
                    CheckProject = CheckProject
                });
            }
            else
            {
                MdbFilePath = goalFilePath;
            }   
        }

        protected void Init2()
        {
            var goalFilePath = CreateNewAccess();
            if (System.IO.File.Exists(goalFilePath) == false)
            {
                QuestionManager2.Add(new Models.Question2
                {
                    Code = ID,
                    Name = RuleName,
                    Description = "创建MDB文件失败，文件不存在",
                    CheckProject = CheckProject
                });
            }
            else
            {
                MdbFilePath = goalFilePath;
            }
        }

        protected bool ExtractGHYT(string where=null)
        {
            Init2();
            if (System.IO.File.Exists(MdbFilePath) == false)
            {
                return false;
            }
            if (ArcExtensions2.ImportFeatureClass(string.Format("{0}\\{1}", ParameterManager2.MDBFilePath, "GHYT"), MdbFilePath, "GHYT", where) == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("导入规划用途层失败")
                });
                return false;
            }

            return true;
        }

        protected bool ExtractJSYDGZQ(string where = null)
        {
            Init2();
            if (System.IO.File.Exists(MdbFilePath) == false)
            {
                return false;
            }
            if (ArcExtensions2.ImportFeatureClass(string.Format("{0}\\{1}", ParameterManager2.MDBFilePath, "JSYDGZQ"), MdbFilePath, "JSYDGZQ", where) == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("导入建设用地管制区图层失败")
                });
                return false;
            }
            return true;
        }

        protected bool ExtractJQDLTB(string where)
        {
            Init2();
            if (System.IO.File.Exists(MdbFilePath) == false)
            {
                return false;
            }
            if (ArcExtensions2.ImportFeatureClass(string.Format("{0}\\{1}", ParameterManager2.MDBFilePath, "JQDLTB"), MdbFilePath, "JQDLTB", where) == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("导入【{0}】的基期地类图斑层失败", where)
                });
                return false;
            }
            if (System.IO.File.Exists(ParameterManager2.TDLYXZ) == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("土地利用现状数据库未指定，无法核对转化前地类信息")

                });
                return false;
            }
            var dltb = string.Format("{0}\\{1}", ParameterManager2.TDLYXZ, "DLTB");
            var clip_feature = string.Format("{0}\\{1}",MdbFilePath,"JQDLTB");
            var out_feature = string.Format("{0}\\{1}", MdbFilePath, "DLTB");
            if (ArcExtensions2.Clip(dltb, clip_feature, out_feature, ParameterManager2.Tolerance) == false)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("提取基期地类图斑层中【{0}】对应基数转换前的DLTB层失败", where)
                });
                return false;
            }



            return true;
        }


        protected object SearchRecord(string mdbfile, string sql)
        {
            object obj = null;
            using (var connection = new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", mdbfile)))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    obj = command.ExecuteScalar();
                }
                connection.Close();
            }
            if (obj == null)
            {
                QuestionManager2.Add(new Question2
                {
                    Code = ID,
                    Name = RuleName,
                    CheckProject = CheckProject,
                    Description = string.Format("核查【{0}】转换之前的地类的时候，执行SQL【{1}】结果为NULL", CheckProject.GetDescription(), sql)
                });
            }
            return obj;
        }
        protected List<string> Search(string mdbfile,string sql)
        {
            var list = new List<string>();
            using (var connection=new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", MdbFilePath)))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(reader[0].ToString());
                    }
                }

                connection.Close();
            }
            return list;
        }


    }
}
