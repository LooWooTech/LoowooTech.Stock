using LoowooTech.Stock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.WorkBench
{
    public enum ProgressResultTypeEnum
    {
        Pass = 0,Fail = 1,Other =2
    }

    public class ProgressEventArgs : EventArgs
    {
        /// <summary>
        /// 是否取消后续任务
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// 已完成任务的编码
        /// </summary>
        public string Code { get; set; }


        /// <summary>
        /// 已完成任务的详情（可以是空）
        /// </summary>
        public string Message { get; set; }

        public ProgressResultTypeEnum Result { get; set; }


    }

    public delegate void ProgramProgressHandler(object sender, ProgressEventArgs e);

    public interface IWorkBench
    {
        /// <summary>
        /// 每条质检规则执行完毕都会调用的事件
        /// </summary>
        event ProgramProgressHandler OnProgramProcess;

        /// <summary>
        /// 质检路径
        /// </summary>
        string Folder { get; set; }
        
        /// <summary>
        /// 质检报告路径
        /// </summary>
        string ReportPath { get; }
        /// <summary>
        /// 行政区名称
        /// </summary>
        string DistrictName { get; }
        /// <summary>
        /// 当前行政区代码
        /// </summary>
        string DistrictCode { get; }
        /// <summary>
        /// 质检规则的ID
        /// </summary>
        List<int> RulsIds { get; set; }

        /// <summary>
        /// 质检结果
        /// </summary>
        List<Question> Results { get; }

        /// <summary>
        /// 质检的主方法
        /// </summary>
        void Program();
        void Write(string folder);
    }
}
