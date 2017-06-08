using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Models
{
    public class Question
    {
        /// <summary>
        /// 规则编码
        /// </summary>
        public string Code { get; set; }
        
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 图层名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 标识码
        /// </summary>
        public string BSM { get; set; }
        /// <summary>
        /// 问题描述
        /// </summary>
        public string Description { get; set; }
        public string Remark { get; set; }
        public CheckProject Project { get; set; }
    }

    public enum CheckProject
    {
        目录及文件规范性=11,
        数据有效性=12,
        图层完整性=21,
        数学基础=22,
        结构符合性=31,
        值符合性=32,
        属性正确性=33,
        面积一致性=34,
        拓扑关系=41,
        碎片多边形=42,
        单要素空间不连续=43,
        图层内属性一致性=51,
        汇总表与数据库图层逻辑一致性=61,
        表格汇总面积和数据库汇总面积一致性=62
    }
}
