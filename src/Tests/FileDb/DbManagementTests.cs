﻿// /*******************************************************************************
//  * Copyright (c) 2016 by RF77 (https://github.com/RF77)
//  * All rights reserved. This program and the accompanying materials
//  * are made available under the terms of the Eclipse Public License v1.0
//  * which accompanies this distribution, and is available at
//  * http://www.eclipse.org/legal/epl-v10.html
//  *
//  * Contributors:
//  *    RF77 - initial API and implementation and/or initial documentation
//  *******************************************************************************/ 

using System.IO;
using System.Linq;
using FileDb.Impl;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.FileDb
{
    [TestFixture]
    public class DbManagementTests
    {
        [SetUp]
        public void Setup()
        {
            _unitUnderTest = new DbManagement(true);
            _unitUnderTest.DetachAllDbs();
        }

        private const string DbRootDir = @"c:\temp\DBs";
        private const string TestDbName = "test_TestDb";
        private const string TestDbName2 = "test_TestDb2";
        private DbManagement _unitUnderTest;

        [Test]
        public void CreateDb()
        {
            _unitUnderTest.CreateDb(DbRootDir, TestDbName);

            Directory.Exists(Path.Combine(DbRootDir, TestDbName));
            _unitUnderTest.GetDbNames().Count.Should().Be(1);
            _unitUnderTest.GetDbNames().First().Should().Be(TestDbName);
        }

        [Test]
        public void DeleteDb()
        {
            _unitUnderTest.CreateDb(DbRootDir, TestDbName);

            var db = _unitUnderTest.GetDb(TestDbName);

            File.Exists(db.Metadata.DbMetadataPath).Should().BeTrue();

            _unitUnderTest.DeleteDb(TestDbName);

            File.Exists(db.Metadata.DbMetadataPath).Should().BeFalse();
        }

        [Test]
        public void Deserialize()
        {
            _unitUnderTest.CreateDb(DbRootDir, TestDbName);

            var secondManagement = new DbManagement(true);
            secondManagement.GetDbNames().Count.Should().Be(1);
        }

        [Test]
        public void DetachAll()
        {
            _unitUnderTest.CreateDb(DbRootDir, TestDbName);

            _unitUnderTest.DetachAllDbs();

            _unitUnderTest.GetDbNames().Count.Should().Be(0);
            _unitUnderTest.LoadedDbs.Any().Should().BeFalse();
            new DirectoryInfo(DbRootDir).Exists.Should().BeTrue();
        }

        [Test]
        public void DetachDb()
        {
            _unitUnderTest.CreateDb(DbRootDir, TestDbName);

            var db = _unitUnderTest.GetDb(TestDbName);

            File.Exists(db.Metadata.DbMetadataPath).Should().BeTrue();

            _unitUnderTest.DetachDb(TestDbName);

            File.Exists(db.Metadata.DbMetadataPath).Should().BeTrue();
        }

        [Test]
        public void ListDbs()
        {
            _unitUnderTest.CreateDb(DbRootDir, TestDbName);
            _unitUnderTest.CreateDb(DbRootDir, TestDbName2);

            var dbList = _unitUnderTest.GetDbNames();

            dbList.Count.Should().Be(2);
        }
    }
}