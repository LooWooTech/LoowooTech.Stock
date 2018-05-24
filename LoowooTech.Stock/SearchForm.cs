using LoowooTech.Stock.ArcGISTool;
using LoowooTech.Stock.Common;
using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LoowooTech.Stock
{
    public partial class SearchForm : Form
    {
        /// <summary>
        /// 矢量数据存放的文件夹
        /// </summary>
        private string _sourcePath { get; set; }
        /// <summary>
        /// 读取分析到的矢量数据列表
        /// </summary>
        private List<StockFile> _files { get; set; }
        /// <summary>
        /// 选择行政区名称
        /// </summary>
        private string XZQMC { get; set; }
        /// <summary>
        /// 选择查询的表格
        /// </summary>
        private string _tableName { get; set; }
        /// <summary>
        /// 当前选择的区县文件信息
        /// </summary>
        private StockFile _currentFile { get; set; }
        private List<Field> _fields { get; set; }
        public SearchForm()
        {
            InitializeComponent();
            _sourcePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datas");
        }

        private void comboBox5_DropDown(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(XZQMC))
            {
                return;
            }
            if (string.IsNullOrEmpty(_tableName))
            {
                return;
            }
            if (this.FieldBox1.SelectedItem == null)
            {
                return;
            }
            var fieldName =GetName(this.FieldBox1.SelectedItem.ToString());
            if (string.IsNullOrEmpty(fieldName))
            {
                return;
            }
            var field = _fields.FirstOrDefault(i => i.Name.ToLower() == fieldName.ToLower());
            if (field as object == null || field.Type == FieldType.Float)
            {
                return;
            }

            var file = _currentFile;
            if (file == null)
            {
                MessageBox.Show(string.Format("未读取分析到{0}对应的矢量数据文件,请核对", XZQMC));
                return;
            }
            var values = ADOSQLHelper.GetUniqueValue(file.FullName, _tableName, fieldName);
            this.ValueBox1.Items.Clear();
            this.ValueBox1.Items.AddRange(values.ToArray());
            
        }

        private void SearchForm_Load(object sender, EventArgs e)
        {
            LoadTables();
            LoadFiles();
        }

        private void LoadTables()
        {
            var tables = ParameterManager.TableFullNames;
            if (tables == null || tables.Count == 0)
            {
                MessageBox.Show("初始化属性表失败");
                return;
            }

            this.TableBox.Items.Clear();
            this.TableBox.Items.AddRange(tables.ToArray());

        }

        private void LoadFiles()
        {
            var files = FolderExtensions.GetFiles(_sourcePath, "*.mdb");
            if (files == null || files.Count == 0)
            {
                MessageBox.Show("Datas文件夹中不存在相关文件列表，请导入");
                return;
            }
            _files = files;
            this.DistrictBox.Items.Clear();
            this.DistrictBox.Items.AddRange(files.Select(e => e.XZQMC).ToArray());
        }

        /// <summary>
        /// 表名发生改变的时候
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TableBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tableFullName = this.TableBox.SelectedItem.ToString();
            Clear();
            _tableName = string.Empty;
            _fields = null;
            if (string.IsNullOrEmpty(tableFullName))
            {
                MessageBox.Show("请选择属性表名");
                return;
            }
            var tableName = GetTableName(tableFullName);
            if (string.IsNullOrEmpty(tableName))
            {
                MessageBox.Show("未获取属性表名称");
                return;
            }
            this._tableName = tableName;
            var fields = GetField(tableName);
            _fields = fields;
            var array = fields.Select(i => string.Format("{0}({1})",i.Title,i.Name)).ToArray();
            FieldBox1.Items.AddRange(array);
            FieldBox2.Items.AddRange(array);
            FieldBox3.Items.AddRange(array);

            var statistic = _fields.Where(i => i.Type == FieldType.Float || i.Type == FieldType.Int).ToList();
            this.StatisticcheckedListBox.Items.Clear();
            foreach(var item in statistic)
            {
                this.StatisticcheckedListBox.Items.Add(string.Format("{0}({1})",item.Title,item.Name),true);
            }

            var groups = _fields.Where(i => i.Type == FieldType.Int || i.Type == FieldType.Char).ToList();

            var garray = groups.Select(i =>string.Format("{0}({1})",i.Title,i.Name)).ToArray();
            GroupFieldBox1.Items.Clear();
            GroupFieldBox2.Items.Clear();
            GroupFieldBox1.Items.AddRange(garray);
            GroupFieldBox2.Items.AddRange(garray);




        }
        private string GetName(string fullName)
        {
            var index = fullName.IndexOf("(");
            fullName = fullName.Replace(")", "");
            return fullName.Substring(index + 1);
        }

        private string GetTableName(string fullTableName)
        {
            var index = fullTableName.IndexOf("(");
            fullTableName = fullTableName.Replace(")", "");
            return fullTableName.Substring(index+1);
        }
        private List<Field> GetField(string tableName)
        {
            return XmlClass.GetField(tableName);
        }

        private void DistrictBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.DistrictBox.SelectedItem != null)
            {
                this.XZQMC = this.DistrictBox.SelectedItem.ToString();
            }
            else
            {
                this.XZQMC = string.Empty;
            }

            _currentFile = _files.FirstOrDefault(i => i.XZQMC.ToLower() == this.XZQMC.ToLower());
        }

        private void ValueBox2_DropDown(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(XZQMC))
            {
                return;
            }
            if (string.IsNullOrEmpty(_tableName))
            {
                return;
            }
            if (this.FieldBox2.SelectedItem == null)
            {
                return;
            }
            var fieldName =GetName(this.FieldBox2.SelectedItem.ToString());
            var field = _fields.FirstOrDefault(i => i.Name.ToLower() == fieldName.ToLower());
            if (field as object == null||field.Type==FieldType.Float)
            {
                return;
            }
            if (_currentFile == null)
            {
                return;
            }
            var values = ADOSQLHelper.GetUniqueValue(_currentFile.FullName, _tableName, fieldName);
            this.ValueBox2.Items.Clear();
            this.ValueBox2.Items.AddRange(values.ToArray());

        }

        private void ValueBox3_DropDown(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(XZQMC))
            {
                return;
            }
            if (string.IsNullOrEmpty(_tableName))
            {
                return;
            }
            if (this.FieldBox3.SelectedItem == null)
            {
                return;
            }
            var fieldName = GetName(this.FieldBox3.SelectedItem.ToString());
            var field = _fields.FirstOrDefault(i => i.Name.ToLower() == fieldName.ToLower());
            if (field as object == null || field.Type == FieldType.Float)
            {
                return;
            }
            if (_currentFile == null)
            {
                return;
            }
            var values = ADOSQLHelper.GetUniqueValue(_currentFile.FullName, _tableName, fieldName);
            this.ValueBox3.Items.Clear();
            this.ValueBox3.Items.AddRange(values.ToArray());
        }

        private void Searchbutton_Click(object sender, EventArgs e)
        {
            if (_fields == null)
            {
                MessageBox.Show("未获取字段信息列表！");
                return;
            }
            if (_currentFile == null)
            {
                MessageBox.Show("请选择需要查询的行政区");
                return;
            }
            var statistics = GetStatisticField();
            if (statistics.Count == 0)
            {
                MessageBox.Show("请选择统计字段");
                return;
            }
            if (this.GroupFieldBox1.SelectedItem == null && this.GroupFieldBox2.SelectedItem == null)
            {
                MessageBox.Show("至少请选择一个分组字段");
                return;
            }
            var groups = new List<Field>();
            if (this.GroupFieldBox1.SelectedItem != null)
            {
                var a = GetName(this.GroupFieldBox1.SelectedItem.ToString());
                var field = _fields.FirstOrDefault(i => i.Name.ToLower() == a.ToLower());
                if(field as object != null)
                {
                    groups.Add(field);
                }
              
            }
            if (this.GroupFieldBox2.SelectedItem != null)
            {
                var a = GetName(this.GroupFieldBox2.SelectedItem.ToString());
                var field = _fields.FirstOrDefault(i => i.Name.ToLower() == a.ToLower());
                if (field as object != null)
                {
                    groups.Add(field);
                }
            }









            #region 获取查询条件
            var sb = new StringBuilder();
            var flag = false;
            if (this.FieldBox1.SelectedItem != null&&this.ConditionBox1.SelectedItem!=null)
            {
                var value1 = this.ValueBox1.SelectedItem == null ? this.ValueBox1.Text : this.ValueBox1.SelectedItem.ToString();
                var fieldName1 = GetName(this.FieldBox1.SelectedItem.ToString());
                var field1 = _fields.FirstOrDefault(i => i.Name.ToLower() == fieldName1.ToLower());
                if ((field1 as object) != null)
                {
                    if (field1.Type == FieldType.Char)
                    {
                        value1 = "'" + value1 + "'";
                        sb.AppendFormat("{0} {1} {2}", fieldName1, this.ConditionBox1.SelectedItem.ToString(), value1);
                        flag = true;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(value1))
                        {
                            flag = true;
                            sb.AppendFormat("{0} {1} {2}", fieldName1, this.ConditionBox1.SelectedItem.ToString(), value1);
                        }
                    }
                    if (flag)
                    {
                        #region 条件二
                        if (this.RelationBox1.SelectedItem != null && this.FieldBox2.SelectedItem != null && this.ConditionBox2.SelectedItem != null)
                        {
                            var value2 = this.ValueBox2.SelectedItem == null ? this.ValueBox2.Text : this.ValueBox2.SelectedItem.ToString();
                            var fieldName2 =GetName(this.FieldBox2.SelectedItem.ToString());
                            var field2 = _fields.FirstOrDefault(i => i.Name.ToLower() == fieldName2.ToLower());
                            if ((field2 as object) != null)
                            {
                                if (field2.Type == FieldType.Char)
                                {
                                    value2 = "'" + value2 + "'";
                                    sb.AppendFormat("{0} {1} {2} {3}", this.RelationBox1.SelectedItem.ToString(), fieldName2, this.ConditionBox2.SelectedItem.ToString(), value2);
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(value2))
                                    {
                                        sb.AppendFormat("{0} {1} {2} {3}", this.RelationBox1.SelectedItem.ToString(), fieldName2, this.ConditionBox2.SelectedItem.ToString(), value2);
                                    }
                                }
                            }
                        }
                        #endregion

                        #region 条件三
                        if (this.RelationBox2.SelectedItem != null && this.FieldBox3.SelectedItem != null && this.ConditionBox3.SelectedItem != null)
                        {
                            var value3 = this.ValueBox3.SelectedItem == null ? this.ValueBox3.Text : this.ValueBox3.SelectedItem.ToString();
                            var fieldName3 = GetName(this.FieldBox3.SelectedItem.ToString());
                            var field3 = _fields.FirstOrDefault(i => i.Name.ToLower() == fieldName3.ToLower());
                            if ((field3 as object ) != null)
                            {
                                if (field3.Type == FieldType.Char)
                                {
                                    value3 = "'" + value3 + "'";
                                    sb.AppendFormat("{0} {1} {2} {3}", this.RelationBox2.SelectedItem.ToString(), fieldName3, this.ConditionBox3.SelectedItem.ToString(), value3);
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(value3))
                                    {
                                        sb.AppendFormat("{0} {1} {2} {3}", this.RelationBox2.SelectedItem.ToString(), fieldName3, this.ConditionBox3.SelectedItem.ToString(), value3);
                                    }
                                }
                            }

                        }
                        #endregion
                    }
                }
            }
            var whereClause = sb.ToString();
            Console.WriteLine(whereClause);
            #endregion
            var sqlText = string.Empty;
            if (!string.IsNullOrEmpty(whereClause))
            {
                sqlText = string.Format("SELECT {0},{1},COUNT(*) as 数量 FROM {2} WHERE {3} GROUP BY {4}",string.Join(",", statistics.Select(i=>string.Format("SUM({0}) as {1}",i.Name,i.Title)).ToArray()),string.Join(",", groups.Select(i=>string.Format("{0} as {1}",i.Name,i.Title)).ToArray()), _tableName, whereClause,string.Join(",",groups.Select(i=>i.Name).ToArray()));
            }
            else
            {
                sqlText = string.Format("SELECT {0},{1},COUNT(*) as 数量 FROM {2} GROUP BY {3}", string.Join(",", statistics.Select(i => string.Format("SUM({0}) as {1}", i.Name,i.Title)).ToArray()), string.Join(",", groups.Select(i => string.Format("{0} as {1}", i.Name, i.Title)).ToArray()), _tableName, string.Join(",", groups.Select(i => i.Name).ToArray()));
            }
            Console.WriteLine(sqlText);
            using (var connection=new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", _currentFile.FullName)))
            {
                var reader = new OleDbDataAdapter(sqlText, connection);
                DataSet ds = new DataSet();
                reader.Fill(ds,"T_Class");
                this.dataGridView1.DataSource = ds;
                this.dataGridView1.DataMember = "T_Class";
            }
            this.ExportExcelbutton.Enabled = true;
        }

        private List<Field> GetStatisticField()
        {
            var list = new List<Field>();
            for(var i = 0; i < StatisticcheckedListBox.Items.Count; i++)
            {
                if (this.StatisticcheckedListBox.GetItemChecked(i))
                {
                    var val = GetName(this.StatisticcheckedListBox.GetItemText(this.StatisticcheckedListBox.Items[i]));
                    var field = _fields.FirstOrDefault(j => j.Name.ToLower() == val.ToLower());
                    if(field as object != null)
                    {
                        list.Add(field);
                    }
                    //list.Add(val);
                }
            }

            return list;
        }



        private void Clear()
        {
            this.FieldBox1.Items.Clear();
            this.FieldBox1.Text = string.Empty;
            this.FieldBox2.Items.Clear();
            this.FieldBox2.Text = string.Empty;
            this.FieldBox3.Items.Clear();
            this.FieldBox3.Text = string.Empty;

            this.ConditionBox1.Text = string.Empty;
            this.ConditionBox2.Text = string.Empty;
            this.ConditionBox3.Text = string.Empty;

            this.RelationBox1.Text = string.Empty;
            this.RelationBox2.Text = string.Empty;

            this.ValueBox1.Text = string.Empty;
            this.ValueBox2.Text = string.Empty;
            this.ValueBox3.Text = string.Empty;

            this.GroupFieldBox1.Text = string.Empty;
            this.GroupFieldBox2.Text = string.Empty;
        }

        private void ClearWherebutton_Click(object sender, EventArgs e)
        {
            this.FieldBox1.Text = string.Empty;
            this.FieldBox2.Text = string.Empty;
            this.FieldBox3.Text = string.Empty;
            

            this.ConditionBox1.Text = string.Empty;
            this.ConditionBox2.Text = string.Empty;
            this.ConditionBox3.Text = string.Empty;

            this.RelationBox1.Text = string.Empty;
            this.RelationBox2.Text = string.Empty;

            this.ValueBox1.Text = string.Empty;
            this.ValueBox2.Text = string.Empty;
            this.ValueBox3.Text = string.Empty;

    

            this.FieldBox1.SelectedItem = null;
            this.FieldBox2.SelectedItem = null;
            this.FieldBox3.SelectedItem = null;


            this.ConditionBox1.SelectedItem = null;
            this.ConditionBox2.SelectedItem = null;
            this.ConditionBox3.SelectedItem = null;

            this.ValueBox1.SelectedItem = null;
            this.ValueBox2.SelectedItem = null;
            this.ValueBox3.SelectedItem = null;

            this.RelationBox1.SelectedItem = null;
            this.RelationBox2.SelectedItem = null;

        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for(var i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].HeaderCell.Value = (i+1).ToString();
                
            }
        }

        private void ExportExcelbutton_Click(object sender, EventArgs e)
        {
            var saveFile = DialogClass.SaveFile("2003Excel文件|*.xls", "请选择保存文件路径");
            if (string.IsNullOrEmpty(saveFile))
            {
                return;
            }
            var modelFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Excels", System.Configuration.ConfigurationManager.AppSettings["Statistic"]);
            try
            {
                ExcelParameterManager.ExportExcel(modelFile, saveFile, this.dataGridView1);

            }
            catch(Exception ex)
            {
                MessageBox.Show("发生错误：" + ex.Message);
                return;
            }
            MessageBox.Show("成果导出！");
         

        }
    }
}
