namespace DbInterfaces.Interfaces
{
    public interface IDb
    {
        void CreateMeasurement(IMeasurementConfig config);
        IMeasurement GetMeasurement(string name);
    }
}