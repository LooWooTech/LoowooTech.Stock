using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public class ExcelValueCompareTool:ITool2
    {
        public string ExcelName { get; set; }
        public string Name
        {
            get
            {
                return string.Format("规则{0}：{1}的和{2}{3}", ID, string.Join(",", FieldArray1), Compare.GetDescription(), string.Join(",", FieldArray2));
            }
        }
        public string ID { get; set; }
        public Compare Compare { get; set; }
        public ExcelType Type { get; set; }
        public Dictionary<string,List<FieldValue>> Dict { get; set; }
        public string[] FieldArray1 { get; set; }
        public string[] FieldArray2 { get; set; }
        private List<Question> _questions { get; set; }

        public ExcelValueCompareTool()
        {
            _questions = new List<Question>();
        }

        public bool CheckInt()
        {
            var info = string.Empty;
            foreach(var entry in Dict)
            {
                var list = entry.Value;
                int val1 = 0, val2 = 0, temp = 0;
                foreach(var item in FieldArray1)
                {
                    var a = list.FirstOrDefault(e => e.Title == item);
                    if (a != null)
                    {
                        if(int.TryParse(a.Value,out temp))
                        {
                            val1 += temp;
                        }
                    }
                }

                foreach(var item in FieldArray2)
                {
                    var a = list.FirstOrDefault(e => e.Title == item);
                    if (a != null)
                    {
                        if(int.TryParse(a.Value,out temp))
                        {
                            val2 += temp;
                        }
                    }
                }

                bool flag = true;
                switch (Compare)
                {
                    case Compare.Above:
                        flag = val1 > val2;
                        break;
                    case Compare.Equal:
                        flag = val1 == val2;
                        break;
                    case Compare.Less:
                        flag = val1 < val2;
                        break;
                    case Compare.MoreEqual:
                        flag = val1 >= val2;
                        break;
                    case Compare.SmarllerEqual:
                        flag = val1 <= val2;
                        break;
                }
                if (!flag)
                {
                    info = string.Format("{0}不符合{1}", entry.Key, Name);
                    _questions.Add(new Question { Code = "3201", Name = Name, TableName = ExcelName, BSM = entry.Key, Description = info });
                }
            }
            QuestionManager.AddRange(_questions);
            return true;
        }
        public bool CheckDouble()
        {
            var info = string.Empty;
            foreach (var entry in Dict)
            {
                var list = entry.Value;
                double val1 = .0, val2 = .0, temp = .0;
                foreach (var item in FieldArray1)
                {
                    var a = list.FirstOrDefault(e => e.Title == item);
                    if (a != null)
                    {
                        if (double.TryParse(a.Value, out temp))
                        {
                            val1 += temp;
                        }
                    }
                }
                foreach (var item in FieldArray2)
                {
                    var a = list.FirstOrDefault(e => e.Title == item);
                    if (a != null)
                    {
                        if (double.TryParse(a.Value, out temp))
                        {
                            val2 += temp;
                        }
                    }
                }
                val1 = Math.Round(val1, 4);
                val2 = Math.Round(val2, 4);
                bool flag = true;
                switch (Compare)
                {
                    case Compare.Above:
                        flag = val1 > val2;
                        break;
                    case Compare.Equal:
                        flag = Math.Abs(val1 - val2) < 0.001;
                        break;
                    case Compare.Less:
                        flag = val1 < val2;
                        break;
                    case Compare.MoreEqual:
                        flag = val1 >= val2;
                        break;
                    case Compare.SmarllerEqual:
                        flag = val1 <= val2;
                        break;
                }
                if (!flag)
                {
                    info = string.Format("{0}不符合{1}", entry.Key, Name);
                    _questions.Add(new Question { Code = "3201", Name = Name, TableName = ExcelName, BSM = entry.Key, Description = info });
                }

            }
            QuestionManager.AddRange(_questions);
            return true;
        }
        public bool Check()
        {
            switch (Type)
            {
                case ExcelType.Double:
                    return CheckDouble();
                case ExcelType.Int:
                    return CheckInt();
            }

            return false;
        }
    
    }
}
