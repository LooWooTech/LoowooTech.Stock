using LoowooTech.Stock.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Common
{
    public static class QuestionManager
    {
        /// <summary>
        /// 质检问题列表
        /// </summary>
        private static List<Question> _questions { get; set; }
        public static List<Question> Questions { get { return _questions; } }
        private static  ConcurrentBag<Question> _paralleQuestions { get; set; }
        public static ConcurrentBag<Question> ParalleQuestions { get { return _paralleQuestions; } }
        public static void Init()
        {
            if (_questions == null)
            {
                _questions = new List<Question>();
            }
            _questions.Clear();
        }
        public static void AddRange(List<Question> questions)
        {
            if (_questions == null)
            {
                _questions = new List<Question>();
            }
            _questions.AddRange(questions);
        }
        public static void Add(Question question)
        {
            if (_questions == null)
            {
                _questions = new List<Question>();   
            }
            _questions.Add(question);
        } 

        public static void Save()
        {

        }
        

    }
}
