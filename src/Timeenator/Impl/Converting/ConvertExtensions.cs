using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryLanguage.Converting
{
    public static class ConvertExtensions
    {
        public static float ToFloat(this object val)
        {
            return Convert.ToSingle(val);
        }

        public static double ToDouble(this object val)
        {
            return Convert.ToDouble(val);
        }

        public static T ToType<T>(this object val) where T:struct 
        {
            return (T)Convert.ChangeType(val, typeof(T));
        }
       
    }
}
