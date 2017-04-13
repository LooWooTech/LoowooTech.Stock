using System;
using System.Collections.Generic;
using System.IO;

namespace LoowooTech.Stock.Common
{
    public static class MessageManager
    {
        public static void Write(this string fileName, List<string> messages,string folder)
        {
            if (!System.IO.Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            using (var fs=new FileStream(System.IO.Path.Combine(folder, fileName + ".txt"), FileMode.OpenOrCreate))
            {
                using (var sw=new StreamWriter(fs))
                {
                    foreach(var item in messages)
                    {
                        sw.WriteLine(item);
                    }
                    sw.Flush();
                    sw.Close();
                }
                fs.Close();
            }
        }

        public static void Write(this string fileName, string message,string folder)
        {
            if (!System.IO.Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            using (var fs=new FileStream(System.IO.Path.Combine(folder, fileName + ".txt"), FileMode.OpenOrCreate))
            {
                using (var sw=new StreamWriter(fs))
                {
                    sw.WriteLine(message);
                    sw.Flush();
                    sw.Close();
                }

                fs.Close();
            }
        }
    }
}
