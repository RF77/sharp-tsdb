using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;
using NUnit.Framework;
using QueryLanguage.Grouping;
using QueryLanguage.Scientific;

namespace Tests.QueryLanguage
{
    [TestFixture]
    public class ScientificTests
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
        public void DewPointTest()
        {
            var db = new DbManagement().GetDb("fux");
            var queryTable = db.GetTable<float>("Aussen.Wetterstation.(.*?)$", "time > now() - 1y");
            var result = queryTable
                .Do(i => i.GroupByHours(1, o => o.Mean()))
                .DewPoint("Temperatur", "Feuchtigkeit", "Taupunkt");
        }
    }
}