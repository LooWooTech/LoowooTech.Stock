using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LoowooTech.Stock.Tools
{
    class ExportExcel
    {
        public void ExportToExcel(System.Windows.Forms.DataGridView dvg)
        {
            if (dvg.DataSource == null)
            {
                MessageBox.Show("未选中图斑或图斑无数据！");
                return;
            }
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "EXCEL 文件(*.xls)|*.xls";
            HSSFWorkbook wb = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)wb.CreateSheet("sheet1");
            if (sf.ShowDialog() == DialogResult.OK)
            {              
                if (dvg.Rows.Count == 0)
                {
                    return;
                }
                HSSFRow headRow = (HSSFRow)sheet.CreateRow(0);

                for (int i = 0; i < dvg.Columns.Count; i++)
                {
                    var cell = headRow.CreateCell(i, CellType.String);
                    cell.SetCellValue(dvg.Columns[i].HeaderText);

                    //HSSFCell headCell = (HSSFCell)headRow.CreateCell(i, CellType.String);
                    //headCell.SetCellValue(dvg.Columns[i].HeaderText);
                }
                for (int i = 0; i < dvg.Rows.Count - 1; i++)
                {
                    HSSFRow row = (HSSFRow)sheet.CreateRow(i + 1);
                    for (int j = 0; j < dvg.Columns.Count; j++)
                    {
                        HSSFCell cell = (HSSFCell)row.CreateCell(j);
                        if (dvg[j, i].ValueType == typeof(string))
                        {
                            cell.SetCellValue(dvg.Rows[i].Cells[j].Value.ToString());
                        }
                        else
                        {
                            cell.SetCellValue(dvg.Rows[i].Cells[j].Value.ToString());
                        }
                    }
                }

                for (int i = 0; i < dvg.Columns.Count; i++)
                {
                    sheet.AutoSizeColumn(i);
                }

                using (var fs = new FileStream(sf.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    wb.Write(fs);
                    fs.Close();
                }

                //FileStream fs = new FileStream(sf.FileName, FileMode.OpenOrCreate, FileAccess.Write);

                //wb.Write(fs);
                //fs.Close();

                MessageBox.Show("导出成功");
            }
        }
    
    }
}
