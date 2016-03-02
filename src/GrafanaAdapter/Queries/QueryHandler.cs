using System.Collections.Generic;
using System.Linq;
using DbInterfaces.Interfaces;
using FileDb.Scripting;
using Timeenator.Extensions;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace GrafanaAdapter.Queries
{
    public class QueryHandler
    {
        public QueryRoot HandleQuery(string db, string query, IDbManagement dbm)
        {
            var root = new QueryRoot();
            var result = new QueryResult();
            var serie = new QuerySerie();

            if (query.ToLower() == "show measurements")
            {

                serie.name = "measurements";
                serie.columns.Add("name");


                var myDb = dbm.GetDb(db);
                serie.values.AddRange(myDb.GetMeasurementNames().Select(i => new List<object>() {i}));

                result.series.Add(serie);

                root.results.Add(result);
                return root;

            }
            var dbInstance = dbm.GetDb(db);
            var scriptingEngine = new ScriptingEngine(dbInstance, query);
            var res = scriptingEngine.Execute();

            if (res.ResultAsSerie != null)
            {
                CreateSerieResult(serie, res.ResultAsSerie, result, root);
            }
            if (res.ResultAsTable != null)
            {
                CreateTableResult(res.ResultAsTable, result, root);
            }
            return root;
        }

        private void CreateTableResult(IObjectQueryTable resultAsTable, QueryResult result, QueryRoot root)
        {
            foreach (var serie in resultAsTable.Series)
            {
                QuerySerie querySerie = new QuerySerie();
                CreateSingleResult(querySerie, serie, result);
            }

            root.results.Add(result);
        }

        private static void CreateSerieResult(QuerySerie serie, IObjectQuerySerie res, QueryResult result, QueryRoot root)
        {
            CreateSingleResult(serie, res, result);

            root.results.Add(result);
        }

        private static void CreateSingleResult(QuerySerie serie, IObjectQuerySerie res, QueryResult result)
        {
            serie.name = res.FullName;
            serie.columns.Add("time");
            serie.columns.Add("value");

            serie.values.AddRange(res.Rows.Select(i => new List<object> {i.TimeUtc.ToMiliSecondsAfter1970Utc(), i.Value}));
            if (res.LastRow != null)
            {
                serie.values.AddRange(res.Rows.Select(i => new List<object> { res.LastRow.TimeUtc.ToMiliSecondsAfter1970Utc(), res.LastRow.Value }));
            }

            result.series.Add(serie);
        }
    }
}
