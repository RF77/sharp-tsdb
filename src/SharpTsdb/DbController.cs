using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using log4net;
using SharpTsdb.Properties;
using Timeenator.Impl;

namespace SharpTsdb
{
    [RoutePrefix("")]
    public class DbController : ApiController
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ControllerLogger MeLog = new ControllerLogger(Logger);

        [Route("dbs/createDb/{dbName}")]
        [HttpGet]
        public string CreateDb(string dbName)
        {
            using (MeLog.LogDebug($"name: {dbName}"))
            {
                DbService.DbManagement.CreateDb(Settings.Default.DbDirectory, dbName);
                return "ok";
            }
        }

        [Route("db/{dbName}/createMeasurment/{name}")]
        [HttpGet]
        public string CreateMeasurement(string dbName, string name)
        {
            using (MeLog.LogDebug($"db: {dbName}, meas: {name}"))
            {
                var myDb = DbService.DbManagement.GetDb(dbName);
                myDb.CreateMeasurement(name, typeof (float));
                return "ok";
            }
        }

        [Route("db/{dbName}/clearMeasurment/{name}")]
        [HttpGet]
        public string ClearMeasurement(string dbName, string name, DateTime? after)
        {
            using (MeLog.LogDebug($"db: {dbName}, meas: {name}, after: {after}"))
            {
                var myDb = DbService.DbManagement.GetDb(dbName);
                myDb.GetMeasurement(name).ClearDataPoints(after);
                return "ok";
            }
        }

        [Route("db/{dbName}/{meas}/appendRows")]
        [HttpPost]
        public string WritePoints(string dbName, string meas, [FromBody] List<WritePoint> points,
            bool truncateDbToFirstElement = false)
        {
            using (MeLog.LogDebug($"db: {dbName}, meas: {meas}, point#: {points?.Count}, trunc: {truncateDbToFirstElement}"))
            {
                var myDb = DbService.DbManagement.GetDb(dbName);
                var measurement = myDb.GetOrCreateMeasurement(meas);

                measurement.AppendDataPoints(points.Select(i => new DataRow {Key = i.t, Value = i.v}));
                return "ok";
            }
        }
    }
}