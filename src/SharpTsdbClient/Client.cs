namespace SharpTsdbClient
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Client
    {
        private readonly string _serverAddress;
        private readonly ushort _port;

        /// <summary>
        /// Create a client to access the Sharp TSDB Server
        /// </summary>
        /// <param name="serverAddress">address of Server, e.g. localhost</param>
        /// <param name="port">Port of server (defaults to 9003)</param>
        public Client(string serverAddress, ushort port = 9003)
        {
            _serverAddress = serverAddress;
            _port = port;
        }
    }
}
