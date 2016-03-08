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
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using DbInterfaces.Interfaces;
using FileDb.Impl;
using log4net;
using Serialize.Linq.Serializers;
using SharpTsdb.Properties;
using SharpTsdbTypes.Communication;
using Timeenator.Extensions;
using Timeenator.Interfaces;

namespace SharpTsdb
{
    [RoutePrefix("")]
    public class DbController : ApiController
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ControllerLogger MeLog = new ControllerLogger(Logger);
        private static readonly ExpressionSerializer LinqSerializer = new ExpressionSerializer(new BinarySerializer());
        private static readonly ReadWritLockable Locker = new ReadWritLockable(TimeSpan.FromMinutes(3));

        [Route("dbs/createDb/{dbName}")]
        [HttpGet]
        public string CreateDb(string dbName)
        {
            using (MeLog.LogDebug($"name: {dbName}"))
            {
                Locker.WriterLock(() =>
                {
                    DbService.DbManagement.CreateDb(Settings.Default.DbDirectory, dbName);
                });
                return "ok";
            }
        }

        [Route("dbs/createOrAttachDb/{dbName}")]
        [HttpGet]
        public string CreateOrAttachDb(string dbName)
        {
            using (MeLog.LogDebug($"name: {dbName}"))
            {
                Locker.WriterLock(() =>
                {
                    DbService.DbManagement.GetOrCreateDb(Path.Combine(Settings.Default.DbDirectory, dbName), dbName);
                });
                return "ok";
            }
        }

        [Route("db/{dbName}/createMeasurment/{name}")]
        [HttpGet]
        public string CreateMeasurement(string dbName, string name, string type = "float")
        {
            using (MeLog.LogDebug($"db: {dbName}, meas: {name}"))
            {
                Locker.WriterLock(() =>
                {
                    var myDb = DbService.DbManagement.GetDb(dbName);
                    myDb.CreateMeasurement(name, type.ToType());
                });
                return "ok";
            }
        }

        [Route("db/{dbName}/{measurementName}/clear")]
        [HttpGet]
        public string ClearMeasurement(string dbName, string measurementName, long? after)
        {
            using (MeLog.LogDebug($"db: {dbName}, meas: {measurementName}, after: {after}"))
            {
                Locker.WriterLock(() =>
                {

                    var myDb = DbService.DbManagement.GetDb(dbName);
                    DateTime? afterTime = null;
                    if (after != null)
                    {
                        afterTime = DateTime.FromFileTimeUtc(after.Value);
                    }
                    myDb.GetMeasurement(measurementName).ClearDataPoints(afterTime);
                });
                return "ok";
            }
        }

        [Route("db/{dbName}/{measurementNameRegex}/addAlias/{aliasName}")]
        [HttpGet]
        public string AddAliasToMeasurements(string dbName, string measurementNameRegex, string aliasName)
        {
            using (MeLog.LogDebug($"db: {dbName}, meas: {measurementNameRegex}, alias: {aliasName}"))
            {
                Locker.WriterLock(() =>
                {
                    var myDb = DbService.DbManagement.GetDb(dbName);
                    myDb.AddAliasToMeasurements(measurementNameRegex, aliasName);
                });
                return "ok";
            }
        }

        [Route("db/{dbName}/allMeasurementsAsText")]
        [HttpGet]
        public HttpResponseMessage AllMeasurementsAsText(string dbName)
        {
            using (MeLog.LogDebug($"db: {dbName}"))
            {
                var resp = new HttpResponseMessage(HttpStatusCode.OK);

                var result = Locker.WriterLock(() =>
                {
                    var myDb = DbService.DbManagement.GetDb(dbName);
                    var measurements = myDb.GetMeasurementNames().OrderBy(i => i);
                    var nameString = string.Join("\r\n", measurements);
                    return nameString;
                });
                resp.Content = new StringContent(result, System.Text.Encoding.UTF8, "text/plain");
                return resp;
            }
        }

