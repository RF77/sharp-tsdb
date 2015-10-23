namespace DbInterfaces.Interfaces
{
    public interface IDb
    {
        IDbMetadata Metadata { get; }
        void CreateMeasurement(IMeasurementConfig config);
        IMeasurement GetMeasurement(string name);
    }
}