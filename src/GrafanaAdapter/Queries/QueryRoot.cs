using System.Collections.Generic;

namespace GrafanaAdapter.Queries
{
    public class QueryRoot
    {
        public List<QueryResult> results { get; set; } = new List<QueryResult>();
    }
}