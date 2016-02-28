using Timeenator.Impl.DataRows;
using Timeenator.Interfaces;

namespace Timeenator.Extensions.Grouping
{
    public static class DataRowExtensions
    {
        public static NamedDataRow<T> Named<T>(this ISingleDataRow<T> rows, string name) where T : struct
        {
            return new NamedDataRow<T>(name, rows);
        }

    }
}
