using System;
using FluentAssertions;
using NUnit.Framework;
using Timeenator.Extensions;
using Timeenator.Impl;

namespace Tests.FileDb
{
    [TestFixture]
    public class TimeExpressionsTests
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
        public void FixFromTo()
        {
            var ex = new TimeExpression("time > 1455354920s and time < 1455376521s");

            ex.From.Should().Be(new DateTime(2016, 2, 13, 10, 15, 20, DateTimeKind.Local).ToUniversalTime());
            ex.To.Should().Be(new DateTime(2016, 2, 13, 16, 15, 21, DateTimeKind.Local).ToUniversalTime());
        }

        [Test]
        public void FixFrom()
        {
            var ex = new TimeExpression("time > 1455354920s");

            ex.From.Should().Be(new DateTime(2016, 2, 13, 10, 15, 20, DateTimeKind.Local).ToUniversalTime());
            ex.To.Should().Be(null);
        }

        [Test]
        public void FixTo()
        {
            var ex = new TimeExpression("time < 1455376521s");

            ex.From.Should().Be(null);
            ex.To.Should().Be(new DateTime(2016, 2, 13, 16, 15, 21, DateTimeKind.Local).ToUniversalTime());
        }

        [Test]
        public void RelativeFrom()
        {
            var ex = new TimeExpression("time > now() - 6h");

            var shouldTime = DateTime.UtcNow - TimeSpan.FromHours(6);

            var diff = shouldTime - ex.From.Value;

            Math.Abs(diff.TotalMinutes).Should().BeLessThan(1);

        }

        [Test]
        public void RelativeTo()
        {
            var ex = new TimeExpression("time < now() - 6h");

            var shouldTime = DateTime.UtcNow - TimeSpan.FromHours(6);

            var diff = shouldTime - ex.To.Value;

            Math.Abs(diff.TotalMinutes).Should().BeLessThan(1);

        }
    }
}