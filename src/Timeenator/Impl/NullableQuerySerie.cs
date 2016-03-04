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
using Timeenator.Impl.Grouping;
using Timeenator.Interfaces;

namespace Timeenator.Impl
{
    [DebuggerDisplay("{FullName} ({Rows.Count})")]
    public class NullableQuerySerie<T> : QuerySerieBase<T>, INullableQuerySerie<T> where T : struct
    {
        public NullableQuerySerie(IReadOnlyList<ISingleDataRow<T?>> rows, DateTime? startTime, DateTime? endTime)
            : base(startTime, endTime)

        {
            Rows = rows;
        }

        public NullableQuerySerie(IReadOnlyList<ISingleDataRow<T?>> result, IQuerySerie<T> olddata) : base(olddata)
        {
            Rows = result;
        }

        public NullableQuerySerie(IReadOnlyList<ISingleDataRow<T?>> result, INullableQuerySerie<T> olddata)
            : base(olddata)
        {
            Rows = result;
        }

        public IReadOnlyList<ISingleDataRow<T?>> Rows { get; }

        public override object this[int index]
        {
            get { return Rows[index].Value; }
            set
            {
                if (value == null)
                {
                    Rows[index].Value = null;
                }
                else
                {
                    Rows[index].Value = (T) Convert.ChangeType(value, typeof (T));
                }
            }
        }

        public INullableQuerySerie<T> Clone(string serieName)
        {
            var serie = new NullableQuerySerie<T>(Rows, this) {Name = serieName};
            return serie;
        }

        IReadOnlyList<IObjectSingleDataRow> IObjectQuerySerie.Rows => Rows;

        #region methods

        public INullableQuerySerie<T> Zip(INullableQuerySerie<T> secondQuery, string resultQueryName,
            Func<T?, T?, T?> transformAction)
        {
            if (Rows.Count != secondQuery.Rows.Count)
                throw new ArgumentOutOfRangeException(nameof(secondQuery),
                    "Zip with different length of row not possible");
            var resultRows = new List<ISingleDataRow<T?>>(secondQuery.Rows.Count);

            var result = new NullableQuerySerie<T>(resultRows, this).Alias(resultQueryName);
            for (int i = 0; i < Rows.Count; i++)
            {
                if (Rows[i].TimeUtc != secondQuery.Rows[i].TimeUtc)
                    throw new ArgumentOutOfRangeException(nameof(secondQuery), "Zip with not aligned times");

                resultRows.Add(new SingleDataRow<T?>(Rows[i].TimeUtc,
                    transformAction(Rows[i].Value, secondQuery.Rows[i].Value)));
            }
            return result;
        }

        public INullableQuerySerie<T> Alias(string name)
        {
            SetAlias(name);
            return this;
        }

        public INullableQuerySerie<T> AppendName(string name)
        {
            Name += name;
            return this;
        }

        public INullableQuerySerie<T> Transform(Func<T?, T?> transformFunc)
        {
            var newRows = new List<ISingleDataRow<T?>>(Rows.Count);
            newRows.AddRange(Rows.Select(r => new SingleDataRow<T?>(r.TimeUtc, transformFunc(r.Value))));
            return new NullableQuerySerie<T>(newRows, this);
        }

        public INullableQuerySerie<T> CalcValue(Func<T?, T?> calculationFunc, string newSerieName = null)
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

        public INullableQuerySerie<T> FillValue(T fillValue)
        {
            foreach (var row in Rows)
            {
                if (row.Value == null)
                {
                    row.Value = fillValue;
                }
            }
            return this;
        }

        public IQuerySerie<T> RemoveNulls()
        {
            return
                new QuerySerie<T>(
                    Rows.Where(i => i.Value != null)
                        .Select(i => new SingleDataRow<T>(i.TimeUtc, i.Value.Value))
                        .ToList(), this);
        }

        public INullableQuerySerie<T> Fill(ValueForNull fillValue)
        {
            switch (fillValue)
            {
                case ValueForNull.Previous:
                {
                    T? previous = PreviousRow?.Value;
                    foreach (var row in Rows)
                    {
                        if (row.Value == null)
                        {
                            row.Value = previous;
                        }
                        else
                        {
                            previous = row.Value;
                        }
                    }
                }
                    break;
                case ValueForNull.Next:
                {
                    T? next = NextRow?.Value;
                    var rows = Rows;
                    for (int i = rows.Count - 1; i >= 0; i--)
                    {
                        var item = rows[i];
                        if (item.Value == null)
                        {
                            item.Value = next;
                        }
                        else
                        {
                            next = item.Value;
                        }
                    }
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fillValue), fillValue, null);
            }
            return this;
        }

        #endregion
    }
}