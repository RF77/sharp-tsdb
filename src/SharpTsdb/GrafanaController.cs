using System.Collections.Generic;
using System.Web.Http;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;
using GrafanaAdapter.Queries;
using SharpTsdb.Properties;

namespace SharpTsdb
{
    [RoutePrefix("")]
    public class GrafanaController : ApiController
    {
        private QueryHandler _handler = new QueryHandler();

        [Route("query")]
        public QueryRoot Get(string db, string q)
        {
            return _handler.HandleQuery(db, q);
        }


        [Route("createDb")]
        [HttpGet]
        public string CreateDb(string dbName)
        {
            IDbManagement db = new DbManagement();

            db.CreateDb(Settings.Default.DbDirectory, dbName);
            return "ok";
        }

        [Route("createMeas")]
        [HttpGet]
        public string CreateMeas(string db, string name)
        {
            IDbManagement dbm = new DbManagement();

            var myDb = dbm.GetDb(db);
            myDb.CreateMeasurement(name, typeof (float));
            return "ok";
        }
    }
}