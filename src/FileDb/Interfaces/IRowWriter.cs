using System.IO;
using Timeenator.Interfaces;

namespace FileDb.Interfaces
{
    public interface IRowWriter
    {
        void WriteRow(BinaryWriter writer, IDataRow row);
    }
}