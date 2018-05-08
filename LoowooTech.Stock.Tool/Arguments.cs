using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace LoowooTech.Stock.Tool
{
    public static class Arguments
    {
        private static Dictionary<CollectTable,List<ExcelField>> _collectTableDict { get; set; }
        public static Dictionary<CollectTable,List<ExcelField>> CollectTableDict { get { return _collectTableDict == null ? _collectTableDict = GetFields("/Tables/CollectExcel/Excel") : _collectTableDict; } }
        private static Dictionary<CollectTable, List<ExcelField>> _tableFieldDict { get; set; }
        /// <summary>
        /// 表格对应字段信息
        /// </summary>
        public static Dictionary<CollectTable, List<ExcelField>> TableFieldDict { get { return _tableFieldDict == null ? _tableFieldDict = GetFields("/Tables/Excel") : _tableFieldDict; } }

        private  static Dictionary<CollectTable, List<ExcelField>> GetFields(string queryString)
        {
            var dict = new Dictionary<CollectTable, List<ExcelField>>();
            var nodes = XmlManager.GetList(queryString, XmlEnum.Field);
            var a = 0;
            if (nodes != null)
            {
                foreach (System.Xml.XmlNode node in nodes)
                {
                    var table = new CollectTable
                    {
                        Name = node.Attributes["Name"].Value,
                        Title = node.Attributes["Title"].Value,
                        TableName = node.Attributes["TableName"].Value,
                        Model2 = node.Attributes["Model2"].Value,
                        Model3 = node.Attributes["Model3"].Value
                    };

                    if (node.Attributes["Regex"] != null)
                    {
                        table.Regex = node.Attributes["Regex"].Value;
                    }
                    if (node.Attributes["Model"] != null)
                    {
                        table.Model = node.Attributes["Model"].Value;
                    }
                    if (node.Attributes["CollectIndex"] != null)
                    {
                        if (int.TryParse(node.Attributes["CollectIndex"].Value, out a))
                        {
                            table.CollectIndex = a;
                        }
                    }
                    if (node.Attributes["StartIndex"] != null)
                    {
                        if (int.TryParse(node.Attributes["StartIndex"].Value, out a))
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
        private static List<ExcelField> GetFields(XmlNode parent, string tableName)
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
                        var whereClause = node.Attributes["WhereClause"].Value.Replace("‘","'").Replace("’","'");

                        val.WhereClause = whereClause;
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
    }
}
