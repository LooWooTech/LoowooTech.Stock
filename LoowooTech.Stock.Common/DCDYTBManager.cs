using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Common
{
    public static class DCDYTBManager
    {
        private static List<DCDYTB> _list { get; set; }
        public static List<DCDYTB> List { get { return _list; } }
        private static Dictionary<string,List<TB>> _dict { get; set; }
        public static Dictionary<string,List<TB>> Dict { get { return _dict; } }
        
        private static List<TB> _tb { get; set; }

        public static List<TB> TB { get { return _tb == null ? _tb = MergeValues<TB>(_dict) : _tb; } }
        private static string[] _keys { get; set; }
        private static List<string> _messages { get; set; }
        static DCDYTBManager()
        {
            _keys = new string[] { "CLZJD", "JYXJSYD", "GGGL_GGFWSSYD", "QTCLJSYD" };
        }
        
        private static List<T> MergeValues<T>(Dictionary<string,List<T>> dict)
        {
            var list = new List<T>();
            foreach(var entry in dict)
            {
                list.AddRange(entry.Value);
            }
            return list;
        }

        public static void Init(OleDbConnection connection)
        {
            if (connection != null)
            {
                if (connection.State == System.Data.ConnectionState.Broken)
                {
                    connection.Close();
                    connection.Open();
                }


                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select XZCDM,XZCMC,TBBH,DCDYLX,MJ from DCDYTB";
                    using (var reader = command.ExecuteReader())
                    {
                        var temp = new List<DCDYTB>();
                        while (reader.Read())
                        {
                            var a = .0;
                            if(double.TryParse(reader[4].ToString(),out a))
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
            }
        }

        public static void AddTB(string key,List<TB> list)
        {
            if (_dict == null)
            {
                _dict = new Dictionary<string, List<TB>>();
            }
            if (_dict.ContainsKey(key))
            {
                _dict[key] = list;
            }
            else
            {
                _dict.Add(key, list);
            }
        }

        public static void Program()
        {
            foreach(var key in _keys)
            {
                if (!_dict.ContainsKey(key))
                {
                    Console.WriteLine(string.Format("未读取表{0}中的图斑面积，无法进行核对图斑面积信息", key));
                }
            }
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
                    Console.WriteLine(str);
                    _messages.Add(str);
                }
            }
        }
    }
}
