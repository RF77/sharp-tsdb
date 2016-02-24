using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;
using GrafanaAdapter.Queries;
using Timeenator.Impl;

namespace SharpTsdb
{
    [RoutePrefix("")]
    public class GrafanaController : ApiController
    {
        private QueryHandler _handler = new QueryHandler();

        [Route("query")]
        [HttpGet()]
        public QueryRoot Get(string db,  string q)
        {
            return _handler.HandleQuery(db, q, DbService.DbManagement);
        }


        

        [Route("createMeas")]
        [HttpGet]
        public string CreateMeas(string db, string name)
        {
            var myDb = DbService.DbManagement.GetDb(db);
            myDb.CreateMeasurement(name, typeof (float));
            return "ok";
        }

        [Route("clearMeas")]
        [HttpGet]
        public string ClearMeas(string db, string name, DateTime? after)
        {
            var myDb = DbService.DbManagement.GetDb(db);
            myDb.GetMeasurement(name).ClearDataPoints(after);
            return "ok";
        }

        [Route("write")]
        [HttpPost]
        public string WritePoints(string db, string meas, [FromBody]List<WritePoint> points)
        {
            var myDb = DbService.DbManagement.GetDb(db);
            var measurement = myDb.GetOrCreateMeasurement(meas);
            measurement.AppendDataPoints(points.Select(i => new DataRow() {Key = i.t, Value = i.v}));
            return "ok";
        }
    }
}