using System;
using System.Linq;
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
            Db = this;
            Client = client;
            DbName = dbName;
        }

        public MeasurementClient Measurement(string name)
        {
            return new MeasurementClient(this, name) {Db = this};
        }

        public async Task<INullableQuerySerie<T>> QuerySerieAsync<T>(Expression<Func<IDb, IObjectQuerySerie>> query) where T : struct
        {
            byte[] queryArray = LinqSerializer.SerializeBinary(query);
            var result = await PostRequestAsync<DataSerie>($"db/{Db.DbName}/binQuerySerie", queryArray, asJson: false);
            return result.ToNullableQuerySerie<T>();
        }

        public async Task<INullableQueryTable<T>> QueryTableAsync<T>(Expression<Func<IDb, IObjectQueryTable>> query) where T : struct
        {
            byte[] queryArray = LinqSerializer.SerializeBinary(query);
            var result = await PostRequestAsync<DataSerie[]>($"db/{Db.DbName}/binQueryTable", queryArray, asJson: false);
            var table = new NullableQueryTable<T>(result.Select(i => i.ToNullableQuerySerie<T>()));
            return table;
        }

        public async Task<bool> CreateOrAtachDbAsync()
        {
            return (await GetRequestAsync($"dbs/createOrAttachDb/{Db.DbName}") == "ok");
        }
    }
}
