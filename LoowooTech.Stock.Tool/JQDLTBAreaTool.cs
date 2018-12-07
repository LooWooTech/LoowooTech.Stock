using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Tool
{
    public class JQDLTBAreaTool
    {
        public string Name { get { return string.Format("规则：{0}{1}", Rule1, Rule2); } }
        private string Rule1 { get { return "图斑地类面积=图斑面积-应扣田坎计算面积；"; } }
        private string Rule2 { get { return "应扣田坎计算面积=图斑面积*田坎系数；"; } }
        private string _SQL
        {
            get
            {
                return "SELECT BSM,TBMJ,TBDLMJ,TKXS,KKSM FROM JQDLTB";
            }
        }
        /// <summary>
        /// 相对值  百分比
        /// </summary>
        public double? Relative { get; set; }
        /// <summary>
        /// 绝对值 具体数值
        /// </summary>
        public double? Absolute { get; set; }

        public List<VillageMessage> Messages { get; set; } = new List<VillageMessage>();
        public bool Check(OleDbConnection connection)
        {
            var reader = ADOSQLHelper.ExecuteReader(connection, _SQL);
            if (reader == null)
            {
                return false;
            }
            var a = .0;
            var tbmj = .0;
            var tbdlmj = .0;
            var tkxs = .0;
            var kksm = .0;
            while (reader.Read())
            {
                var keyValue = reader[0].ToString().Trim();
                if (string.IsNullOrEmpty(keyValue) == false)
                {
                    if (double.TryParse(reader[4].ToString().Trim(), out a))//应扣田坎计算面积
                    {
                        kksm = Math.Round(a, 0);
                    }
                    else
                    {
                        Messages.Add(new VillageMessage
                        {
                            Value = keyValue,
                            CheckValue = reader[4].ToString().Trim(),
                            Description = string.Format("未获取应扣田坎计算面积，无法核对【{0}】", Name),
                            WhereClause = string.Format("[BSM] = '{0}'", keyValue)
                        });
                        continue;
                    }
                    if (double.TryParse(reader[1].ToString().Trim(), out a))//图斑面积
                    {
                        tbmj = Math.Round(a, 0);
                    }
                    else
                    {
                        Messages.Add(new VillageMessage
                        {
                            Value = keyValue,
                            CheckValue = reader[1].ToString().Trim(),
                            Description = string.Format("未获取图斑面积，无法核对【{0}】", Name),
                            WhereClause = string.Format("[BSM] = '{0}'", keyValue)
                        });
                        continue;
                    }

                    #region 核对 规则1
                    if (double.TryParse(reader[2].ToString().Trim(), out a))//图斑地类面积
                    {
                        tbdlmj = Math.Round(a, 0);
                        var abs = Math.Abs(tbdlmj - tbmj + kksm);

                        var sb = new StringBuilder();

                        if (Relative.HasValue)
                        {
                            var va = abs / tbdlmj;
                            if (va > Relative.Value)
                            {
                                sb.AppendFormat("相对值为【{0}】;", va);
                            }
                        }

                        if (Absolute.HasValue)
                        {
                            if (abs > Absolute.Value)
                            {
                                sb.AppendFormat("绝对值为【{0};", abs);
                                
                            }
                        }
                        if (sb.Length > 0)
                        {
                            Messages.Add(new VillageMessage
                            {
                                Value = keyValue,
                                CheckValue = sb.ToString(),
                                Description = string.Format("【BSM = '{0}'】对应的数据不符合【{1}】", keyValue, Rule1),
                                WhereClause = string.Format("[BSM] = '{0}'", keyValue)
                            });
                        }

                       
                    }
                    else
                    {
                        Messages.Add(new VillageMessage
                        {
                            Value = keyValue,
                            CheckValue = reader[2].ToString().Trim(),
                            Description = string.Format("未获取图斑地类面积，无法核对【{0}】", Rule1),
                            WhereClause = string.Format("[BSM] = '{0}'", keyValue)
                        });
                    }

                    #endregion


                    #region 核对  规则2
                    if (double.TryParse(reader[3].ToString().Trim(), out a))//田坎系数
                    {
                        tkxs = Math.Round(a, 4);
                        var calSum = tbmj * tkxs;
                        var abs = Math.Abs(calSum - kksm);
                        var sb = new StringBuilder();

                        if (Relative.HasValue)
                        {
                            var va = abs / kksm;
                            if (va > Relative.Value)
                            {
                                sb.AppendFormat("相对值为【{0}】;", va);
                            }
                        }

                        if (Absolute.HasValue)
                        {
                            if (abs > Absolute.Value)
                            {
                                sb.AppendFormat("绝对值为【{0};", abs);

                            }
                        }
                        if (sb.Length > 0)
                        {
                            Messages.Add(new VillageMessage
                            {
                                Value = keyValue,
                                CheckValue = sb.ToString(),
                                Description = string.Format("【BSM = '{0}'】对应的数据不符合【{1}】", keyValue, Rule2),
                                WhereClause = string.Format("[BSM] = '{0}'", keyValue)
                            });
                        }

                    }
                    else
                    {
                        Messages.Add(new VillageMessage
                        {
                            Value = keyValue,
                            CheckValue = reader[3].ToString().Trim(),
                            Description = string.Format("未获取田坎系数，无法核对【{0}】", Rule2),
                            WhereClause = string.Format("[BSM] = '{0}'", keyValue)
                        });
                    }

                    #endregion






                }
            }


            return true;
        }
    }
}
