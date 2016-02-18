using Timeenator.Impl.DataRows;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping
{
    public static class DataRowExtensions
    {
        public static NamedDataRow<T> Named<T>(this ISingleDataRow<T> rows, string name) where T : struct
        {
            return new NamedDataRow<T>(name, rows);
        }

        //public static IEnumerable<ISingleDataRow<T?>> Zip<T, TA, TB>(this IEnumerable<ISingleDataRow<TA?>> rowsA, IEnumerable<ISingleDataRow<TB?>> rowsB,
        //    Func<TA, TB, T> combineFunc) where T:struct where TA:struct where TB:struct
        //{
        //    var rowAEnumerator = rowsA.GetEnumerator();
        //    var rowBEnumerator = rowsB.GetEnumerator();

        //    var rowA = rowAEnumerator.MoveNext() ? rowAEnumerator.Current : null;
        //    var rowB = rowBEnumerator.MoveNext() ? rowBEnumerator.Current : null;

        //    if (rowA != null && rowB != null)
        //    {
        //        while (rowB != null && rowA.Key < rowB.Key)
        //        {
        //            rowB = rowBEnumerator.MoveNext() ? rowBEnumerator.Current : null;
        //        }
        //    }
        //}
    }
}
