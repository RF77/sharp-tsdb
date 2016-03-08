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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DbInterfaces.Interfaces;
using FileDb.Properties;
using Infrastructure;
using Timeenator.Extensions;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace FileDb.Impl
{
    public class Db : ReadWritLockable, IDb
    {
        public Db(DbMetadata metadata) : base(TimeSpan.FromSeconds(30))
        {
            MetadataInternal = metadata;
            Directory.CreateDirectory(MeasurementDirectory);
        }

        public DbMetadata MetadataInternal { get; set; }
        public string Name => Metadata.Name;

        public void SaveMetadata()
        {
            MetadataInternal.SaveToFile(MetadataInternal.DbMetadataPath);
        }

        public string MeasurementDirectory
            => Path.Combine(MetadataInternal.DbPath, Settings.Default.MeasurementDirectory);

        public IDbMetadata Metadata => MetadataInternal;

        public void CreateMeasurement(IMeasurementMetadata metadata)
        {
            WriterLock(
                () =>
                {
                    MetadataInternal.SetMeasurement(metadata.Name, new Measurement((MeasurementMetadata) metadata, this));
                });
        }

        public IMeasurement CreateMeasurement(string name, Type valueType)
        {
            return WriterLock(() =>
            {
                var meas = new Measurement(name, valueType, this);
                MetadataInternal.SetMeasurement(name, meas);
                SaveMetadata();
                return meas;
            });
        }

        public IMeasurement GetMeasurement(string name)
        {
            return ReaderLock(() => MetadataInternal.GetMeasurement(name));
        }

        public IReadOnlyList<IMeasurement> GetMeasurements(string nameRegex)
        {
            List<IMeasurement> result = new List<IMeasurement>();
            var regex = new Regex(nameRegex);
            foreach (var m in MetadataInternal.Measurements.Values)
            {
                if (regex.IsMatch(m.Metadata.Name))
                {
                    result.Add(m);
                    continue;
                }
                if (m.Metadata.Aliases.Any(alias => regex.IsMatch(alias)))
                {
                    result.Add(m);
                }
            }

            return result;
        }

        public void AddAliasToMeasurements(string nameRegex, string aliasName)
        {
            WriterLock(() =>
            {
                var regex = new Regex(nameRegex);
                foreach (var m in MetadataInternal.Measurements.Values)
                {
                    if (regex.IsMatch(m.Metadata.Name))
                    {
                        var alias = regex.Replace(m.Metadata.Name, aliasName);
                        m.AddAlias(alias);
                        continue;
                    }
                    foreach (var alias in m.Metadata.Aliases)
                    {
                        if (regex.IsMatch(alias))
                        {
                            var newAlias = regex.Replace(alias, aliasName);
                            m.AddAlias(newAlias);
                            break;
                        }
                    }
                }
                SaveMetadata();
            });
        }

        public IQuerySerie<T> GetSerie<T>(string measurementName, string timeExpression) where T : struct
        {
            return
                ReaderLock(() => GetMeasurement(measurementName).GetDataPoints<T>(timeExpression).Alias(measurementName));
        }

        public IQuerySerie<T> GetSerie<T>(string measurementName) where T : struct
        {
            return GetSerie<T>(measurementName, null);
        }

        public IQueryTable<T> GetTable<T>(string measurementRegex, string timeExpression) where T : struct
        {
            var result = new QueryTable<T>();
            foreach (var meas in MetadataInternal.MeasurementsWithAliases)
            {
                var match = Regex.Match(meas.Key, measurementRegex);
                if (match.Success)
                {
                    var serie = meas.Value.GetDataPoints<T>(timeExpression).Alias(meas.Key);

                    Group g = match.Groups["g"];

                    if (g.Success)
                    {
                        serie.GroupName = g.Value;
                    }


                    Group k = match.Groups["k"];

                    if (k.Success)
                    {
                        serie.Key = k.Value;
                    }

                    result.AddSerie(serie);
                }
            }
            return result;
        }

        //public IObjectQueryTable GetTable(string measurementRegex, string timeExpression)
        //{
        //    throw new NotImplementedException();
        //}

        public IQueryResult Collect(params IQueryResult[] results)
        {
            return new SeriesCollection(results.SelectMany(i => i.Series));
        }

        public IReadOnlyList<string> GetMeasurementNames()
        {
            return ReaderLock(() => MetadataInternal.MeasurementsWithAliases.Keys.ToList());
        }

        public void DeleteMeasurement(string name)
        {
            WriterLock(() => MetadataInternal.Measurements.Remove(GetMeasurement(name).Name));
        }

        public void CopyMeasurement(string fromName, string toName)
        {
            WriterLock(() =>
            {
                var source = GetMeasurement(fromName);
                var target = GetOrCreateMeasurement(toName, source.ValueType);
                target.ClearDataPoints();
               // target.AppendDataPoints(source.GetDataPoints<>());
            });
        }

        public void DeleteAllMeasurements()
        {
            WriterLock(() => MetadataInternal.Measurements.Clear());
        }

        public IMeasurement GetOrCreateMeasurement(string name, string type = "float")
        {
            return WriterLock(() =>
            {
                if (MetadataInternal.MeasurementsWithAliases.ContainsKey(name))
                {
                    return GetMeasurement(name);
                }
                return CreateMeasurement(name, type.ToType());
            });
        }
        public IMeasurement GetOrCreateMeasurement(string name, Type type)
        {
            return WriterLock(() =>
            {
                if (MetadataInternal.Measurements.ContainsKey(name))
                {
                    return GetMeasurement(name);
                }
                return CreateMeasurement(name, type);
            });
        }

    }
}