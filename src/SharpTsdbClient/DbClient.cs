namespace SharpTsdbClient
{
    public class DbClient
    {
        public Client Client { get; set; }
        public string DbName { get; }

        public DbClient(Client client, string dbName)
        {
            Client = client;
            DbName = dbName;
        }

        public MeasurementClient Measurement(string name)
        {
            return new MeasurementClient(this, name);
        }
    }
}
