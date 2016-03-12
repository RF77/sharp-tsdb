// /*******************************************************************************
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
using Timeenator.Impl.Grouping.Configurators;
using Timeenator.Interfaces;

namespace Timeenator.Impl
{
    public class QueryTable<T> : QueryTableBase<T>, IQueryTable<T> where T : struct
    {
        public override IObjectQuerySerieBase GetOrCreateSerie(string name)
        {
            var serie = TryGetSerie(name);
            if (serie == null)
            {
                var firstSerie = Series.Values.FirstOrDefault();
                if (firstSerie != null)
                {
                    var newSerie = new QuerySerie<T>(new List<ISingleDataRow<T>>(firstSerie.Rows.Select(i => new SingleDataRow<T>(i.Key, default(T)))), firstSerie.StartTime, firstSerie.EndTime);
                    AddSerie(newSerie);
                    serie = newSerie;
                }
            }
            return serie;
        }

        public new IDictionary<string, IQuerySerie<T>> Series { get; } = new Dictionary<string, IQuerySerie<T>>();

        public void AddSerie(IQuerySerie<T> serie)
        {
            Series[serie.Name] = serie;
        }

        IQuerySerie<T> IQueryTable<T>.TryGetSerie(string name)
        {
            IQuerySerie<T> serie;
            if (Series.TryGetValue(name, out serie))
            {
                return serie;
            }
            return null;
        }

        IEnumerable<IObjectQuerySerie> IObjectQueryTable.Series => Series.Values;
        IEnumerable<IQuerySerie<T>> IQueryTable<T>.Series => Series.Values;

        public override IObjectQuerySerieBase TryGetSerie(string name)
        {
            return ((IQueryTable<T>) this).TryGetSerie(name);
        }

        public INullableQueryTable<T> Transform(Func<IQuerySerie<T>, INullableQuerySerie<T>> doFunc)
        {
            var table = new NullableQueryTable<T>();
            foreach (var serie in Series.Values)
            {
                table.AddSerie(doFunc(serie));
            }

            return table;
        }

        public INullableQueryTable<T> Group(Func<IGroupSelector<T>, IExecutableGroup<T>> groupConfigurator)
        {
            var table = new NullableQueryTable<T>();
            foreach (var serie in Series.Values)
            {
                table.AddSerie(serie.Group(groupConfigurator));
            }

            return table;
        }

        public IQueryTable<T> RemoveDbSeries()
        {
            var querySeries = Series.Values.Where(i => i.IsDbSerie).ToArray();
            foreach (var serie in querySeries)
            {
                Series.Remove(serie.Name);
            }
            return this;
        }

        protected override IEnumerable<IObjectQuerySerie> GetSeries() => Series.Values;
    }
}