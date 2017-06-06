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
        private static List<DCDYTB> _list { get; set; }
        /// <summary>
        /// 读取到的调查单元图斑信息
        /// </summary>
        public static List<DCDYTB> List { get { return _list; } }
        private static ConcurrentBag<TB> _tb { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static ConcurrentBag<TB> TB { get { return _tb; } }
        private static string[] _keys { get; set; }
        private static List<string> _messages { get; set; }
        static DCDYTBManager()
        {
            _keys = new string[] { "CLZJD", "JYXJSYD", "GGGL_GGFWSSYD", "QTCLJSYD" };
        }
        

        public static void Init(OleDbConnection connection)
        {
            _tb = new ConcurrentBag<Models.TB>();
            var reader = ADOSQLHelper.ExecuteReader(connection, "Select XZCDM,XZCMC,TBBH,DCDYLX,MJ from DCDYTB");
            if (reader != null)
            {
                var temp = new List<DCDYTB>();
                while (reader.Read())
                {
                    var a = .0;
                    if (double.TryParse(reader[4].ToString(), out a))
                    {
                        temp.Add(new DCDYTB
                        {
                            XZCDM = reader[0].ToString().Trim(),
                            XZCMC = reader[1].ToString().Trim(),
                            TBBH = reader[2].ToString().Trim(),
                            DCDYLX = reader[3].ToString().Trim(),
                            MJ = a
                        });
                    }

                }
                _list = temp;
            }
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
            foreach(var item in _list)
            {
                Console.WriteLine(string.Format("正在验证行政区代码【{0}】行政区名称：【{1}】图斑编号：【{2}】的面积一致性；", item.XZCDM, item.XZCMC, item.TBBH));
                var currentSum = TB.Where(e => e.XZCDM == item.XZCDM && e.TBBH == item.TBBH).Sum(e => e.MJ);
                if (item.MJ < currentSum)
                {
                    var str = string.Format("行政区代码：【{0}】行政村名称：【{1}】图斑编号：【{2}】面积：【{3}】在表：【{4}】中面积和不符", item.XZCDM, item.XZCMC, item.TBBH, item.MJ, string.Join(",", _keys));
                    LogManager.Log(str);
                    QuestionManager.Add(new Question() { Code = "3401", Name = "面积一致性",Project=CheckProject.面积一致性, TableName = "DCDYTB", BSM = item.TBBH, Description = str });
                    _messages.Add(str);
                }
            }
        }
    }
}
