using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using DbInterfaces.Interfaces;
using FileDb.Properties;
using Infrastructure;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace FileDb.InterfaceImpl
{
    public class Db : ReadWritLockable, IDb
    {
        public string Name => Metadata.Name;

        public void SaveMetadata()
        {
            MetadataInternal.SaveToFile(MetadataInternal.DbMetadataPath);
        }

        public string MeasurementDirectory => Path.Combine(MetadataInternal.DbPath, Settings.Default.MeasurementDirectory);

        public Db(DbMetadata metadata):base(TimeSpan.FromSeconds(30))
        {
            MetadataInternal = metadata;
            Directory.CreateDirectory(MeasurementDirectory);
        }

        public DbMetadata MetadataInternal { get; set; }

        public IDbMetadata Metadata => MetadataInternal;

        public void CreateMeasurement(IMeasurementMetadata metadata)
        {
            WriterLock(() =>
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

        public IQuerySerie<T> GetSerie<T>(string measurementName, string timeExpression) where T : struct
        {
            return ReaderLock(() => GetMeasurement(measurementName).GetDataPoints<T>(timeExpression));
        }

        public IQueryTable<T> GetTable<T>(string measurementRegex, string timeExpression) where T : struct
        {
            var result = new QueryTable<T>();
            foreach (var meas in MetadataInternal.Measurements)
            {
                var match = Regex.Match(meas.Key, measurementRegex);
                if (match.Success)
                {
                    var serie = meas.Value.GetDataPoints<T>(timeExpression);

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

        public IObjectQueryTable GetTable(string measurementRegex, string timeExpression)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<string> GetMeasurementNames()
        {
            return ReaderLock(() => MetadataInternal.Measurements.Keys.ToList());
        }

        public void DeleteMeasurement(string name)
        {
            WriterLock(() => MetadataInternal.Measurements.Remove(name));
        }

        public void DeleteAllMeasurements()
        {
            WriterLock(() => MetadataInternal.Measurements.Clear());
        }

        public IMeasurement GetOrCreateMeasurement(string name)
        {
            return WriterLock(() =>
            {
                if (MetadataInternal.Measurements.ContainsKey(name))
                {
                    return GetMeasurement(name);
                }
                return CreateMeasurement(name, typeof (float));
            });
        }
    }
}