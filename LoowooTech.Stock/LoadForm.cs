using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LoowooTech.Stock
{
    public partial class LoadForm : Form
    {
        //private Thread _th { get; set; }
        public LoadForm()
        {
            InitializeComponent();
        }

        private void LoadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (_th != null)
            //{
            //    while (!_th.IsAlive) ;
            //    _th.Join();
            //}
        }

        private void LoadForm_Load(object sender, EventArgs e)
        {
            //_th = new Thread(DoData);
            //_th.IsBackground = true;
            //_th.Start(100);
        }
        //private delegate void DoDataDelegate(object number);

        //private void DoData(object number)
        //{
        //    if (progressBar1.InvokeRequired)
        //    {
        //        DoDataDelegate d = DoData;
        //        progressBar1.Invoke(d, number);
        //    }
        //    else
        //    {
        //        progressBar1.Maximum = (int)number;
        //        for(var i = 0; i < (int)number; i++)
        //        {
        //            progressBar1.Value = i;
        //            Application.DoEvents();
        //        }
        //    }
        //}
    }
}
