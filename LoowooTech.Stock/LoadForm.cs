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
            _instance = this;
        }


        private static LoadForm _instance;
        public static LoadForm Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
