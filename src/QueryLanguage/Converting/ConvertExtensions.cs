using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryLanguage.Converting
{
    public static class ConvertExtensions
    {
        public static float ToFloat<T>(this T val) where T : struct
        {
            return Convert.ToSingle(val);
        }

        public static float ToFloat<T>(this T? val) where T : struct
        {
            return Convert.ToSingle(val.Value);
        }

        public static double ToDouble<T>(this T val) where T : struct
        {
            return Convert.ToDouble(val);
        }

        public static double ToDouble<T>(this T? val) where T : struct
        {
            return Convert.ToDouble(val.Value);
        }

        public static T ToType<T>(this float val) where T : struct
        {
            return (T)Convert.ChangeType(val, typeof(T));
        }
    }
}
