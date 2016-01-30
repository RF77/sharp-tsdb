using System.Collections.Generic;
using System.Web.Http;
using GrafanaAdapter.Queries;

namespace SharpTsdb
{
    [RoutePrefix("")]
    public class GrafanaController : ApiController
    {
        [Route("query")]
        public QueryRoot Get(InfluxQueryParams p)
        {
            var root = new QueryRoot();
            var result = new QueryResult();
            var serie = new QuerySerie();
            serie.name = "measurements";
            serie.columns.Add("name");
            serie.values.Add(new List<object>() { "Test1" });
            serie.values.Add(new List<object>() { "Test2" });

            result.series.Add(serie);

            root.results.Add(result);


            return root;
        }
    }
}