        [Route("db/{dbName}/allMeasurementsNamesAsText")]
        [HttpGet]
        public HttpResponseMessage AllMeasurementsNamesAsText(string dbName)
        {
            using (MeLog.LogDebug($"db: {dbName}"))
            {
                var resp = new HttpResponseMessage(HttpStatusCode.OK);

                var result = Locker.WriterLock(() =>
                {
                    var myDb = DbService.DbManagement.GetDb(dbName);
                    var measurements = myDb.Metadata.Measurements.Values;
                    var values = measurements.Select(i => string.Join("/", i.NameAndAliases)).OrderBy(i => i);
                    var nameString = string.Join("\r\n", values);
                    return nameString;
                });
                resp.Content = new StringContent(result, System.Text.Encoding.UTF8, "text/plain");
                return resp;
            }
        }

        [Route("db/{dbName}/findMeasurements/{regex}")]
        [HttpGet]
        public HttpResponseMessage FindMeasurementsAsText(string dbName, string regex)
        {
            using (MeLog.LogDebug($"db: {dbName}"))
            {
                var resp = new HttpResponseMessage(HttpStatusCode.OK);

                var result = Locker.WriterLock(() =>
                {
                    var myDb = DbService.DbManagement.GetDb(dbName);
                    var measurements = myDb.GetMeasurementNames().Where(i => Regex.IsMatch(i, regex)).OrderBy(i => i);
                    var nameString = string.Join("\r\n", measurements);
                    return nameString;
                });
                resp.Content = new StringContent(result, System.Text.Encoding.UTF8, "text/plain");
                return resp;
            }
        }

        [Route("db/{dbName}/{meas}/appendRows")]
        [HttpPost]
        public string WritePoints(string dbName, string meas, [FromBody] DataRows data, string type = "float",
            bool truncateDbToFirstElement = false)
        {
            using (
                MeLog.LogDebug(
                    $"db: {dbName}, meas: {meas}, point#: {data.Rows.Count}, trunc: {truncateDbToFirstElement}"))
            {
                IMeasurement measurement = null;
                Locker.WriterLock(() =>
                {
                    var myDb = DbService.DbManagement.GetDb(dbName);
                    measurement = myDb.GetOrCreateMeasurement(meas, type);
                });

                measurement.AppendDataPoints(data.AsIDataRows());
                return "ok";
            }
        }

        [Route("db/{dbName}/binQuerySerie")]
        [HttpPost]
        public async Task<DataSerie> BinaryQuerySerie(string dbName)
        {
            using (MeLog.LogDebug($"db: {dbName}"))
            {

                byte[] query = await Request.Content.ReadAsByteArrayAsync();

                var queryExpression =
                    ((Expression<Func<IDb, IObjectQuerySerie>>) LinqSerializer.DeserializeBinary(query)).Compile();

                Locker.ReaderLock(() =>
                {
                    var myDb = DbService.DbManagement.GetDb(dbName);
                    IObjectQuerySerie result = queryExpression(myDb);
                    return new DataSerie(result);
                });
                return null;
            }
        }

        [Route("db/{dbName}/binQueryTable")]
        [HttpPost]
        public async Task<DataSerie[]> BinaryQueryTable(string dbName)
        {
            using (MeLog.LogDebug($"db: {dbName}"))
            {
                byte[] query = await Request.Content.ReadAsByteArrayAsync();

                var queryExpression =
                    ((Expression<Func<IDb, IObjectQueryTable>>) LinqSerializer.DeserializeBinary(query)).Compile();

                Locker.ReaderLock(() =>
                {
                    var myDb = DbService.DbManagement.GetDb(dbName);
                    IObjectQueryTable result = queryExpression(myDb);
                    return result.Series.Select(i => new DataSerie(i)).ToArray();
                });
                return null;
            }
        }
    }
}