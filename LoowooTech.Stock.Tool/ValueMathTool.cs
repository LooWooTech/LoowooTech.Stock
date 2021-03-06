﻿using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LoowooTech.Stock.Tool
{
    /// <summary>
    /// 字段
    /// </summary>
    public class ValueMathTool:ValueBaseTool, ITool
    {

        /// <summary>
        /// 检查字段
        /// </summary>
        public string CheckFieldName { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 匹配正则表达式
        /// </summary>
        public string RegexString { get; set; }
        public string Name
        {
            get
            {
                return string.Format("规则{0}:表‘{1}’中字段‘{2}’的值格式‘{3}’",ID,TableName,CheckFieldName,RegexString);
            }
        }


        public bool Check(OleDbConnection connection)
        {
            var reader = ADOSQLHelper.ExecuteReader(connection, string.Format("Select {0},{1} from {2}", CheckFieldName, Key, TableName));
            if (reader != null)
            {
                var str = string.Empty;
                var info = string.Empty;
                while (reader.Read())
                {
                    str = reader[0].ToString();
                    if (Regex.IsMatch(str, RegexString))
                    {
                        continue;
                    }
                    else
                    {
                        info = string.Format("{0}对应的值不正确，请核对", reader[1].ToString());
                        Messages.Add(info);
                        _questions.Add(
                            new Question()
                            {
                                Code = "3201",
                                Name = Name,
                                Project = CheckProject.值符合性,
                                TableName = TableName,
                                BSM = reader[1].ToString(),
                                Description = info,
                                RelationClassName=RelationName,
                                ShowType=ShowType.Space,
                                WhereClause=string.Format("[{0}] ='{1}'",Key,reader[1].ToString())
                            });
                    }
                }
                QuestionManager.AddRange(_questions);
                return true;
            }
            return false;
        }
    }
}
