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
            var field = this.FieldBox1.SelectedItem.ToString();
            if (string.IsNullOrEmpty(field))
            {
                return;
            }

            var file = _currentFile;
            if (file == null)
            {
                MessageBox.Show(string.Format("未读取分析到{0}对应的矢量数据文件,请核对", XZQMC));
                return;
            }
            var values = ADOSQLHelper.GetUniqueValue(file.FullName, _tableName, field);
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
            this.FieldBox1.Items.Clear();
            this.FieldBox2.Items.Clear();
            this.FieldBox3.Items.Clear();
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
            var array = fields.Select(i => i.Name).ToArray();
            FieldBox1.Items.AddRange(array);
            FieldBox2.Items.AddRange(array);
            FieldBox3.Items.AddRange(array);

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
            var field = this.FieldBox2.SelectedItem.ToString();
            if (_currentFile == null)
            {
                return;
            }
            var values = ADOSQLHelper.GetUniqueValue(_currentFile.FullName, _tableName, field);
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
            var field = this.FieldBox3.SelectedItem.ToString();

            if (_currentFile == null)
            {
                return;
            }
            var values = ADOSQLHelper.GetUniqueValue(_currentFile.FullName, _tableName, field);
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
            var sb = new StringBuilder();
            var flag = false;
            if (this.FieldBox1.SelectedItem != null&&this.ConditionBox1.SelectedItem!=null)
            {
                var value1 = this.ValueBox1.SelectedItem == null ? this.ValueBox1.Text : this.ValueBox1.SelectedItem.ToString();
                var fieldName1 = this.FieldBox1.SelectedItem.ToString();
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
                            var fieldName2 = this.FieldBox2.SelectedItem.ToString();
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
                            var fieldName3 = this.FieldBox3.SelectedItem.ToString();
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
            var sqlText = string.Empty;
            if (!string.IsNullOrEmpty(whereClause))
            {
                sqlText = string.Format("SELECT {0} FROM {1} WHERE {2}",string.Join(",", _fields.Select(i => i.Name).ToArray()), _tableName, whereClause);
            }
            else
            {
                sqlText = string.Format("SELECT {0} FROM {1}", string.Join(",", _fields.Select(i => i.Name).ToArray()), _tableName);
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

            var infos = new List<SearchInfo>();
            foreach(var field in _fields)
            {
                if (field.Type == FieldType.Float||field.Type==FieldType.Int)
                {
                    infos.Add(new SearchInfo { Field = field, TableName = _tableName, WhereClause = whereClause });
                }
            }
            infos = ADOSQLHelper.Query(_currentFile.FullName, infos);
            this.listView1.Items.Clear();
            foreach(var item in infos)
            {
                this.listView1.Items.Add(item.Title);
            }
            //this.listBox1.Items.Clear();
            //this.listBox1.Items.AddRange(infos.Select(i => i.Title).ToArray());
        }

        private void Closebutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ClearWherebutton_Click(object sender, EventArgs e)
        {
            this.FieldBox1.Items.Clear();
            this.FieldBox2.Items.Clear();
            this.FieldBox3.Items.Clear();
            var array = _fields.Select(i => i.Name).ToArray();
            this.FieldBox1.Items.AddRange(array);
            this.FieldBox2.Items.AddRange(array);
            this.FieldBox3.Items.AddRange(array);
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for(var i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].HeaderCell.Value = (i+1).ToString();
                
            }
        }
    }
}
