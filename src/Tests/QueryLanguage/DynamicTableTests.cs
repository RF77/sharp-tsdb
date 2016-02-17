using System;
using System.Collections.Generic;
using System.Diagnostics;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;
using NUnit.Framework;
using QueryLanguage.Converting;
using QueryLanguage.Grouping;
using QueryLanguage.Scientific;
using QueryLanguage.Scripting;

namespace Tests.QueryLanguage
{
    [TestFixture]
    public class DynamicTableTests
    {

        [SetUp]
        public void Setup()
        {
           
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void DynamicTableInScript()
        {
            var db = new DbManagement().GetDb("fux");
            var result = new ScriptingEngine(db, @"GetTable<float>(""Aussen.Wetterstation.(?<k>[TF]).*?$"", ""time > now() - 1M"")
.Do(i => i.GroupByHours(1, o => o.Mean()))
.ZipAndAdd<float>(""Sum"", t => t.T + t.F)").Execute();
        }

        [Test]
        public void DynamicWithRealDb()
        {
            var db = new DbManagement().GetDb("fux");
            var queryTable = db.GetTable<float>("Aussen.Wetterstation.(?<k>[TF]).*?$", "time > now() - 1M");
            var result = queryTable
                .Do(i => i.GroupByHours(1, o => o.Mean()))
                .ZipAndAdd<float>("Sum", t => t.T + t.F);
        }

        private int DummyLambda(Func<dynamic, dynamic> testFunc)
        {
            return (int)testFunc(5);
        }

        dynamic Dummy(dynamic a)
        {
            return a + 2;
        }

        [Test]
        public void DynamicSerieTests()
        {
            DummyLambda(a => a);
            Dummy(3);

            const int max = 100000;
            var start = new DateTime(1000,1,1);
            var rowsA = new List<ISingleDataRow<int>>();
            var rowsB = new List<ISingleDataRow<int>>();
            

            for (int i = 0; i < max; i++)
            {
                rowsA.Add(new SingleDataRow<int>(start + TimeSpan.FromMinutes(i), i));
                rowsB.Add(new SingleDataRow<int>(start + TimeSpan.FromMinutes(i), i*2));
            }

            var serieA = new QuerySerie<int>(rowsA, start, start + TimeSpan.FromMinutes(max)) { Name = "A", Key = "A"};
            var serieB = new QuerySerie<int>(rowsB, start, start + TimeSpan.FromMinutes(max)) { Name = "B", Key = "B" };
            var table = new QueryTable<int>();
            table.AddSerie(serieA);
            table.AddSerie(serieB);

            //var sw = Stopwatch.StartNew();
            //var result = table.Do(i => i.GroupByHours(1, t => t.Mean())).ZipToNew<int>("SumTable", t => t.A + t.B);
            //sw.Stop();

            var sw2 = Stopwatch.StartNew();
            var result2 = table.Do(i => i.GroupByMinutes(1, t => t.First())).ZipToNew<int>("DiffTable", t => t.A - t.B);
            sw2.Stop();
        }


    }
}