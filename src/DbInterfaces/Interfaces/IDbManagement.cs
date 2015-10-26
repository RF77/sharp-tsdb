using System.Collections;
using System.Collections.Generic;

namespace DbInterfaces.Interfaces
{
    public interface IDbManagement
    {
        IDb CreateDb(string directoryPath, string name);
        IDb GetDb(string name);
        IReadOnlyList<string> GetDbNames();
        void DeleteDb(string name);
        void AttachDb(string dbPath);
        void DetachDb(string dbName);
        void DetachAllDbs();


    }
}
