using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoowooTech.Stock.Models;

namespace LoowooTech.Stock.Common
{
    public static class QuestionManager2
    {
        private static int MAXNUMBER = 65534;

        private static readonly object _syncRoot = new object();

        public static List<Question2> Questions { get; private set; } = new List<Question2>();

        public static void Clear()
        {
            lock (_syncRoot)
            {
                Questions.Clear();
            }
        }

        public static void AddRange(List<Question2> questions)
        {
            lock (_syncRoot)
            {
                Questions.AddRange(questions);
            }
        }


        public static void Add(Question2 question)
        {
            lock (_syncRoot)
            {
                Questions.Add(question);
            }
        }
    }
}
