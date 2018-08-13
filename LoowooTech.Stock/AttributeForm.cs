using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using LoowooTech.Stock.Tools;

namespace LoowooTech.Stock
{
    public partial class AttributeForm : Form
    {
        public AttributeForm(DataTable dataTable,string name)
        {
            InitializeComponent();
            this.dataGridView1.DataSource = dataTable;
            this.Text = name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //SaveFileDialog sf = new SaveFileDialog();
            //sf.Filter = "Excel files(*.xls)|*.xlsx";
            //if (sf.ShowDialog() == DialogResult.OK)
            //{
            //    if (dataGridView1.Rows.Count == 0)//判断是否有数据
            //        return;//返回
            //    IWorkbook excel = new HSSFWorkbook();
            //    ISheet sheet = excel.CreateSheet("sheet1");

            //    DataTable dt = (DataTable)dataGridView1.DataSource;
            //    IRow headerRow = sheet.CreateRow(0);
            //    foreach (DataColumn column in dt.Columns)
            //    {
            //        headerRow.CreateCell(column.Ordinal).SetCellValue(column.Caption);

            //    }

            //    int rowIndex = 1;
            //    foreach (DataRow row in dt.Rows)
            //    {
            //        IRow dataRow = sheet.CreateRow(rowIndex);
            //        foreach (DataColumn column in dt.Columns)
            //        {
            //            dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
            //        }
            //        rowIndex++;
            //    }

            //    FileStream fl = new FileStream(sf.OpenFile().ToString(), FileMode.Append);

            //    excel.Write(fl);


            //    fl.Flush();
            //    fl.Close();
            //    MessageBox.Show("导出成功", "提示！");
            //}
            ExportExcel ex = new ExportExcel();
            ex.ExportToExcel(dataGridView1);

            //SaveFileDialog sf = new SaveFileDialog();
            //sf.Filter = "EXCEL 文件(*.xls)|*.xls|Excel 文件(*.xlsx)|*.xlsx";
            //HSSFWorkbook wb = new HSSFWorkbook();
            //HSSFSheet sheet = (HSSFSheet)wb.CreateSheet("sheet1");
            //if (sf.ShowDialog() == DialogResult.OK)
            //{
            //    if (dataGridView1.Rows.Count == 0)
            //    {
            //        return;
            //    }
            //    HSSFRow headRow = (HSSFRow)sheet.CreateRow(0);
            //    for (int i = 0; i < dataGridView1.Columns.Count; i++)
            //    {
            //        HSSFCell headCell = (HSSFCell)headRow.CreateCell(i, CellType.String);
            //        headCell.SetCellValue(dataGridView1.Columns[i].HeaderText);
            //    }
            //    for (int i = 0; i < dataGridView1.Rows.Count-1; i++)
            //    {
            //        HSSFRow row = (HSSFRow)sheet.CreateRow(i + 1);
            //        for (int j = 0; j < dataGridView1.Columns.Count; j++)
            //        {
            //            HSSFCell cell = (HSSFCell)row.CreateCell(j);
            //            if (dataGridView1[j, i].ValueType == typeof(string))
            //            {
            //                cell.SetCellValue(dataGridView1.Rows[i].Cells[j].Value.ToString());
            //            }
            //            else
            //            {
            //                cell.SetCellValue(dataGridView1.Rows[i].Cells[j].Value.ToString());
            //            }
            //        }
            //    }

            //    for (int i = 0; i < dataGridView1.Columns.Count; i++)
            //    {
            //        sheet.AutoSizeColumn(i);
            //    }

            //    FileStream fs = new FileStream(sf.FileName, FileMode.OpenOrCreate, FileAccess.Write);

            //    wb.Write(fs);


            //    MessageBox.Show("导出成功");
            //}
        }

        private void panel2_Resize(object sender, EventArgs e)
        {
            dataGridView1.Size = new Size(panel2.Bounds.Width, panel2.Bounds.Height);
        }

        private void AttributeForm_Load(object sender, EventArgs e)
        {
            dataGridView1.Size = new Size(panel2.Bounds.Width, panel2.Bounds.Height);
        }
    }
    
}
