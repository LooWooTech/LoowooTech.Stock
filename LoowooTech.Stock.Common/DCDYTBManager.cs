using LoowooTech.Stock.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoowooTech.Stock.Common
{
    public static class DCDYTBManager
    {
        public static string ClassName = "DCDYTB";
        private static List<DCDYTB> _list { get; set; }
        /// <summary>
        /// 读取到的调查单元图斑信息
        /// </summary>
        public static List<DCDYTB> List { get { return _list; } set { _list = value; } }
        private static ConcurrentBag<TB> _tb { get; set; }
        private static Dictionary<string,double> _dict { get; set; }
        public static Dictionary<string,double> Dict { get { return _dict == null ? _dict = _tb.GroupBy(e => e.XZCDM + "#" + e.TBBH).ToDictionary(e => e.Key, e => e.Sum(k =>Math.Round(k.MJ,2))) : _dict; } }
        private static string[] _keys { get; set; }
        private static readonly object _syncRoot = new object();
        private static List<string> _messages { get; set; }
        static DCDYTBManager()
        {
            _keys = new string[] { "CLZJD", "JYXJSYD", "GGGL_GGFWSSYD", "QTCLJSYD" };
        }
        

        public static void Init(OleDbConnection connection)
        {
            _tb = new ConcurrentBag<Models.TB>();
            //var reader = ADOSQLHelper.ExecuteReader(connection, "Select XZCDM,XZCMC,TBBH,DCDYLX,MJ from DCDYTB");
            //if (reader != null)
            //{
            //    var temp = new List<DCDYTB>();
            //    while (reader.Read())
            //    {
            //        var a = .0;//原始单位为亩
            //        if (double.TryParse(reader[4].ToString(), out a))
            //        {
            //            temp.Add(new DCDYTB
            //            {
            //                XZCDM = reader[0].ToString().Trim(),
            //                XZCMC = reader[1].ToString().Trim(),
            //                TBBH = reader[2].ToString().Trim(),
            //                DCDYLX = reader[3].ToString().Trim(),
            //                MJ =a
            //            });
            //        }

            //    }
            //    _list = temp;
            //}
        }

        public static void AddTB(List<TB> list)
        {
            Parallel.ForEach(list, item =>
            {
                _tb.Add(item);
            });
        }

        public static void Program()
        {
            if (_messages == null)
            {
                _messages = new List<string>();
            }
            else
            {
                _messages.Clear();
            }
            var dict = _list.GroupBy(e => e.XZCDM.Substring(0,9)).ToDictionary(e => e.Key, e => e.ToList());
            Parallel.ForEach(dict, entry =>
            {
                Program(entry.Value);
            });
            //foreach(var item in _list)
            //{
            //    Console.WriteLine(string.Format("正在验证行政区代码【{0}】行政区名称：【{1}】图斑编号：【{2}】的面积一致性；", item.XZCDM, item.XZCMC, item.TBBH));
            //    var currentSum = TB.Where(e => e.XZCDM == item.XZCDM && e.TBBH == item.TBBH).Sum(e => e.MJ);
            //    if (item.MJ < currentSum)
            //    {
            //        var str = string.Format("行政区代码：【{0}】行政村名称：【{1}】图斑编号：【{2}】面积：【{3}】在表：【{4}】中面积和不符", item.XZCDM, item.XZCMC, item.TBBH, item.MJ, string.Join(",", _keys));
            //        LogManager.Log(str);
            //        QuestionManager.Add(new Question() { Code = "3401", Name = "面积一致性",Project=CheckProject.面积一致性, TableName = "DCDYTB", BSM = item.TBBH, Description = str });
            //        _messages.Add(str);
            //    }
            //}
        }
        private static void AddMessage(string message)
        {
            lock (_syncRoot)
            {
                _messages.Add(message);
            }
        }

        private static void Program(List<DCDYTB> list)
        {
            foreach(var item in list)
            {
                Console.WriteLine(string.Format("正在验证行政区代码【{0}】行政区名称：【{1}】图斑编号：【{2}】的面积一致性；", item.XZCDM, item.XZCMC, item.TBBH));
                var key = string.Format("{0}#{1}", item.XZCDM, item.TBBH);
                if (Dict.ContainsKey(key))
                {
                    var currentSum = Dict[key];
                    if (item.Area+1<=currentSum)//面积容差率 不能超过1平方米
                    {
                        var str = string.Format("行政区代码：【{0}】行政村名称：【{1}】图斑编号：【{2}】面积：【{3}】平方米 在表：【{4}】中面积和【{5}】平方米不符", item.XZCDM, item.XZCMC, item.TBBH, item.MJ, string.Join(",", _keys),currentSum);
                        LogManager.Log(str);
                        QuestionManager.Add(
                            new Question()
                            {
                                Code = "3401",
                                Name = "面积一致性",
                                Project = CheckProject.面积一致性,
                                TableName = "DCDYTB",
                                BSM = item.BSM,
                                Description = str,
                                RelationClassName="DCDYTB",
                                ShowType=ShowType.Space,
                                WhereClause = string.Format("[BSM] = {0}", item.BSM)
                            });
                        AddMessage(str);
                    }
                }
               
                
              
            }
        }
    }
}
