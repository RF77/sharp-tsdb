using System.Collections.Generic;
using System.Linq;
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

        [Route("clearMeas")]
        [HttpGet]
        public string ClearMeas(string db, string name)
        {
            IDbManagement dbm = new DbManagement();

            var myDb = dbm.GetDb(db);
            myDb.GetMeasurement(name).ClearDataPoints();
            return "ok";
        }

        [Route("write")]
        [HttpPost]
        public string WritePoints(string db, string meas, [FromBody]List<WritePoint> points)
        {
            IDbManagement dbm = new DbManagement();

            var myDb = dbm.GetDb(db);
            var measurement = myDb.GetMeasurement(meas);
            measurement.AppendDataPoints(points.Select(i => new DataRow() {Key = i.t, Value = i.v}));
            return "ok";
        }
    }
}