using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Models
{
    public class Question2
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string TableName { get; set; }


        public string BSM { get; set; }
        public string Description { get; set; }
        public CheckProject2 CheckProject { get; set; }
        public string Folder { get; set; }

        public string LocationClause { get; set; }
    }

    public enum CheckProject2
    {
        目录及文件规范性=1101,
        元数据=1102,
        图层完整性=1201,
        数学基础=1202,
        结构符合性=1301,
        值符合性=1302,
        属性正确性=1303,
        面积一致性=1304,
        拓扑关系=1401,
        碎片多边形=1402,
        基期与现状数据对比检查=2001,
        基数转换前后地类转换关系正确性=2002,
        A类转换过程合理性=2003,
        B类转换过程合理性=2004,
        C类转换过程合理性=2005,
        E类转换过程合理性=2006,
        F类转换过程合理性=2007,
        J类转换过程合理性=2008,
        规划用途数据与基期图斑数据地类逻辑一致性检查=3001,
        规划用途数据与土地规划地类数据地类逻辑一致性检查=3002,
        规划用途与建设用地管制区对比检查=3003,
        拟拆旧面积与村规划拟建新面积对比检查=4004,
        F1基数填报值与数据库汇总值一致=4101,
        F1规划目标填报值与数据库汇总值一致=4102,
        F1主要控制指标基本农田与乡规划下达值对比分析=4103,
        F1规划期末建设用地总规模不超过基期建设用地规模=4104,
        F2表格数据填报值与数据库汇总值一致=4201,
        F3表格数据填报值与数据库汇总值一致=4301,
        F4表格数据填报值与数据库汇总值一致=4401,
        规划用途与农转用数据套合=5001,
        村规划数据库与乡规划数据库永久基本农田空间范围是否一致=6001,
        村规划数据库与乡规划数据库示范区永久基本农田空间范围是否一致=6002,
        村规划数据库与乡规划数据库永久基本农田面积是否一致=6003,
        村规划数据库与乡规划数据库示范区永久基本农田面积是否一致=6004,
        村规划是否落实乡规划禁止建设区=6005,
        村规划020是否在乡规划020内=6006,
        村规划允许建设区不超过乡规划范围=6007
    }
}
