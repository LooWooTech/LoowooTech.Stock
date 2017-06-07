using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Common
{
    public static class LogManager
    {
        private static ConcurrentBag<string> _list { get; set; }
        public static ConcurrentBag<string> List { get { return _list; } }

        public static void Init()
        {
            _list = new ConcurrentBag<string>();
        }
        public static void Log(string message)
        {
            System.Console.WriteLine(message);
        }
        public static void Record(string message)
        {
            _list.Add(message);
        }
        public static void LogRecord(string message)
        {
            Log(message);
            Record(message);
        }
    }
}
