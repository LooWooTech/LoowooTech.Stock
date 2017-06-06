using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public class ValueNullTool:ValueBaseTool, ITool
    {

        public string[] CheckFields { get; set; }
        public string Key { get; set; }
        public string WhereCaluse { get; set; }
        public bool Is_Nullable { get; set; }//true  为空  false  必填

        
        
        public string Name
        {
            get
            {
                var sb = new StringBuilder(string.Format("规则{0}:表‘{1}’ 当‘{2}’的时候，‘{3}’", ID, TableName, WhereCaluse, CheckFields[0]));
                for(var i = 1; i < CheckFields.Count(); i++)
                {
                    sb.AppendFormat("、‘{0}’", CheckFields[i]);
                }
                sb.Append(Is_Nullable ? "为空" : "必填");
                return sb.ToString(); 
            }
        }


        public bool Check(OleDbConnection connection)
        {
            var sb = new StringBuilder(CheckFields[0]);
            for (var i = 1; i < CheckFields.Count(); i++)
            {
                sb.AppendFormat(",{0}", CheckFields[i]);
            }
            var reader = ADOSQLHelper.ExecuteReader(connection, string.Format("Select {0},{1} from {2} where {3}", sb.ToString(), Key, TableName, WhereCaluse));
            if (reader != null)
            {
                var str = string.Empty;
                var info = string.Empty;
                while (reader.Read())
                {
                    str = string.Empty;
                    for (var i = 0; i < CheckFields.Count(); i++)
                    {
                        if (Is_Nullable ^ string.IsNullOrEmpty(reader[i].ToString()))//异或  Is_NULLable  ture 为空  字段不为空或者 Is_NULLable false 必填 字段为空 矛盾
                        {
                            str += CheckFields[i] + ",";
                        }
                    }
                    if (!string.IsNullOrEmpty(str))
                    {
                        info = string.Format("{0}对应的字段：{1}与要求的{2}不符", reader[CheckFields.Count()], str, Is_Nullable ? "为空" : "必填");
                        _questions.Add(new Question { Code = "5101", Name = Name, Project = CheckProject.图层内属性一致性, TableName = TableName, BSM = reader[CheckFields.Count()].ToString(), Description = info });
                        Messages.Add(info);
                    }
                }
                QuestionManager.AddRange(_questions);
                return true;
            }
            return false;
        }
    }
}
