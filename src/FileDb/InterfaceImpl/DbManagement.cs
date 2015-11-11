// /*******************************************************************************
//  * Copyright (c) 2015 by RF77 (https://github.com/RF77)
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
using System.IO;
using System.Linq;
using System.Reflection;
using DbInterfaces.Interfaces;
using Infrastructure;
using log4net;

namespace FileDb.InterfaceImpl
{
    public class DbManagement : IDbManagement
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Key: DB Name
        /// Value: Path to Metadata
        /// </summary>
        private Dictionary<string, string> _dbNames = new Dictionary<string, string>();

        private readonly Dictionary<string, IDb> _loadedDbs = new Dictionary<string, IDb>();

        private readonly string _prefix = "";

        private string DbNamesFileName => $"{_prefix}Dbs.json";


        public DbManagement()
        {
            Deserialize();
        }

        public DbManagement(bool test)
        {
            if (test)
            {
                _prefix = "test_";
            }

            Deserialize();
        }

        private void Deserialize()
        {
            try
            {
                _dbNames = DbNamesFileName.LoadFromUserProfile<Dictionary<string, string>>();
            }
            catch (Exception ex)
            {
                Logger.Warn($"Deserialize(): Failed -> {ex.Message}");
            }
            if (_dbNames == null)
            {
                _dbNames = new Dictionary<string, string>();
            }
        }

        private void Serialize()
        {
            _dbNames.SaveToUserProfile(DbNamesFileName);
        }

        public IDb CreateDb(string directoryPath, string name)
        {
            var dirInfo = new DirectoryInfo(Path.Combine(directoryPath, name));
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            var metaData = new DbMetadata()
            {
                Name = name,
                DbPath = dirInfo.FullName,
                Id = Guid.NewGuid()
            };

            InitDbFromMetadata(name, metaData);
            
            return GetDb(name);
        }

        private void InitDbFromMetadata(string name, DbMetadata metaData)
        {
            _dbNames[name] = metaData.DbMetadataPath;
            Serialize();
            var db = new Db(metaData);
            _loadedDbs[name] = db;
            db.SaveMetadata();
        }


        public IDb GetDb(string name)
        {
            IDb db;
            if (!_loadedDbs.TryGetValue(name, out db))
            {
                var path = _dbNames[name];
                db = new Db(path.LoadFromFile<DbMetadata>());
            }
            return db;
        }

        public IReadOnlyList<string> GetDbNames()
        {
            return _dbNames.Keys.ToList();
        }

        public void DeleteDb(string name)
        {
            var directoryInfo = new FileInfo(_dbNames[name]).Directory;
            directoryInfo?.Delete(true);
        }

        public void AttachDb(string dbPath)
        {
            var metadataPath = DbMetadata.GetMetadataPath(dbPath);
            var metaData = metadataPath.LoadFromFile<DbMetadata>();
            InitDbFromMetadata(metaData.Name, metaData);
        }

        public void DetachDb(string dbName)
        {
            _dbNames.Remove(dbName);
            _loadedDbs.Remove(dbName);
            Serialize();
        }

        public void DetachAllDbs()
        {
            _loadedDbs.Clear();
            _dbNames.Clear();
            Serialize();
        }

        public IEnumerable<IDb> LoadedDbs => _loadedDbs.Values;
    }
}