using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace LoowooTech.Stock
{
    public static class UpdateUtil
    {
        public static ProductVersion GetMetadata(string serverAddress, string productId)
        {
            using (var stream = new MemoryStream())
            {
                var url = string.Format("{0}/{1}/version.json", serverAddress, productId);
                if (Download2Stream(stream, url))
                {
                    var arr = stream.ToArray();
                    var str = Encoding.UTF8.GetString(arr, 3, arr.Length - 3);
                    return DeserializeObject<ProductVersion>(str);
                }
            }
            return null;
        }

        public static ProductVersion GetLocalMetadata()
        {
            using (var reader = new StreamReader("local.json"))
            {
                return DeserializeObject<ProductVersion>(reader.ReadToEnd());
            }
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

        public static T DeserializeObject<T>(string content)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                return (T)serializer.ReadObject(ms);
            }
        }

    }

    public class ProductVersion
    {
        public string ChangeLog { get; set; }

        /// <summary>
        /// Version Name
        /// </summary>
        public string Name { get; set; }

        public int Build { get; set; }

        public string ProductId { get; set; }

        public List<ProductFile> Files { get; set; }
    }

    public class ProductFile
    {
        public string Address { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Hash { get; set; }

        public long Size { get; set; }
    }
}
