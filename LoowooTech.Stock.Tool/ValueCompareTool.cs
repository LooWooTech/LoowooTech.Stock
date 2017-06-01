using LoowooTech.Stock.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public class ValueCompareTool:ValueBaseTool, ITool
    {
        public string Name
        {
            get
            {
                return  string.Format("规则{0}：表‘{1}’中字段‘{2}’值{3}字段‘{4}’值", ID, TableName, Field, Compare.GetDescription(), string.Join(",", FieldArray)); 
            }
        }
        public string Key { get; set; }
        public Compare Compare { get; set; }
        public string Field { get; set; }
        public string[] FieldArray { get; set; }

        public bool Check(OleDbConnection connection)
        {
            var reader = ADOSQLHelper.ExecuteReader(connection, string.Format("Select {0},{1},{2} from {3}",Key, Field,string.Join(",",FieldArray), TableName));
            if (reader != null)
            {
                var val1 = .0;
                var val2 = .0;
                var info = string.Empty;
                while (reader.Read())
                {
                    var str = reader[0].ToString().Trim();
                    
                    double.TryParse(reader[1].ToString(), out val1);
                    for(var i = 0; i < FieldArray.Length; i++)
                    {
                        var a = reader[i+2].ToString();
                        val2 = .0;
                        var flag = false;
                        if (!string.IsNullOrEmpty(a))
                        {
                            double.TryParse(a, out val2);
                            switch (Compare)
                            {
                                case Compare.Above:// >
                                    flag = val1 > val2;
                                    break;
                                case Compare.MoreEqual:// >=
                                    flag = val1 >= val2;
                                    break;
                                case Compare.Equal:// ==
                                    flag = Math.Abs(val1 - val2) < 0.01;
                                    break;
                                case Compare.Less:
                                    flag = val1 < val2;
                                    break;
                                case Compare.SmarllerEqual:
                                    flag = val1 <= val2;
                                    break;
                            }
                            if (flag)
                            {
                                info = string.Format("{0}对应的{1}不符合", str, FieldArray[i]);
                                Messages.Add(info);
                                _questions.Add(new Models.Question { Code = "3201", Name = Name, TableName = TableName, BSM = str, Description = info });
                            }
                        }
                    }   
                }
                QuestionManager.AddRange(_questions);
                return true;
            }
            return false;
        }
    }

    public enum Compare
    {
        [Description("大于")]
        Above,
        [Description("大于等于")]
        MoreEqual,
        [Description("等于")]
        Equal,
        [Description("小于")]
        Less,
        [Description("小于等于")]
        SmarllerEqual
    }
}
