using System.Collections;
using System.Collections.Generic;

namespace DbInterfaces.Interfaces
{
    public interface IDbManagement
    {
        void CreateDb(string directoryPath, string name);
        IDb GetDb(string name);
        IReadOnlyList<IDb> ListDbs();
        void DeleteDb(string name);
    }
}
