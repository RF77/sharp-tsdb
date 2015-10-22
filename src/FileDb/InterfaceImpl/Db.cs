using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public class Db : IDb
    {
        public void CreateMeasurement(IMeasurementConfig config)
        {
            throw new System.NotImplementedException();
        }

        public IMeasurement GetMeasurement(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}