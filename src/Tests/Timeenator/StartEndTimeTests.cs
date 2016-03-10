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
using FluentAssertions;
using NUnit.Framework;
using Timeenator.Extensions.Grouping;
using Timeenator.Impl.Grouping;

namespace Tests.QueryLanguage
{
    [TestFixture]
    public class StartEndTimeTests
    {
        [SetUp]
        public void Setup()
        {
            _unitUnderTest = new[]
            {
                new StartEndTime(new DateTime(1000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(1000, 1, 1, 0, 10, 0, DateTimeKind.Utc)),
                new StartEndTime(new DateTime(1000, 1, 1, 0, 5, 0, DateTimeKind.Utc),
                    new DateTime(1000, 1, 1, 0, 15, 0, DateTimeKind.Utc)),
                new StartEndTime(new DateTime(1000, 1, 1, 0, 25, 0, DateTimeKind.Utc),
                    new DateTime(1000, 1, 1, 0, 30, 0, DateTimeKind.Utc)),
                new StartEndTime(new DateTime(1000, 1, 1, 0, 35, 0, DateTimeKind.Utc),
                    new DateTime(1000, 1, 1, 0, 40, 0, DateTimeKind.Utc)),
                new StartEndTime(new DateTime(1000, 1, 1, 0, 55, 0, DateTimeKind.Utc),
                    new DateTime(1000, 1, 1, 0, 56, 0, DateTimeKind.Utc)),
                new StartEndTime(new DateTime(1000, 1, 1, 0, 56, 0, DateTimeKind.Utc),
                    new DateTime(1000, 1, 1, 0, 57, 0, DateTimeKind.Utc))
            };

            _unitUnderTest2 = new[]
            {
                new StartEndTime(new DateTime(1000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(1000, 1, 1, 0, 10, 0, DateTimeKind.Utc)),
                new StartEndTime(new DateTime(1000, 1, 1, 0, 5, 0, DateTimeKind.Utc),
                    new DateTime(1000, 1, 1, 0, 15, 0, DateTimeKind.Utc)),
                new StartEndTime(new DateTime(1000, 1, 1, 0, 25, 0, DateTimeKind.Utc),
                    new DateTime(1000, 1, 1, 0, 50, 0, DateTimeKind.Utc)),
                new StartEndTime(new DateTime(1000, 1, 1, 0, 27, 0, DateTimeKind.Utc),
                    new DateTime(1000, 1, 1, 0, 35, 0, DateTimeKind.Utc)),
                new StartEndTime(new DateTime(1000, 1, 1, 0, 33, 0, DateTimeKind.Utc),
                    new DateTime(1000, 1, 1, 0, 45, 0, DateTimeKind.Utc)),
                new StartEndTime(new DateTime(1000, 1, 1, 0, 56, 0, DateTimeKind.Utc),
                    new DateTime(1000, 1, 1, 0, 57, 0, DateTimeKind.Utc))
            };
        }

        [TearDown]
        public void TearDown()
        {
        }

        private StartEndTime[] _unitUnderTest;
        private StartEndTime[] _unitUnderTest2;

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
    }
}