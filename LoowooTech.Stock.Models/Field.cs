using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Models
{
    public class Field
    {
        /// <summary>
        /// 字段代码
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// 字段类型
        /// </summary>
        public FieldType Type { get; set; }
        public static bool operator !=(Field a,Field b)
        {
            return !(a == b);
        }
        public static bool operator ==(Field a,Field b)
        {
            return a.Type == b.Type && a.Length == b.Length && a.Name == b.Name;
        }

        public override bool Equals(object o)
        {
            var f = o as Field;
            if (f == null) return false;
            return this == f;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }

    public enum FieldType
    {
        SmallInt=2,
        Int=3,
        Real=4,
        Float=5,
        Moeny=6,
        DateTime=7,
        Bit=11,
        TimeStamp=13,
        TinyInt=17,
        UniqueIdentifier=72,
        Binary=128,
        Char=129,
        NChar=130,
        Decimal=131,
        SmallDateTime=135,
        VarChar=200,
        Text=201,
        Image=205
    }
}
