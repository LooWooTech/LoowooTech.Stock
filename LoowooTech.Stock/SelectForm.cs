using LoowooTech.Stock.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LoowooTech.Stock
{
    public partial class SelectForm : Form
    {

        private AdvanceForm _father { get; set; }
        public AdvanceForm Father { get { return _father == null ? _father = this.Owner as AdvanceForm : _father; } }
        public SelectForm()
        {
            InitializeComponent();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Father.IsAttribute = false;
            this.Close();
        }

        private void SelectForm_Load(object sender, EventArgs e)
        {
            this.comboBoxXZ.Items.Clear();
            foreach(var item in ExcelManager.XZQ)
            {
                this.comboBoxXZ.Items.Add(item.XZCMC);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            if (this.comboBoxDCDYLX.SelectedItem != null)
            {
                var D = this.comboBoxDCDYLX.SelectedItem.ToString();
                if (!string.IsNullOrEmpty(D))
                {
                    switch (D)
                    {
                        case "撤并型":
                            sb.Append("DCDYLX = '1'");
                            break;
                        case "保留型":
                            sb.Append("DCDYLX = '2'");
                            break;
                        case "集聚型":
                            sb.Append("DCDYLX = '3'");
                            break;
                    }
                }
            }
            if (this.comboBoxXZ.SelectedItem != null)
            {
                var XZ = this.comboBoxXZ.SelectedItem.ToString();
                if (!string.IsNullOrEmpty(XZ))
                {
                    var XZQ = ExcelManager.XZQ.FirstOrDefault(j => j.XZCMC.ToLower() == XZ.ToLower());
                    if (XZQ != null)
                    {
                        var key = XZQ.XZCMC + "," + XZQ.XZCDM;
                        if (ExcelManager.Dict.ContainsKey(key))
                        {
                            var XZCList = ExcelManager.Dict[key];
                            var temp = new List<string>();
                            foreach (var item in XZCList)
                            {
                                temp.Add(string.Format("XZCDM = '{0}'", item.XZCDM));
                            }

                            if (sb.Length > 0)
                            {
                                sb.AppendFormat("AND ({0})", string.Join(" OR ", temp.ToArray()));
                            }
                            else
                            {
                                sb.Append(string.Join(" OR ", temp.ToArray()));
                            }
                        }
                    }
                }
            }
           
            var whereClause = sb.ToString();
            Father.Search(whereClause);
        }
    }
}
