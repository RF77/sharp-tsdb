using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public class Db : IDb
    {
        public Db(IDbMetadata metadata)
        {
            Metadata = metadata;
        }

        public IDbMetadata Metadata { get; set; }
        
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