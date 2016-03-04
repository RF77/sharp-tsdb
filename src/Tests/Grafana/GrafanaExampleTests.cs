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

using System.Diagnostics;
using FileDb.Impl;
using NUnit.Framework;
using Timeenator.Extensions.Grouping;

namespace Tests.Grafana
{
    [TestFixture]
    public class GrafanaExampleTests
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
            var dbm = new DbManagement();
            var db = dbm.GetDb("fux");

            var sw = Stopwatch.StartNew();
            var result = db.GetSerie<float>("Aussen.Wetterstation.Temperatur", "time > now() - 2d")
                .GroupBy("6h", a => a.Mean());
            sw.Stop();
        }
    }
}