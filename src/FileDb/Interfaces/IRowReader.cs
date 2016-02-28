using System.IO;
using Timeenator.Interfaces;

namespace FileDb.Interfaces
{
    public interface IRowReader
    {
        ISingleDataRow<T> ReadRow<T>(BinaryReader reader);
        IDataRow ReadRow(BinaryReader reader);
    }
}