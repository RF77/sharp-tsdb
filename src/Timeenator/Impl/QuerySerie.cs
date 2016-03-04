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
using System.Diagnostics;
using System.Linq;
using Timeenator.Extensions.Converting;
using Timeenator.Impl.Grouping;
using Timeenator.Impl.Grouping.Configurators;
using Timeenator.Interfaces;

namespace Timeenator.Impl
{
    [DebuggerDisplay("{FullName} ({Rows.Count})")]
    public partial class QuerySerie<T> : QuerySerieBase<T>, IQuerySerie<T> where T : struct
    {
        public QuerySerie(IReadOnlyList<ISingleDataRow<T>> rows, DateTime? startTime, DateTime? endTime)
            : base(startTime, endTime)

        {
            Rows = rows;
        }

        public QuerySerie(IReadOnlyList<ISingleDataRow<T>> rows, IQuerySerieBase<T> oldSerie) : base(oldSerie)
        {
            Rows = rows;
        }

        public IReadOnlyList<ISingleDataRow<T>> Rows { get; }

        public override object this[int index]
        {
            get { return Rows[index].Value; }
            set { Rows[index].Value = (T) Convert.ChangeType(value, typeof (T)); }
        }

        public IEnumerable<T> Values => Rows.Select(i => i.Value);
        IReadOnlyList<IObjectSingleDataRow> IObjectQuerySerie.Rows => Rows;

        public IQuerySerie<T> IncludeLastRow()
        {
            if (Rows.Any())
            {
                LastRow = Rows.Last();
            }
            return this;
        }

        public IQuerySerie<T> Where(Func<ISingleDataRow<T>, bool> predicate)
        {
            if (Rows.Any())
            {
                return new QuerySerie<T>(Rows.Where(predicate).ToList(), this);
            }
            return this;
        }

        public IQuerySerie<T> WhereValue(Func<T, bool> predicate)
        {
            if (Rows.Any())
            {
                return new QuerySerie<T>(Rows.Where(i => predicate(i.Value)).ToList(), this);
            }
            return this;
        }

        public IQuerySerie<T> Alias(string name)
        {
            SetAlias(name);
            return this;
        }

        public IQuerySerie<T> AppendName(string name)
        {
            Name += name;
            return this;
        }

        public INullableQuerySerie<T> Transform(Func<T, T?> transformFunc)
        {
            var newRows = new List<ISingleDataRow<T?>>(Rows.Count);
            newRows.AddRange(Rows.Select(r => new SingleDataRow<T?>(r.TimeUtc, transformFunc(r.Value))));
            return new NullableQuerySerie<T>(newRows, this);
        }

        /// <summary>
        ///     Normalize a saw tooth like series due to overflows or resets
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resetValue">let it null to take the first value after the overflow, otherwise set the value explicitly</param>
        /// <returns></returns>
        public IQuerySerie<T> NormalizeOverflows(double? resetValue = null)
        {
            if (Rows.Any())
            {
                var newRows = new List<ISingleDataRow<T>>(Rows.Count);
                double offset = 0;
                double previousValue = Rows.First().Value.ToDouble();
                foreach (var row in Rows)
                {
                    double rowValue = row.Value.ToDouble();
                    if (previousValue > rowValue)
                    {
                        if (resetValue != null)
                        {
                            offset += previousValue - (rowValue - resetValue.Value);
                        }
                        else
                        {
                            offset += previousValue;
                        }
                    }
                    newRows.Add(new SingleDataRow<T>(row.TimeUtc, (rowValue + offset).ToType<T>()));
                    previousValue = rowValue;
                }
                return new QuerySerie<T>(newRows, this);
            }
            return this;
        }

        public INullableQuerySerie<T> ToNullable()
        {
            return new NullableQuerySerie<T>(Rows.Select(i => new SingleDataRow<T?>(i.TimeUtc, i.Value)).ToList(), this);
        }

        public IQuerySerie<T> CalcValue(Func<T, T> calculationFunc, string newSerieName = null)
        {
            var rows = new List<ISingleDataRow<T>>(Rows.Count);
            if (Rows.Any())
            {
                rows.AddRange(Rows.Select(row => new SingleDataRow<T>(row.TimeUtc, calculationFunc(row.Value))));
            }
            var newSerie = new QuerySerie<T>(rows, this);
            if (newSerieName != null)
            {
                newSerie.Name = newSerieName;
            }
            return newSerie;
        }

        public INullableQuerySerie<T> CalcNullableValue(Func<T, T?> calculationFunc, string newSerieName = null)
        {
            var rows = new List<ISingleDataRow<T?>>(Rows.Count);
            if (Rows.Any())
            {
                rows.AddRange(Rows.Select(row => new SingleDataRow<T?>(row.TimeUtc, calculationFunc(row.Value))));
            }
            var newSerie = new NullableQuerySerie<T>(rows, this);
            if (newSerieName != null)
            {
                newSerie.Name = newSerieName;
            }
            return newSerie;
        }

        public INullableQuerySerie<T> Group(Func<IGroupSelector<T>, IExecutableGroup<T>> groupConfigurator)
        {
            return groupConfigurator(new GroupSelector<T>(this)).ExecuteGrouping();
        }

        public IReadOnlyList<StartEndTime> TimeRanges(
            Func<IGroupSelector<T>, IGroupByStartEndTimesConfiguratorOptional<T>> groupConfigurator)
        {
            var groupTimesCreator = groupConfigurator(new GroupSelector<T>(this)) as IGroupTimesCreator;

            return groupTimesCreator?.CreateGroupTimes() ?? new List<StartEndTime>();
        }
    }
}