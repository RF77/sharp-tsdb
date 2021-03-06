﻿// /*******************************************************************************
//  * Copyright (c) 2016 by RF77 (https://github.com/RF77)
//  * All rights reserved. This program and the accompanying materials
//  * are made available under the terms of the Eclipse Public License v1.0
//  * which accompanies this distribution, and is available at
//  * http://www.eclipse.org/legal/epl-v10.html
//  *
//  * Contributors:
//  *    RF77 - initial API and implementation and/or initial documentation
//  *******************************************************************************/ 

using System;
using System.Collections.Generic;
using System.Linq;
using DbInterfaces.Interfaces;
using FileDb.Scripting;
using Timeenator.Extensions;
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

            if (query.ToLower().StartsWith("show measurements"))
            {
                serie.name = "measurements";
                serie.columns.Add("name");


                var myDb = dbm.GetDb(db);
                serie.values.AddRange(myDb.GetMeasurementNames().Select(i => new List<object> {i}));

                result.series.Add(serie);

                root.results.Add(result);
                return root;
            }
            var dbInstance = dbm.GetDb(db);

            var queries = query.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);

            foreach (var subQuery in queries)
            {
                var scriptingEngine = new ScriptingEngine(dbInstance, subQuery);
                var res = scriptingEngine.Execute();

                foreach (var s in res.Result.Series.OrderByDescending(i => i.FullName))
                {
                    QuerySerie querySerie = new QuerySerie();
                    CreateSingleResult(querySerie, s, result);
                }
            }

            root.results.Add(result);
            return root;
        }

        private void CreateTableResult(IObjectQueryTable resultAsTable, QueryResult result, QueryRoot root)
        {
            foreach (var serie in resultAsTable.Series.OrderByDescending(i => i.FullName))
            {
                QuerySerie querySerie = new QuerySerie();
                CreateSingleResult(querySerie, serie, result);
            }

            root.results.Add(result);
        }

        private static void CreateSerieResult(QuerySerie serie, IObjectQuerySerie res, QueryResult result,
            QueryRoot root)
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
                serie.values.AddRange(
                    res.Rows.Select(
                        i => new List<object> {res.LastRow.TimeUtc.ToMiliSecondsAfter1970Utc(), res.LastRow.Value}));
            }

            result.series.Add(serie);
        }
    }
}