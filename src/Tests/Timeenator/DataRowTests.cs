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
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Timeenator.Extensions.Rows;
using Timeenator.Impl;

namespace Tests.Timeenator
{
    [TestFixture]
    public class DataRowTests
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
        public void OnlyValueChangesTest()
        {
            var rows = new DataRow[]
            {
                new DataRow() {Key = new DateTime(2010, 1, 1, 0, 0, 0), Value = 1},
                new DataRow() {Key = new DateTime(2010, 1, 1, 0, 0, 1), Value = 1},
                new DataRow() {Key = new DateTime(2010, 1, 1, 0, 0, 2), Value = 2},
                new DataRow() {Key = new DateTime(2010, 1, 1, 0, 0, 3), Value = 2},
                new DataRow() {Key = new DateTime(2010, 1, 1, 0, 0, 4), Value = 2},
                new DataRow() {Key = new DateTime(2010, 1, 1, 0, 0, 5), Value = 3},
                new DataRow() {Key = new DateTime(2010, 1, 1, 0, 0, 6), Value = 4},
            };

            var result = rows.ValueChanges().ToList();

            result.Count.Should().Be(4);

        }

        [Test]
        public void TimeSpan1ChangesTest()
        {
            var rows = new DataRow[]
            {
                new DataRow() {Key = new DateTime(2010, 1, 1, 0, 0, 0), Value = 1},
                new DataRow() {Key = new DateTime(2010, 1, 1, 0, 0, 1), Value = 1},
                new DataRow() {Key = new DateTime(2010, 1, 1, 0, 0, 2), Value = 2},
                new DataRow() {Key = new DateTime(2010, 1, 1, 0, 1, 3), Value = 2},
                new DataRow() {Key = new DateTime(2010, 1, 1, 0, 1, 23), Value = 2},
                new DataRow() {Key = new DateTime(2010, 1, 1, 0, 2, 5), Value = 3},
                new DataRow() {Key = new DateTime(2010, 1, 1, 0, 2, 6), Value = 4},
            };

            var result = rows.MinimalTimeSpan(TimeSpan.FromSeconds(20)).ToList();
            var result2 = rows.MinimalTimeSpan("20s").ToList();

            result.Count.Should().Be(4);
            result2.SequenceEqual(result).Should().BeTrue();

        }

        [Test]
        public void TimeSpanIsNullChangesTest()
        {
            var rows = new DataRow[]
            {
                new DataRow() {Key = new DateTime(2010, 1, 1, 0, 0, 0), Value = 1},
                new DataRow() {Key = new DateTime(2010, 1, 1, 0, 0, 1), Value = 1},
                new DataRow() {Key = new DateTime(2010, 1, 1, 0, 0, 2), Value = 2},
                new DataRow() {Key = new DateTime(2010, 1, 1, 0, 1, 3), Value = 2},
                new DataRow() {Key = new DateTime(2010, 1, 1, 0, 1, 23), Value = 2},
                new DataRow() {Key = new DateTime(2010, 1, 1, 0, 2, 5), Value = 3},
                new DataRow() {Key = new DateTime(2010, 1, 1, 0, 2, 6), Value = 4},
            };

            var result = rows.MinimalTimeSpan((string)null).ToList();

            result.Count.Should().Be(7);

        }
    }
}