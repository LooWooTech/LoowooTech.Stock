using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LoowooTech.Stock.Rules
{
    public class CollectDataTool
    {
        private CollectType _collectType { get; set; }
        /// <summary>
        /// 读取数据类型是MDB还是Excel类型
        /// </summary>
        public CollectType CollectType { get { return _collectType; } set { _collectType = value; } }
        private string _sourceFolder { get; set; }
        /// <summary>
        /// 数据读取文件夹路径
        /// </summary>
        public string SourceFolder { get { return _sourceFolder; } set { _sourceFolder = value; } }
        private string _saveFolder { get; set; }
        public string SaveFolder { get { return _saveFolder; } set { _saveFolder = value; } }
        private List<CollectXZQ> _collectXZQ { get; set; }
        /// <summary>
        /// 每个市关联下属的区县列表信息
        /// </summary>
        public List<CollectXZQ> CollectXZQ { get { return _collectXZQ; } set { _collectXZQ = value; } }

        private Dictionary<CollectTable,List<ExcelField>> _tableFieldDict { get; set; }
        /// <summary>
        /// 表格对应字段信息
        /// </summary>
        public Dictionary<CollectTable,List<ExcelField>> TableFieldDict { get { return _tableFieldDict == null ? _tableFieldDict = GetFields() : _tableFieldDict; } }

        private Dictionary<CollectTable,List<ExcelField>> GetFields()
        {
            var dict = new Dictionary<CollectTable, List<ExcelField>>();
            var nodes = XmlManager.GetList("/Tables/Excel", XmlEnum.Field);
            var a = 0;
            if (nodes != null)
            {
                foreach(XmlNode node in nodes)
                {
                    var table = new CollectTable
                    {
                        Name = node.Attributes["Name"].Value,
                        Title = node.Attributes["Title"].Value,
                        Regex = node.Attributes["Regex"].Value,
                        TableName = node.Attributes["TableName"].Value,
                        Model = node.Attributes["Model"].Value,
                        Model2 = node.Attributes["Model2"].Value
                    };
                    if (node.Attributes["CollectIndex"] != null)
                    {
                        if(int.TryParse(node.Attributes["CollectIndex"].Value,out a))
                        {
                            table.CollectIndex = a;
                        }
                    }
                    if (node.Attributes["StartIndex"] != null)
                    {
                        if(int.TryParse(node.Attributes["StartIndex"].Value,out a))
                        {
                            table.StartIndex = a;
                        }
                    }
                    var list = GetFields(node, table.TableName);
                    dict.Add(table, list);
                }
            }


            return dict;
        }
        private List<ExcelField> GetFields(XmlNode parent,string tableName)
        {
            var list = new List<ExcelField>();
            var nodes = parent.SelectNodes("Field");
            if (nodes != null && nodes.Count > 0)
            {
                for (var i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    var val = new ExcelField
                    {
                        TableName = tableName,
                        Name = node.Attributes["Name"].Value,
                        Title = node.Attributes["Title"].Value,
                        Index = int.Parse(node.Attributes["Index"].Value),
                        Type = node.Attributes["Type"].Value.ToLower() == "int" ? ExcelType.Int : ExcelType.Double,
                        Compute = node.Attributes["Compute"].Value.ToLower() == "sum" ? Compute.Sum : Compute.Count,
                    };
                    if (node.Attributes["Unit"] != null)
                    {
                        val.Unit = node.Attributes["Unit"].Value.Trim();
                    }

                    if (node.Attributes["View"] != null)
                    {
                        val.View = node.Attributes["View"].Value;
                    }
                    if (node.Attributes["WhereClause"] != null)
                    {
                        val.WhereClause = node.Attributes["WhereClause"].Value;
                    }
                    if (node.Attributes["TableName"] != null)
                    {
                        val.FieldTableName = node.Attributes["TableName"].Value;
                    }
                    if (node.Attributes["Indexs"] != null)
                    {
                        var indexs = node.Attributes["Indexs"].Value;
                        if (!string.IsNullOrEmpty(indexs))
                        {
                            var temps = indexs.Split(',');
                            var res = new int[temps.Length];
                            for (var j = 0; j < temps.Length; j++)
                            {
                                var a = 0;
                                res[j] = int.TryParse(temps[j], out a) ? a : 0;
                            }
                            val.Indexs = res;
                        }
                    }
                    list.Add(val);
                }
            }
            return list;
        }

        private List<Collect> _result { get; set; } = new List<Collect>();
        public List<Collect> Result { get { return _result; } }

        private readonly object _syncRoot = new object();
        private void InitMdb()
        {
            var mdbfiles = FolderExtensions.GetFiles(SourceFolder, "*.mdb");//获取文件及下所有的mdb文件列表
            var codefiles = FolderExtensions.GetFiles(SourceFolder, "*.xls");//获取文件夹下的所有的Excel文件  即单位代码表文件
            var tools = new List<CollectTool>();
            foreach(var shi in CollectXZQ)
            {
                if (shi.Children != null)
                {
                    foreach(var quxian in shi.Children)
                    {
                        var code = codefiles.FirstOrDefault(e => e.XZQDM.ToLower() == quxian.XZCDM.ToLower() && e.XZQMC.ToLower() == quxian.XZCMC.ToLower());
                        var mdb = mdbfiles.FirstOrDefault(e => e.XZQDM.ToLower() == quxian.XZCDM.ToLower() && e.XZQMC.ToLower() == quxian.XZCMC.ToLower());
                        if (code != null && mdb != null)
                        {
                            tools.Add(new CollectTool { MdbFile = mdb.FullName, CodeFile = code.FullName, TableFieldDict = TableFieldDict,XZQDM=quxian.XZCDM,XZQMC=quxian.XZCMC });
                        }
                        else
                        {
                            Console.WriteLine(string.Format("缺少行政区代码【{0}】行政区名称【{1}】的相关数据文件或者单位代码表，故未进行统计操作", quxian.XZCDM, quxian.XZCMC));
                        }
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("市级{0}下未获取区县列表，请核对", shi.XZQMC));
                }
            }

            Parallel.ForEach(tools, tool =>
            {
                tool.Program();
                var output = tool.Result2;
                AddResult(output);
            });

            var writes = new List<WriteCollectTool>();

            foreach(var tableInfo in TableFieldDict)
            {
                var collects = Result.Where(e => e.Table.Name == tableInfo.Key.Name).ToList();
                writes.Add(new WriteCollectTool { CollectXZQ = CollectXZQ, CollectTable = tableInfo.Key, Collects = collects,Fields=tableInfo.Value,SaveFolder=SaveFolder });
            }
            Parallel.ForEach(writes, tool =>
            {
                tool.Program();
            });

        }

        private void AddResult(List<Collect> list)
        {
            lock (_syncRoot)
            {
                _result.AddRange(list);
            }
        }
        private void GainData(string mdbfile,string codeFile)
        {


        }




        public void Program()
        {




        }
    }


}
