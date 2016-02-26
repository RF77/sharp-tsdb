using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using SharpTsdb.Properties;
using Timeenator.Impl;

namespace SharpTsdb
{
    [RoutePrefix("")]
    public class DbController : ApiController
    {
        [Route("dbs/createDb/{dbName}")]
        [HttpGet]
        public string CreateDb(string dbName)
        {
            DbService.DbManagement.CreateDb(Settings.Default.DbDirectory, dbName);
            return "ok";
        }

        [Route("db/{dbName}/createMeasurment/{name}")]
        [HttpGet]
        public string CreateMeasurement(string dbName, string name)
        {
            var myDb = DbService.DbManagement.GetDb(dbName);
            myDb.CreateMeasurement(name, typeof(float));
            return "ok";
        }

        [Route("db/{dbName}/clearMeasurment/{name}")]
        [HttpGet]
        public string ClearMeasurement(string dbName, string name, DateTime? after)
        {
            var myDb = DbService.DbManagement.GetDb(dbName);
            myDb.GetMeasurement(name).ClearDataPoints(after);
            return "ok";
        }

        [Route("db/{dbName}/{meas}/appendRows")]
        [HttpPost]
        public string WritePoints(string dbName, string meas, [FromBody]List<WritePoint> points, bool truncateDbToFirstElement = false)
        {
            var myDb = DbService.DbManagement.GetDb(dbName);
            var measurement = myDb.GetOrCreateMeasurement(meas);
            
            measurement.AppendDataPoints(points.Select(i => new DataRow() { Key = i.t, Value = i.v }));
            return "ok";
        }
    }
}