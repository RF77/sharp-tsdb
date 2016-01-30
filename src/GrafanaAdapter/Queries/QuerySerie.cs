using System.Collections.Generic;

namespace GrafanaAdapter.Queries
{
    public class QuerySerie
    {
        public string name { get; set; }
        public List<string> columns { get; set; } = new List<string>();
        public List<List<object>> values { get; set; } = new List<List<object>>();
    }
}