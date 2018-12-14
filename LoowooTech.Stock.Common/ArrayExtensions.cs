using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Common
{
    public static class ArrayExtensions
    {
        public static int[] TranlateInt(string[] items)
        {
            var results = new int[items.Length];
            for(var i = 0; i < items.Length; i++)
            {
                var item = items[i];
                var a = 0;
                if(int.TryParse(item,out a))
                {
                    results[i] = a;
                }
            }

            return results;
        }
    }
}
