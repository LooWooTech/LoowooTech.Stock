using System.Windows.Forms;

namespace LoowooTech.VillagePlanning
{
    public partial class LoadForm : Form
    {
        public LoadForm()
        {
            InitializeComponent();
            _instance = this;
        }

        private static LoadForm _instance { get; set; }
        public static LoadForm Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
