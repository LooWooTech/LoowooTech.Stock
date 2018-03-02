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

        public string[] WhereFields { get; set; }
        public string Split { get; set; }
        public List<string> WhereList { get; set; }
        public string Name
        {
            get
            {
                return
                    string.IsNullOrEmpty(WhereCaluse) ?
                    string.Format("规则{0}：表‘{1}’中字段‘{2}’{3}", ID, TableName, string.Join("、", CheckFields), Is_Nullable ? "为空" : "必填")
                    : string.Format("规则{0}：表‘{1}’ 当‘{2}’时，字段‘{3}’{4}", ID, TableName, WhereCaluse, string.Join("、", CheckFields), Is_Nullable ? "为空" : "必填");
            }
        }

        public string[] LocationFields { get; set; }

        private bool CheckWhereFields(OleDbConnection connection)
        {
            var reader = ADOSQLHelper.ExecuteReader(connection,
                string.IsNullOrEmpty(WhereCaluse) ?
                (
                LocationFields == null ?
                 string.Format("Select {0},{1},{2} from {3}", Key, string.Join(",", CheckFields), string.Join(",", WhereFields), TableName)
                 : string.Format("Select {0},{1},{2},{3} from {4}", Key, string.Join(",", CheckFields), string.Join(",", WhereFields), string.Join(",", LocationFields), TableName)
                )
                : (
                LocationFields == null ?
                 string.Format("Select {0},{1},{2} from {3} where {4}", Key, string.Join(",", CheckFields), string.Join(",", WhereFields), TableName, WhereCaluse)
                 : string.Format("Select {0},{1},{2},{3} from {4} where {5}", Key, string.Join(",", CheckFields), string.Join(",", WhereFields), string.Join(",", LocationFields), TableName, WhereCaluse)
                )

                );
            if (reader != null)
            {
                var array = new string[WhereFields.Length];
                var str = string.Empty;
                var info = string.Empty;
                while (reader.Read())
                {
                    for(var i = 0; i < WhereFields.Length; i++)
                    {
                        array[i] = reader[i + 1 + CheckFields.Length].ToString();
                    }
                    var key = string.Join(Split, array);
                    if (WhereList.Contains(key))
                    {
                        str = string.Empty;
                        for(var i = 0; i < CheckFields.Length; i++)
                        {
                            var a = reader[i + 1].ToString().Trim();
                            if (Is_Nullable ^ string.IsNullOrEmpty(a))
                            {
                                str += CheckFields[i] + ",";
                            }
                        }
                        if (!string.IsNullOrEmpty(str))
                        {
                            info = string.Format("{0}对应的字段：{1}与要求的{2}不符,图斑信息：行政村代码：【{3}】图斑编号：【{4}】", reader[0].ToString().Trim(), str, Is_Nullable ? "为空" : "必填",array[0],array[1]);
                            _questions.Add(
                                new Question
                                {
                                    Code = "5101",
                                    Name = Name,
                                    Project = CheckProject.图层内属性一致性,
                                    TableName = TableName,
                                    BSM = reader[0].ToString(),
                                    Description = info,
                                    RelationClassName = RelationName,
                                    ShowType = ShowType.Space,
                                    WhereClause =
                                    LocationFields == null ?
                                    string.Format("[{0}] ='{1}'", Key, reader[0].ToString())
                                    : ADOSQLHelper.GetWhereClause(LocationFields, ADOSQLHelper.GetValues(reader, 1 + CheckFields.Length+WhereFields.Length, LocationFields.Length))
                                });
                            Messages.Add(info);
                        }
                    }
                }
                QuestionManager.AddRange(_questions);

                return true;
            }
            return false;
        }


        private bool CheckNoWhereFields(OleDbConnection connection)
        {
            var reader = ADOSQLHelper.ExecuteReader(connection,
                string.IsNullOrEmpty(WhereCaluse) ?
                (
                LocationFields == null ?
                 string.Format("Select {0},{1} from {2}", string.Join(",", CheckFields), Key, TableName)
                 : string.Format("Select {0},{1},{2} from {3}", string.Join(",", CheckFields), Key, string.Join(",", LocationFields), TableName)
                )
                : (
                LocationFields == null ?
                string.Format("Select {0},{1} from {2} where {3}", string.Join(",", CheckFields), Key, TableName, WhereCaluse)
                : string.Format("Select {0},{1},{2} from {3} where {4}", string.Join(",", CheckFields), Key, string.Join(",", LocationFields), TableName, WhereCaluse)
                )

                );
            if (reader != null)
            {
                var str = string.Empty;
                var info = string.Empty;
                while (reader.Read())
                {
                    str = string.Empty;
                    for (var i = 0; i < CheckFields.Count(); i++)
                    {
                        var a = reader[i].ToString().Trim();
                        if (Is_Nullable ^ string.IsNullOrEmpty(a))//异或  Is_NULLable  ture 为空  字段不为空或者 Is_NULLable false 必填 字段为空 矛盾
                        {
                            str += CheckFields[i] + ",";
                        }
                    }
                    if (!string.IsNullOrEmpty(str))
                    {
                        info = string.Format("{0}对应的字段：{1}与要求的{2}不符", reader[CheckFields.Count()], str, Is_Nullable ? "为空" : "必填");
                        _questions.Add(
                            new Question
                            {
                                Code = "5101",
                                Name = Name,
                                Project = CheckProject.图层内属性一致性,
                                TableName = TableName,
                                BSM = reader[CheckFields.Count()].ToString(),
                                Description = info,
                                RelationClassName = RelationName,
                                ShowType = ShowType.Space,
                                WhereClause =
                                LocationFields == null ?
                                string.Format("[{0}] ='{1}'", Key, reader[CheckFields.Count()].ToString())
                                : ADOSQLHelper.GetWhereClause(LocationFields, ADOSQLHelper.GetValues(reader, 1 + CheckFields.Length, LocationFields.Length))
                            });
                        Messages.Add(info);
                    }
                }
                QuestionManager.AddRange(_questions);
                return true;
            }
            return false;
        }


        public bool Check(OleDbConnection connection)
        {
            if (WhereFields != null)
            {
                return CheckWhereFields(connection);
            }
            return CheckNoWhereFields(connection);
        }
    }
}
