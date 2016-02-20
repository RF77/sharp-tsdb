using System;
using FluentAssertions;
using NUnit.Framework;
using Timeenator.Impl.Grouping;

namespace Tests.QueryLanguage
{
    [TestFixture]
    public class StartEndTimeTests
    {
        private StartEndTime[] _unitUnderTest;
        private StartEndTime[] _unitUnderTest2;

        [SetUp]
        public void Setup()
        {
            _unitUnderTest = new StartEndTime[]
            {
                new StartEndTime(new DateTime(1000, 1, 1, 0, 0, 0), new DateTime(1000, 1, 1, 0, 10, 0)),
                new StartEndTime(new DateTime(1000, 1, 1, 0, 5, 0), new DateTime(1000, 1, 1, 0, 15, 0)),
                new StartEndTime(new DateTime(1000, 1, 1, 0, 25, 0), new DateTime(1000, 1, 1, 0, 30, 0)),
                new StartEndTime(new DateTime(1000, 1, 1, 0, 35, 0), new DateTime(1000, 1, 1, 0, 40, 0)),
                new StartEndTime(new DateTime(1000, 1, 1, 0, 55, 0), new DateTime(1000, 1, 1, 0, 56, 0)),
                new StartEndTime(new DateTime(1000, 1, 1, 0, 56, 0), new DateTime(1000, 1, 1, 0, 57, 0)),

            };

            _unitUnderTest2 = new StartEndTime[]
{
                new StartEndTime(new DateTime(1000, 1, 1, 0, 0, 0), new DateTime(1000, 1, 1, 0, 10, 0)),
                new StartEndTime(new DateTime(1000, 1, 1, 0, 5, 0), new DateTime(1000, 1, 1, 0, 15, 0)),
                new StartEndTime(new DateTime(1000, 1, 1, 0, 25, 0), new DateTime(1000, 1, 1, 0, 50, 0)),
                new StartEndTime(new DateTime(1000, 1, 1, 0, 27, 0), new DateTime(1000, 1, 1, 0, 35, 0)),
                new StartEndTime(new DateTime(1000, 1, 1, 0, 33, 0), new DateTime(1000, 1, 1, 0, 45, 0)),
                new StartEndTime(new DateTime(1000, 1, 1, 0, 56, 0), new DateTime(1000, 1, 1, 0, 57, 0)),

};
        }

        [TearDown]
        public void TearDown()
        {
        }


        [Test]
        public void CombineByToleranceWith5min()
        {
            var result = _unitUnderTest.CombineByTolerance("5m");

            result.Count.Should().Be(3);
            result[0].Start.Minute.Should().Be(0);
            result[0].End.Minute.Should().Be(15);
            result[1].Start.Minute.Should().Be(25);
            result[1].End.Minute.Should().Be(40);
            result[2].Start.Minute.Should().Be(55);
            result[2].End.Minute.Should().Be(57);
        }


        [Test]
        public void CombineByToleranceWith0min()
        {
            var result = _unitUnderTest.CombineByTolerance("0m");

            result.Count.Should().Be(4);
            result[3].Start.Minute.Should().Be(55);
            result[3].End.Minute.Should().Be(57);

        }

        [Test]
        public void CombineByToleranceWith1minAndNested()
        {
            var result = _unitUnderTest2.CombineByTolerance("1m");

            result.Count.Should().Be(3);
            result[1].Start.Minute.Should().Be(25);
            result[1].End.Minute.Should().Be(50);

        }


    }
}