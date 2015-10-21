namespace DbInterfaces.Interfaces
{
    interface IDbManagement
    {
        void CreateDb(string directoryPath, string name);
        IDb GetDb(string name);
        void DeleteDb(string name);
    }
}
