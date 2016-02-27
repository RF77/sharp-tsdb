using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DbInterfaces.Interfaces;
using SharpTsdbTypes.Communication;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace SharpTsdbClient
{
    public class DbClient : ClientBase
    {
        public Client Client { get; set; }
        public string DbName { get; }

        public DbClient(Client client, string dbName)
        {
            Client = client;
            DbName = dbName;
        }

        public MeasurementClient Measurement(string name)
        {
            return new MeasurementClient(this, name);
        }

        public async Task<IQuerySerie<T>> QuerySerie<T>(Expression<Func<IDb, IQuerySerie<T>>> query) where T:struct
        {
            byte[] queryArray = LinqSerializer.SerializeBinary(query);
            var result = await PostRequestAsync<DataSerie>($"db /{Db.DbName}/binQuerySerie", queryArray, asJson:false);
            return result.ToQuerySerie<T>();
        }
    }
}
