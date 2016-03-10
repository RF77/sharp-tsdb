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
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace Tests.QueryLanguage
{
    [TestFixture]
    public class GroupByTriggerTests
    {
        [SetUp]
        public void Setup()
        {
            _rows.Clear();
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 1, 0, DateTimeKind.Utc), 2));
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 2, 0, DateTimeKind.Utc), 4));
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 8, 0, DateTimeKind.Utc), 6));
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 11, 0, DateTimeKind.Utc), 8));
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 14, 0, DateTimeKind.Utc), 5));
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 17, 0, DateTimeKind.Utc), 4));
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 18, 0, DateTimeKind.Utc), 6));
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 20, 0, DateTimeKind.Utc), 8));

            _serie = new QuerySerie<float>(_rows, new DateTime(1000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(1000, 1, 1, 0, 22, 0, DateTimeKind.Utc))
            {
                PreviousRow = new SingleDataRow<float>(new DateTime(99, 1, 1, 0, 11, 0, DateTimeKind.Utc), 8),
                NextRow = new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 26, 0, DateTimeKind.Utc), 11)
            };

            _serie2 = new QuerySerie<float>(_rows.Take(6).ToList(), new DateTime(1000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(1000, 1, 1, 0, 20, 0, DateTimeKind.Utc))
            {
                PreviousRow = new SingleDataRow<float>(new DateTime(99, 1, 1, 0, 11, 0, DateTimeKind.Utc), 8),
                NextRow = new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 26, 0, DateTimeKind.Utc), 11)
            };

            _serie3 = new QuerySerie<float>(_rows.Take(7).ToList(), new DateTime(1000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(1000, 1, 1, 0, 20, 0, DateTimeKind.Utc))
            {
                PreviousRow = new SingleDataRow<float>(new DateTime(99, 1, 1, 0, 11, 0, DateTimeKind.Utc), 8),
                NextRow = new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 26, 0, DateTimeKind.Utc), 11)
            };
        }

        [TearDown]
        public void TearDown()
        {
        }

        private readonly List<ISingleDataRow<float>> _rows = new List<ISingleDataRow<float>>();
        private QuerySerie<float> _serie;
        private QuerySerie<float> _serie2;
        private QuerySerie<float> _serie3;

        [Test]
        public void GroupTimesDefault_EndIsTrigger_NextIsNull_Test()
        {
            _serie3.EndTime = new DateTime(1000, 1, 1, 0, 18, 0, DateTimeKind.Utc);
            _serie3.NextRow = null;
            var times = _serie3.TimeRanges(t => t.ByTrigger(i => i.Value > 5));

            times.Count.Should().Be(3);
            times[0].Start.Should().Be(new DateTime(1000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            times[0].End.Should().Be(new DateTime(1000, 1, 1, 0, 1, 0, DateTimeKind.Utc));
            times[1].Start.Should().Be(new DateTime(1000, 1, 1, 0, 8, 0, DateTimeKind.Utc));
            times[1].End.Should().Be(new DateTime(1000, 1, 1, 0, 14, 0, DateTimeKind.Utc));
            times[2].Start.Should().Be(new DateTime(1000, 1, 1, 0, 18, 0, DateTimeKind.Utc));
            times[2].End.Should().Be(new DateTime(1000, 1, 1, 0, 18, 0, DateTimeKind.Utc));
        }

        [Test]
        public void GroupTimesDefault2Test()
        {
            var times = _serie2.TimeRanges(t => t.ByTrigger(i => i.Value > 5));

            times.Count.Should().Be(2);
            times[0].Start.Should().Be(new DateTime(1000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            times[0].End.Should().Be(new DateTime(1000, 1, 1, 0, 1, 0, DateTimeKind.Utc));
            times[1].Start.Should().Be(new DateTime(1000, 1, 1, 0, 8, 0, DateTimeKind.Utc));
            times[1].End.Should().Be(new DateTime(1000, 1, 1, 0, 14, 0, DateTimeKind.Utc));
        }

        [Test]
        public void GroupTimesDefault2WithoutNextTest()
        {
            _serie2.NextRow = null;
            var times = _serie2.TimeRanges(t => t.ByTrigger(i => i.Value > 5));

            times.Count.Should().Be(2);
            times[0].Start.Should().Be(new DateTime(1000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            times[0].End.Should().Be(new DateTime(1000, 1, 1, 0, 1, 0, DateTimeKind.Utc));
            times[1].Start.Should().Be(new DateTime(1000, 1, 1, 0, 8, 0, DateTimeKind.Utc));
            times[1].End.Should().Be(new DateTime(1000, 1, 1, 0, 14, 0, DateTimeKind.Utc));
        }

        [Test]
        public void GroupTimesDefaultEndIsTriggerTest()
        {
            _serie3.EndTime = new DateTime(1000, 1, 1, 0, 18, 0, DateTimeKind.Utc);
            var times = _serie3.TimeRanges(t => t.ByTrigger(i => i.Value > 5));

            times.Count.Should().Be(3);
            times[0].Start.Should().Be(new DateTime(1000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            times[0].End.Should().Be(new DateTime(1000, 1, 1, 0, 1, 0, DateTimeKind.Utc));
            times[1].Start.Should().Be(new DateTime(1000, 1, 1, 0, 8, 0, DateTimeKind.Utc));
            times[1].End.Should().Be(new DateTime(1000, 1, 1, 0, 14, 0, DateTimeKind.Utc));
            times[2].Start.Should().Be(new DateTime(1000, 1, 1, 0, 18, 0, DateTimeKind.Utc));
            times[2].End.Should().Be(new DateTime(1000, 1, 1, 0, 18, 0, DateTimeKind.Utc));
        }

        [Test]
        public void GroupTimesDefaultOnly18Test()
        {
            _serie2.EndTime = new DateTime(1000, 1, 1, 0, 18, 0, DateTimeKind.Utc);
            var times = _serie2.TimeRanges(t => t.ByTrigger(i => i.Value > 5));

            times.Count.Should().Be(2);
            times[0].Start.Should().Be(new DateTime(1000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            times[0].End.Should().Be(new DateTime(1000, 1, 1, 0, 1, 0, DateTimeKind.Utc));
            times[1].Start.Should().Be(new DateTime(1000, 1, 1, 0, 8, 0, DateTimeKind.Utc));
            times[1].End.Should().Be(new DateTime(1000, 1, 1, 0, 14, 0, DateTimeKind.Utc));
        }

        [Test]
        public void GroupTimesDefaultTest()
        {
            var times = _serie.TimeRanges(t => t.ByTrigger(i => i.Value > 5));

            times.Count.Should().Be(3);
            times[0].Start.Should().Be(new DateTime(1000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            times[0].End.Should().Be(new DateTime(1000, 1, 1, 0, 1, 0, DateTimeKind.Utc));
            times[1].Start.Should().Be(new DateTime(1000, 1, 1, 0, 8, 0, DateTimeKind.Utc));
            times[1].End.Should().Be(new DateTime(1000, 1, 1, 0, 14, 0, DateTimeKind.Utc));
            times[2].Start.Should().Be(new DateTime(1000, 1, 1, 0, 18, 0, DateTimeKind.Utc));
            times[2].End.Should().Be(new DateTime(1000, 1, 1, 0, 22, 0, DateTimeKind.Utc));
        }

        [Test]
        public void GroupTimesDefaultWithoutNextTest()
        {
            _serie.NextRow = null;
            var times = _serie.TimeRanges(t => t.ByTrigger(i => i.Value > 5));

            times.Count.Should().Be(3);
            times[0].Start.Should().Be(new DateTime(1000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            times[0].End.Should().Be(new DateTime(1000, 1, 1, 0, 1, 0, DateTimeKind.Utc));
            times[1].Start.Should().Be(new DateTime(1000, 1, 1, 0, 8, 0, DateTimeKind.Utc));
            times[1].End.Should().Be(new DateTime(1000, 1, 1, 0, 14, 0, DateTimeKind.Utc));
            times[2].Start.Should().Be(new DateTime(1000, 1, 1, 0, 18, 0, DateTimeKind.Utc));
            times[2].End.Should().Be(new DateTime(1000, 1, 1, 0, 20, 0, DateTimeKind.Utc));
        }

        [Test]
        public void GroupTimesOffsets_EndIsStart_Test()
        {
            _serie3.EndTime = new DateTime(1000, 1, 1, 0, 18, 0, DateTimeKind.Utc);
            _serie3.NextRow = null;
            var times = _serie3.TimeRanges(t => t.ByTrigger(i => i.Value > 5).EndIsStart()
                .StartOffset(TimeSpan.FromMinutes(-2)).EndOffset(TimeSpan.FromMinutes(3)));

            times.Count.Should().Be(3);
            //times[0].Start.Should().Be(new DateTime(1000, 1, 1, 0, 0, 0));
            times[0].End.Should().Be(new DateTime(1000, 1, 1, 0, 3, 0, DateTimeKind.Utc));
            times[1].Start.Should().Be(new DateTime(1000, 1, 1, 0, 6, 0, DateTimeKind.Utc));
            times[1].End.Should().Be(new DateTime(1000, 1, 1, 0, 11, 0, DateTimeKind.Utc));
            times[2].Start.Should().Be(new DateTime(1000, 1, 1, 0, 16, 0, DateTimeKind.Utc));
            times[2].End.Should().Be(new DateTime(1000, 1, 1, 0, 21, 0, DateTimeKind.Utc));
        }

        [Test]
        public void GroupTimesOffsets_EndIsTrigger_NextIsNull_Test()
        {
            _serie3.EndTime = new DateTime(1000, 1, 1, 0, 18, 0, DateTimeKind.Utc);
            _serie3.NextRow = null;
            var times = _serie3.TimeRanges(t => t.ByTrigger(i => i.Value > 5)
                .StartOffset(TimeSpan.FromMinutes(-2)).EndOffset(TimeSpan.FromMinutes(3)));

            times.Count.Should().Be(3);
            // times[0].Start.Should().Be(new DateTime(1000, 1, 1, 0, 0, 0));
            times[0].End.Should().Be(new DateTime(1000, 1, 1, 0, 4, 0, DateTimeKind.Utc));
            times[1].Start.Should().Be(new DateTime(1000, 1, 1, 0, 6, 0, DateTimeKind.Utc));
            times[1].End.Should().Be(new DateTime(1000, 1, 1, 0, 17, 0, DateTimeKind.Utc));
            times[2].Start.Should().Be(new DateTime(1000, 1, 1, 0, 16, 0, DateTimeKind.Utc));
            times[2].End.Should().Be(new DateTime(1000, 1, 1, 0, 21, 0, DateTimeKind.Utc));
        }

        [Test]
        public void GroupTimesOffsets_StartIsEnd_Test()
        {
            _serie3.EndTime = new DateTime(1000, 1, 1, 0, 18, 0, DateTimeKind.Utc);
            _serie3.NextRow = null;
            var times = _serie3.TimeRanges(t => t.ByTrigger(i => i.Value > 5).StartIsEnd()
                );

            times.Count.Should().Be(3);
            times[0].Start.Should().Be(new DateTime(1000, 1, 1, 0, 1, 0, DateTimeKind.Utc));
            times[0].End.Should().Be(new DateTime(1000, 1, 1, 0, 1, 0, DateTimeKind.Utc));
            times[1].Start.Should().Be(new DateTime(1000, 1, 1, 0, 14, 0, DateTimeKind.Utc));
            times[1].End.Should().Be(new DateTime(1000, 1, 1, 0, 14, 0, DateTimeKind.Utc));
            times[2].Start.Should().Be(new DateTime(1000, 1, 1, 0, 18, 0, DateTimeKind.Utc));
            times[2].End.Should().Be(new DateTime(1000, 1, 1, 0, 18, 0, DateTimeKind.Utc));
        }
    }
}