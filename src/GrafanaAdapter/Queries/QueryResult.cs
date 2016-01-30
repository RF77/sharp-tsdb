using System.Collections.Generic;

namespace GrafanaAdapter.Queries
{
    public class QueryResult
    {
        public List<QuerySerie> series { get; set; } = new List<QuerySerie>();
    }
}