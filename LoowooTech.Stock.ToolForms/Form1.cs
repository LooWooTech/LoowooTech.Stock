
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LoowooTech.Stock.ToolForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fileName = string.Empty;
            var openfileDialog = new OpenFileDialog();
            openfileDialog.Filter = "矢量数据文件|*.mdb";
            openfileDialog.Title = "请选择农村存量建设用地调查数据成果数据库";
            if (openfileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = openfileDialog.FileName;
            }
            this.textBox1.Text = fileName;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBox1.Text) || !System.IO.File.Exists(this.textBox1.Text))
            {
                MessageBox.Show("请选择矢量数据库文件，或当前矢量数据库文件不存在，请核对！");
                return;
            }
            this.buttonOK.Enabled = false;
            Search(this.textBox1.Text);
            this.buttonOK.Enabled = true;

        }
        private List<DCDYTB> GainDCDYTB(OleDbConnection connection)
        {
            var list = new List<DCDYTB>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = string.Format("Select XZCDM,TBBH From DCDYTB");
                var reader = command.ExecuteReader();
              

                while (reader.Read())
                {
                    if (reader[0] != null && reader[1] != null)
                    {
                        list.Add(new DCDYTB { XZCDM = reader[0].ToString(), TBBH = reader[1].ToString() });
                    }
                }
                
            }
            return list;
        }

        private object Gain(OleDbConnection connection,string SQLText)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = SQLText;

                return command.ExecuteScalar();
            }
        }
        private OleDbDataReader Read(OleDbConnection connection,string SQLText)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = SQLText;
                var reader = command.ExecuteReader();
                return reader;
            }
        }

        private int NoQuery(OleDbConnection connection,string SQLText)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = SQLText;
                return command.ExecuteNonQuery();
            }
        }

        private void Search(string filePath)
        {
            using (var connection=new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", filePath)))
            {
                connection.Open();
                var list = GainDCDYTB(connection);
                object obj = null;
                int a = 0;
                var i = 0;
                //double a = 0.0;//宅基地
                //double b = 0.0;//经营性建设而用地
                //double c = 0.0;//公共管理及公共服务设施用地
                //double d = 0.0;//其它存量建设用地
                foreach (var item in list)
                {
                    List<string> YT = new List<string>();
                    OleDbDataReader reader = null;
                    List<string> temp = null;
                    a = 0;
                    obj = Gain(connection, string.Format("Select COUNT(*) FROM CLZJD where XZCDM = '{0}' AND TBBH = '{1}'", item.XZCDM, item.TBBH));
                    if (obj != null&&int.TryParse(obj.ToString(),out a)&&a>0)
                    {
                        YT.Add("宅基地");
                    }
                    else
                    {
                        reader = Read(connection, string.Format("Select TDYT FROM JYXJSYD where XZCDM = '{0}' AND TBBH = '{1}'", item.XZCDM, item.TBBH));
                        temp = GetValues(reader, 0);
                        if (temp.Count > 0)
                        {
                            YT.AddRange(temp);
                        }
                        else
                        {
                            reader = Read(connection, string.Format("Select TDYT FROM GGGL_GGFWSSYD where XZCDM = '{0}' AND TBBH = '{1}'", item.XZCDM, item.TBBH));
                            temp = GetValues(reader, 0);
                            if (temp.Count > 0)
                            {
                                YT.AddRange(temp);
                            }
                            else
                            {
                                reader = Read(connection, string.Format("Select TDYT FROM QTCLJSYD where XZCDM = '{0}' AND TBBH = '{1}'", item.XZCDM, item.TBBH));
                                temp = GetValues(reader, 0);
                                if (temp.Count > 0)
                                {
                                    YT.AddRange(temp);
                                }
                            }
                        }
                    }

                    if (YT.Count > 0)
                    {
                        item.TDYT = string.Join(",", YT.Distinct().ToArray());
                    }
                    i++;
                    Console.WriteLine(i);
                  
                }
                i = 0;
                var tt = list.Where(e => !string.IsNullOrEmpty(e.TDYT)).ToList();
                foreach (var item in tt)
                {
                    var str = string.Format("UPDATE DCDYTB SET TDYT = '{0}' WHERE XZCDM = '{1}' AND TBBH = '{2}'", item.TDYT, item.XZCDM, item.TBBH);
                    i = NoQuery(connection, str);
                    if (i <= 0)
                    {
                        Console.WriteLine(string.Format("执行SQL语句：【{0}】失败！", str));
                    }
                }
                connection.Close();
            }
        }

        private List<string> GainTDYT(string XZCDM,string TBBH,OleDbConnection connection)
        {
            var list = new List<string>();
            var reader = Read(connection, string.Format("Select TDYT FROM JYXJSYD where XZCDM = '{0}' AND TBBH = '{1}'", XZCDM, TBBH));
            var temp = GetValues(reader, 0);
            list.AddRange(temp.Distinct());

            reader = Read(connection, string.Format("Select TDYT FROM GGGL_GGFWSSYD where XZCDM = '{0}' AND TBBH = '{1}'", XZCDM, TBBH));
            temp = GetValues(reader, 0);
            list.AddRange(temp.Distinct());

            reader = Read(connection, string.Format("Select TDYT FROM QTCLJSYD where XZCDM = '{0}' AND TBBH = '{1}'", XZCDM, TBBH));
            temp = GetValues(reader, 0);
            list.AddRange(temp.Distinct());

            return list.Distinct().ToList();
        }

        public static List<string> GetValues(OleDbDataReader reader, int line)
        {
            var list = new List<string>();
            while (reader.Read())
            {
                if (reader[line] != null)
                {
                    list.Add(reader[line].ToString().Trim());
                }
            }
            return list;
        }
    }
}
