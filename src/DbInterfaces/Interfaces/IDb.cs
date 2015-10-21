namespace DbInterfaces.Interfaces
{
    interface IDb
    {
        void CreateMeasurement(IMeasurementConfig config);
        IMeasurement GetMeasurement(string name);
    }
}