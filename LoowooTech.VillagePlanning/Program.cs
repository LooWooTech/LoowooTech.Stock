using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;

namespace LoowooTech.VillagePlanning
{
    static class Program
    {

        private static MainForm _form { get; set; }
        public static LicenseInitializer m_AOLicenseInitializer { get; set; } = new LicenseInitializer();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!RuntimeManager.Bind(ProductCode.Desktop))
            {
                if (!RuntimeManager.Bind(ProductCode.Engine))
                {
                    MessageBox.Show("当前基期上无法找到ArcGIS授权！");
                    return;
                }
            }
            try
            {
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB, esriLicenseProductCode.esriLicenseProductCodeAdvanced }, new esriLicenseExtensionCode[] { esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst });

                var load = new LoadForm();
                load.Show();
                Application.DoEvents();
                _form = new MainForm();
                Application.Run(_form);
                m_AOLicenseInitializer.ShutdownApplication();
            }
            catch(Exception ex)
            {
                string str = "";
                string strDateInfo = "出现应用程序未处理的异常：" + DateTime.Now.ToString() + "\r\n";

                if (ex != null)
                {
                    str = string.Format(strDateInfo + "异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n",
                         ex.GetType().Name, ex.Message, ex.StackTrace);
                }
                else
                {
                    str = string.Format("应用程序线程错误:{0}", ex);
                }


                WriteLog(str);
                MessageBox.Show("发生未处理异常，请及时联系软件维护人员！", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static void WriteLog(string str)
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LoowooTech" + "\\VillagePlanning";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            using (StreamWriter sw = new StreamWriter(folder + "\\log.txt", true))
            {
                sw.WriteLine(string.Format("[{0:yyyyMMdd HH:mm:ss}]", DateTime.Now));
                sw.WriteLine(str);
                sw.WriteLine("---------------------------------------------------------");
                sw.Close();
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {

            string str = "";
            string strDateInfo = "出现应用程序未处理的异常：" + DateTime.Now.ToString() + "\r\n";
            Exception error = e.Exception as Exception;
            if (error != null)
            {
                str = string.Format(strDateInfo + "异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n",
                     error.GetType().Name, error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("应用程序线程错误:{0}", e);
            }

            WriteLog(str);
            MessageBox.Show("发生未处理异常，请及时联系软件维护人员！", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string str = "";
            Exception error = e.ExceptionObject as Exception;
            string strDateInfo = "出现应用程序未处理的异常：" + DateTime.Now.ToString() + "\r\n";
            if (error != null)
            {
                str = string.Format(strDateInfo + "Application UnhandledException:{0};\n\r堆栈信息:{1}", error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("Application UnhandledError:{0}", e);
            }

            WriteLog(str);
            MessageBox.Show("发生未处理异常，请及时联系软件维护人员！", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
