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
                return  string.Format("规则{0}：表‘{1}’中字段‘{2}’值{3}字段‘{4}’值", ID, TableName, string.Format(",",FieldArray1), Compare.GetDescription(), string.Join(",", FieldArray2)); 
            }
        }
        public string Key { get; set; }
        public Compare Compare { get; set; }
        public string[] FieldArray1 { get; set; }
        public string[] FieldArray2 { get; set; }

        public bool Check(OleDbConnection connection)
        {
            var reader = ADOSQLHelper.ExecuteReader(connection, string.Format("Select {0},{1},{2} from {3}",Key, string.Join(",",FieldArray1),string.Join(",",FieldArray2), TableName));
            if (reader != null)
            {
                var val1 = .0;
                var val2 = .0;
                var a = .0;
                var info = string.Empty;
                while (reader.Read())
                {
                    var str = reader[0].ToString().Trim();
                    for(var i = 0; i < FieldArray1.Length; i++)
                    {
                        if(double.TryParse(reader[i+1].ToString(),out a))
                        {
                            val1 += a;
                        }
                    }
                    for(var i = 0; i < FieldArray2.Length; i++)
                    {
                        if (double.TryParse(reader[i+1+FieldArray1.Length].ToString(),out a))
                        {
                            val2 += a;
                        }
                    }
                    var flag = false;
                    switch (Compare)
                    {
                        case Compare.Above:
                            flag = val1 > val2;
                            break;
                        case Compare.MoreEqual:
                            flag = val1 >= val2;
                            break;
                        case Compare.Equal:
                            flag = Math.Abs(val1 - val2) < 0.001;
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
                        info = string.Format("{0}对应的不符合{1}", str, Name);
                        Messages.Add(info);
                        _questions.Add(new Models.Question { Code = "3201", Name = Name, TableName = TableName, BSM = str, Description = info });
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
