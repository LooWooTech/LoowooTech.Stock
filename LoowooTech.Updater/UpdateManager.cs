using Ionic.Zip;
using LoowooTech.Updater.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace LoowooTech.Updater
{
    public class UpdateManager
    {
        public event EventHandler<UpdateProgressChangedEventArgs> OnUpdateProgressChangedEvent;

        public ProductVersion LocalVersion { get; set; }

        public ProductVersion RemoteVersion { get; set; }

        public string ServerAddress { get; private set; }

        public string ProductId { get; private set; }

        private long _totalProgress;

        private long _progressValue;

        public bool _stopSignal = false;

        public UpdateManager(string server, string productId)
        {
            ServerAddress = server;
            ProductId = productId;
        }

        public UpdateManager() : this(ConfigurationManager.AppSettings["Server"], ConfigurationManager.AppSettings["ProductId"]) { }

        public bool NeedUpdate
        {
            get
            {
                return LocalVersion != null && RemoteVersion != null && LocalVersion.Build < RemoteVersion.Build;
            }
        }

        public void GetLocalMetadata()
        {
            using (var reader = new StreamReader("local.json"))
            {
                LocalVersion = JsonConvert.DeserializeObject<ProductVersion>(reader.ReadToEnd());
            }
        }

        public void GetMetadata()
        {
            RemoteVersion = GetMetadata(ServerAddress, ProductId);
            _progressValue = 0;
            _totalProgress = 0;
            foreach (var file in RemoteVersion.Files)
            {
                _totalProgress += file.Size;
            }
        }

        public static ProductVersion GetMetadata(string serverAddress, string productId)
        {
            using (var stream = new MemoryStream())
            {
                var url = string.Format("{0}/{1}/version.json", serverAddress, productId);
                if (Download2Stream(stream, url))
                {
                    var arr = stream.ToArray();
                    var str = Encoding.UTF8.GetString(arr, 3, arr.Length - 3);
                    return JsonConvert.DeserializeObject<ProductVersion>(str);
                }
            }
            return null;
        }
        
        private static bool Download2Stream(Stream stream, string url, bool overwrite = false)
        {
            //打开上次下载的文件
            long SPosition = 0;

            try
            {
                //打开网络连接
                var myRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                if (SPosition > 0)
                    myRequest.AddRange((int)SPosition);
                using (var myStream = myRequest.GetResponse().GetResponseStream())
                {
                    //定义一个字节数据
                    var btContent = new byte[512];
                    var intSize = myStream.Read(btContent, 0, 512);
                    while (intSize > 0)
                    {
                        stream.Write(btContent, 0, intSize);
                        intSize = myStream.Read(btContent, 0, 512);
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public void StartUpdate()
        {
            _stopSignal = false;
            var localFiles = new List<string>();
            _progressValue = 0;
            _totalProgress = (long)(_totalProgress * 1.1);

            OnUpdateProgressChangedEvent(this, new UpdateProgressChangedEventArgs { CurrentProgress = _progressValue, TotalProgress = _totalProgress, Message = "开始更新", State = UpdateStateChangeStateEnum.InProgress });


            foreach (var file in RemoteVersion.Files)
            {
                for (var i = 0; i < 3; i++)
                {                    
                    try
                    {
                        var localFile = DownloadFile(file);
                        if (string.IsNullOrEmpty(localFile)) throw new Exception("下载失败");
                        _progressValue += file.Size;
                        var args = new UpdateProgressChangedEventArgs { CurrentProgress = _progressValue, TotalProgress = _totalProgress, Message = "下载文件中...", State = UpdateStateChangeStateEnum.InProgress };
                        OnUpdateProgressChangedEvent(this, args);
                        if (args.NeedCancel) return;
                        
                        localFiles.Add(localFile);
                        break;
                    }
                    catch
                    {
                        if (_stopSignal == true) return;
                        if (i == 2)
                        {
                            OnUpdateProgressChangedEvent(this, new UpdateProgressChangedEventArgs { CurrentProgress = 0, TotalProgress = 100, Message = "下载远程文件失败", State = UpdateStateChangeStateEnum.Fail });
                            return;
                        }
                        else
                        {
                            OnUpdateProgressChangedEvent(this, new UpdateProgressChangedEventArgs { CurrentProgress = 0, TotalProgress = 100, Message = "下载失败正在重试", State = UpdateStateChangeStateEnum.InProgress });
                        }
                    }                    
                }
            }

            OnUpdateProgressChangedEvent(this, new UpdateProgressChangedEventArgs { CurrentProgress = _progressValue, TotalProgress = _totalProgress, Message = "正在更新文件...", State = UpdateStateChangeStateEnum.InProgress });
            for (var i=0;i<localFiles.Count;i++)
            {
                var file = RemoteVersion.Files[i];
                if (file.Type == "Zip")
                {
                    ExtractFile(file, localFiles[i]);
                }
                else
                {
                    ReplaceFile(file, localFiles[i]);
                }
            }

            using (var writer = new StreamWriter("local.json",false))
            {
                writer.WriteLine(JsonConvert.SerializeObject(RemoteVersion));
            }
            LocalVersion = RemoteVersion;

            OnUpdateProgressChangedEvent(this, new UpdateProgressChangedEventArgs { CurrentProgress = _totalProgress, TotalProgress = _totalProgress, Message = "自动更新完成", State = UpdateStateChangeStateEnum.Done});
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="url"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        private bool DownloadFile(string savePath, string url, bool overwrite = true)
        {
            //打开上次下载的文件
            long SPosition = 0;
            var exist = File.Exists(savePath) && overwrite == false;
            //实例化流对象
            using (var FStream = exist ? File.OpenWrite(savePath) : new FileStream(savePath, FileMode.Create))
            {
                if(exist)
                {
                    SPosition = FStream.Length;
                    FStream.Seek(SPosition, SeekOrigin.Current);
                }

                try
                {
                    //打开网络连接
                    var myRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                    if (SPosition > 0)
                        myRequest.AddRange((int)SPosition);

                    long totalSize = SPosition;

                    var args = new UpdateProgressChangedEventArgs { CurrentProgress = _progressValue, TotalProgress = _totalProgress, Message = "下载文件中...", State = UpdateStateChangeStateEnum.InProgress };
                    
                    using (var myStream = myRequest.GetResponse().GetResponseStream())
                    {
                        //定义一个字节数据
                        var btContent = new byte[512];                        
                        var intSize = myStream.Read(btContent, 0, 512);
                        while (intSize > 0)
                        {
                            totalSize += intSize;
                            args.CurrentProgress = _progressValue + totalSize;
                            OnUpdateProgressChangedEvent(this, args);
                            if (args.NeedCancel)
                            {
                                _stopSignal = true;
                                return false;
                            }
                            
                           
                            FStream.Write(btContent, 0, intSize);
                            intSize = myStream.Read(btContent, 0, 512);
                        }
                    }
            
                    return true;                    
                }
                catch (Exception)
                {
                    return false;                    
                }
            }
        }

        public static string HashFile(string fileName)
        {

            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                var algorithm = System.Security.Cryptography.SHA1.Create();
                var bytes = algorithm.ComputeHash(fs);
                return BitConverter.ToString(bytes).Replace("-", "");
            }
        }

        

        private void ReplaceFile(ProductFile info, string localPath)
        {
            var fullName = Path.Combine(Application.StartupPath, info.Name);
            if (File.Exists(fullName)) File.Delete(fullName);
            File.Copy(localPath, localPath);
        }

        private void ExtractFile(ProductFile info, string localPath)
        {
            using (var zip = new ZipFile(localPath, Encoding.Default))
            {
                zip.ExtractAll(Application.StartupPath, ExtractExistingFileAction.OverwriteSilently);
            }
            /*
            using (var s = new ZipInputStream(File.OpenRead(localPath)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    var directoryName = Path.GetDirectoryName(theEntry.Name);
                    var fileName = Path.GetFileName(theEntry.Name);
                    if (directoryName.Length > 0) Directory.CreateDirectory(Path.Combine(Application.StartupPath, directoryName));                    
                    if (!directoryName.EndsWith("\\")) directoryName += "\\";
                    if (fileName != string.Empty)
                    {
                        var fullName = Path.Combine(Application.StartupPath, theEntry.Name);
                        if (File.Exists(fullName)) File.Delete(fullName);
                        using (var streamWriter = File.Create(fullName))
                        {
                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (size>0)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }                                
                            }
                        }
                    }
                }
            }*/
        }
           
        private string DownloadFile(ProductFile info)
        {
            var basePath = Path.GetTempFileName();
            if (DownloadFile(basePath, info.Address) == false) return string.Empty;
            var hash = HashFile(basePath);
            if (hash != info.Hash) return string.Empty;
            return basePath;
        }
        
    }
}
  