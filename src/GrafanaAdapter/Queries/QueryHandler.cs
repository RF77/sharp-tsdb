using System.Collections.Generic;
using System.Linq;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;

namespace GrafanaAdapter.Queries
{
    public class QueryHandler
    {
        public QueryRoot HandleQuery(string db, string query)
        {
            if (query.ToLower() == "show measurements")
            {
                 var root = new QueryRoot();
                var result = new QueryResult();
                var serie = new QuerySerie();
                serie.name = "measurements";
                serie.columns.Add("name");

                IDbManagement dbm = new DbManagement();

                var myDb = dbm.GetDb(db);
                serie.values.AddRange(myDb.GetMeasurementNames().Select(i => new List<object>() { i }));

                result.series.Add(serie);

                root.results.Add(result);
                return root;
               
            }

            return null;
        }
    }
}
