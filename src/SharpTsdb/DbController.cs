using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using DbInterfaces.Interfaces;
using log4net;
using Serialize.Linq.Serializers;
using SharpTsdb.Properties;
using SharpTsdbTypes.Communication;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace SharpTsdb
{
    [RoutePrefix("")]
    public class DbController : ApiController
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ControllerLogger MeLog = new ControllerLogger(Logger);
        private static readonly ExpressionSerializer LinqSerializer = new ExpressionSerializer(new BinarySerializer());

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
        public string CreateMeasurement(string dbName, string name, string type = "float")
        {
            using (MeLog.LogDebug($"db: {dbName}, meas: {name}"))
            {
                var myDb = DbService.DbManagement.GetDb(dbName);
                myDb.CreateMeasurement(name, Type.GetType(type));
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
        public string WritePoints(string dbName, string meas, [FromBody] DataRows data,
            bool truncateDbToFirstElement = false)
        {
            using (MeLog.LogDebug($"db: {dbName}, meas: {meas}, point#: {data.Rows.Count}, trunc: {truncateDbToFirstElement}"))
            {
                var myDb = DbService.DbManagement.GetDb(dbName);
                var measurement = myDb.GetOrCreateMeasurement(meas);

                measurement.AppendDataPoints(data.AsIDataRows());
                return "ok";
            }
        }

        [Route("db/{dbName}/binQuerySerie")]
        [HttpPost]
        public async Task<DataSerie> BinaryQuerySerie(string dbName)
        {
            using (MeLog.LogDebug($"db: {dbName}"))
            {
                var myDb = DbService.DbManagement.GetDb(dbName);

                byte[] query = await Request.Content.ReadAsByteArrayAsync();

                var queryExpression = ((Expression<Func<IDb, IObjectQuerySerie>>)LinqSerializer.DeserializeBinary(query)).Compile();

                IObjectQuerySerie result = queryExpression(myDb);
                return new DataSerie(result);
            }
        }
    }
}