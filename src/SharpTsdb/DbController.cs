using System.Web.Http;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;
using SharpTsdb.Properties;

namespace SharpTsdb
{
    [RoutePrefix("")]
    public class DbController : ApiController
    {
        [Route("createDb")]
        [HttpGet]
        public string CreateDb(string dbName)
        {
            DbService.DbManagement.CreateDb(Settings.Default.DbDirectory, dbName);
            return "ok";
        }
    }
}