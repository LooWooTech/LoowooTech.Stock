using Ionic.Zip;
using LoowooTech.Updater.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace LoowooTech.Updater
{
    static class Program
    {
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new UpdateForm());
            }
            else
            {
                if(Directory.Exists(args[0])== false)
                {
                    Console.WriteLine("找不到指定的目录");
                    return;
                }
                var file = string.Format("{0}.zip", Guid.NewGuid());

                using (var zip = new ZipFile())
                {
                    ZipDirectory(new DirectoryInfo(args[0]), zip, (new DirectoryInfo(args[0])).FullName);
                    zip.Save(file);
                }
                
                ProductVersion version;
                try
                {
                    version = UpdateManager.GetMetadata(ConfigurationManager.AppSettings["Server"], ConfigurationManager.AppSettings["ProductId"]);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("下载远程版本号失败:" + ex);
                    return;
                }

                /*using (var reader = new StreamReader("version.json"))
                {
                    var serializer = new DataContractJsonSerializer(typeof(ProductVersion));
                    version = DeserializeObject<ProductVersion>(reader.ReadToEnd());
                }*/

                version.Build++;
                version.Files.Clear();

                var fileInfo = new ProductFile
                {
                    Name = file,
                    Address = string.Format("{0}/{1}/{2}", ConfigurationManager.AppSettings["Server"], ConfigurationManager.AppSettings["ProductId"], file),
                    Type = "Zip",
                    Hash = UpdateManager.HashFile(file),
                    Size = (new FileInfo(file)).Length
                };

                version.Files.Add(fileInfo);
                version.ProductId = ConfigurationManager.AppSettings["ProductId"];
                if (args.Length > 1)
                {
                    version.Name = args[1];
                }

                if(args.Length>2)
                {
                    version.ChangeLog = string.Format("版本{0}(Build {1}) {2:yyyy-MM-dd}<br>{3}<br>------------------------------------------<br>{2}", version.Name, version.Build, DateTime.Now, args[2], version.ChangeLog);
                }
                
                using (var writer = new StreamWriter("version.json", false))
                {
                    writer.WriteLine(JsonConvert.SerializeObject(version));
                }
                Console.WriteLine(string.Format("成功生成发布包'{0}'和配置文件version.json", file));
            }
        }

       
        static void ZipDirectory(DirectoryInfo dir, ZipFile zip, string baseFolder)
        {
            var files = dir.GetFiles();
            foreach(var file in files)
            {
                zip.AddFile(file.FullName, file.DirectoryName.Replace(baseFolder, string.Empty));
            }

            var dirs = dir.GetDirectories();
            foreach(var d in dirs)
            {                
                ZipDirectory(d, zip, baseFolder);
            }
        }

      
    }
}
