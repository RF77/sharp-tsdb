using System.Reflection;
using System.Web.Http;
using GrafanaAdapter.Queries;
using log4net;

namespace SharpTsdb
{
    [RoutePrefix("")]
    public class GrafanaController : ApiController
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ControllerLogger MeLog = new ControllerLogger(Logger);

        private readonly QueryHandler _handler = new QueryHandler();

        [Route("query")]
        [HttpGet()]
        public QueryRoot Get(string db,  string q)
        {
            using (MeLog.LogDebug($"db: {db}, q: {q}"))
            {
                return _handler.HandleQuery(db, q, DbService.DbManagement);
            }
        }
       



       

        
    }
}