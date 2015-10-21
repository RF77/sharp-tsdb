namespace DbInterfaces.Interfaces
{
    public interface IDbManagement
    {
        void CreateDb(string directoryPath, string name);
        IDb GetDb(string name);
        void DeleteDb(string name);
    }
}
