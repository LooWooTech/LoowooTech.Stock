using System;
using System.Configuration;
using System.Threading;
using System.Windows.Forms;

namespace LoowooTech.Updater
{
    public delegate void UpdateProgressDelegate(UpdateProgressChangedEventArgs e);

    public partial class UpdateForm : Form
    {
        private UpdateManager _updateManager;

        private bool _stopSignal = false;

        public UpdateForm()
        {
            InitializeComponent();
            Text = ConfigurationManager.AppSettings["Name"];
            _updateManager = new UpdateManager();
            _updateManager.OnUpdateProgressChangedEvent += UpdateManager_OnUpdateProgressChangedEvent;
        }

        private void UpdateManager_OnUpdateProgressChangedEvent(object sender, UpdateProgressChangedEventArgs e)
        {
            UpdateProgress(e);            
        }

        private void UpdateProgress(UpdateProgressChangedEventArgs e)
        {
            if(lblMessage.InvokeRequired)
            {
                Invoke(new UpdateProgressDelegate(UpdateProgress), e);
            }
            else
            {
                if (lblMessage.Text != e.Message) lblMessage.Text = e.Message;

                var progress = (int)((double)e.CurrentProgress / e.TotalProgress * 100);
                if (progress != progressBar1.Value && progress <= 100) progressBar1.Value = progress;
                if (_stopSignal) e.NeedCancel = true;

                if(e.State == UpdateStateChangeStateEnum.Done)
                {
                    btnUpdate.Text = "开始更新";
                    btnUpdate.Enabled = false;
                }
                else if(e.State == UpdateStateChangeStateEnum.Fail)
                {
                    btnUpdate.Text = "开始更新";
                    btnUpdate.Enabled = true;
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _updateManager.GetLocalMetadata();
            Text = string.Format("{0} (当前版本:{1})", Text, _updateManager.LocalVersion.Name);
            txtLog.Text = _updateManager.LocalVersion.ChangeLog;

            GetRemoteVersion();            
        }

        private void GetRemoteVersion()
        {
            try
            {
                _updateManager.GetMetadata();
                if(_updateManager.RemoteVersion == null)
                {
                    throw new Exception("服务器无法访问");
                }
                txtLog.Text = _updateManager.RemoteVersion.ChangeLog.Replace("<br>", "\r\n");
                if (_updateManager.NeedUpdate)
                {
                    lblMessage.Text = "发现新版本:" + _updateManager.RemoteVersion.Name + ", 单击'开始更新'按钮进行更新";
                }
                else
                {
                    lblMessage.Text = "当前版本已经是最新版本。";
                }
                btnUpdate.Enabled = _updateManager.NeedUpdate;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "获取更新时出错:" + ex.Message;
                btnCheck.Enabled = true;
                btnUpdate.Enabled = false;
            }
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            GetRemoteVersion();
        }

        private Thread _thread;

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (btnUpdate.Text == "开始更新")
            {
                if(_updateManager.NeedUpdate == false)
                {
                    MessageBox.Show("当前已经是最新版本，无需更新", "更新", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                _thread = new Thread(StartUpdate);
                _thread.Start();
                btnUpdate.Text = "停止更新";
            }
            else
            {
                if (MessageBox.Show("当前操作将取消本次更新，是否确认?", "确认停止", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
                _stopSignal = true;
                while(_thread == null || _thread.IsAlive == true)
                {
                    Application.DoEvents();
                }
                btnUpdate.Text = "开始更新";
            }
        }

        private void StartUpdate()
        {
            _updateManager.StartUpdate();
        }
    }
}
