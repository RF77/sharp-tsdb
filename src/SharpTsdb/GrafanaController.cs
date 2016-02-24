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
       



       

        
    }
}