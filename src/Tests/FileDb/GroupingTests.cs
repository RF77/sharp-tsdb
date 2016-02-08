using System;
using System.Collections.Generic;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;
using NUnit.Framework;
using QueryLanguage.Grouping;

namespace Tests.FileDb
{
    [TestFixture]
    public class GroupingTests
    {
        private IList<ISingleDataRow<float>> _unitUnderTest40s;
        private IList<ISingleDataRow<float>> _unitUnderTest9m;

        [SetUp]
        public void Setup()
        {
            _unitUnderTest40s = new List<ISingleDataRow<float>>();
            _unitUnderTest9m = new List<ISingleDataRow<float>>();
            int items = 10000;
            var startDate = new DateTime(2010, 1, 1, 13, 27, 14);
            var current40s = startDate;
            var current9m = startDate;

            for (int i = 0; i < 10000; i++)
            {
                _unitUnderTest40s.Add(new SingleDataRow<float>(current40s, 0.5f * i ));
                _unitUnderTest9m.Add(new SingleDataRow<float>(current9m, 0.5f * i ));
                current40s += TimeSpan.FromSeconds(40);
                current9m += TimeSpan.FromMinutes(9);
            }
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void GroupByMinutesTestStartDate()
        {
            _unitUnderTest40s.GroupByMinutes(5, a => a.First());
        }
    }
}