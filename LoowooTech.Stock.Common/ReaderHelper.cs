using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Common
{
    public static class ReaderHelper
    {
        public static double[] GetDoubleArray(this  OleDbDataReader reader,int count)
        {
            var result = new double[count];
            for(var i = 0; i < count; i++)
            {
                result[i] = reader.GetDouble(i);
            }
            return result;
        }

        public static int[] GetIntArray(this OleDbDataReader reader,int count)
        {
            var result = new int[count];
            for(var i = 0; i < count; i++)
            {
                result[i] = reader.GetInt32(i);
            }
            return result;
        }
    }
}
