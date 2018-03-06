using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Models
{
    public class CollectTable
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string TableName { get; set; }
        public string Regex { get; set; }
        public string Model { get; set; }
        public string Model2 { get; set; }
        public string Model3 { get; set; }
        public int CollectIndex { get; set; }
        public int StartIndex { get; set; }
    }

    public class Collect
    {
        public string XZQDM { get; set; }
        public string XZQMC { get; set; }
        public CollectTable Table { get; set; }
        public Dictionary<string,List<FieldValue>> Values { get; set; }
        private List<FieldValue> _valueList { get; set; }
        public List<FieldValue> ValueList { get { return _valueList == null ? _valueList = Tranlate() : _valueList; } }
        private List<FieldValue> Tranlate()
        {
            var list = new List<FieldValue>();
            if (Values != null)
            {
                foreach(var entry in Values.Values)
                {
                    list.AddRange(entry);
                }
            }
            return list;
        }
    }

    public class CollectExcel
    {
        public string XZQDM { get; set; }
        public string XZQMC { get; set; }
        public int Count { get; set; }
        public List<FieldValue> Values { get; set; }
    }
}
