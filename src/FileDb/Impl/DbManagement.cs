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
using System.IO;
using System.Linq;
using System.Reflection;
using DbInterfaces.Interfaces;
using Infrastructure;
using log4net;

namespace FileDb.Impl
{
    public class DbManagement : ReadWritLockable, IDbManagement
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<string, IDb> _loadedDbs = new Dictionary<string, IDb>();
        private readonly string _prefix = "";

        /// <summary>
        ///     Key: DB Name
        ///     Value: Path to Metadata
        /// </summary>
        private Dictionary<string, string> _dbNames = new Dictionary<string, string>();

        public DbManagement() : base(TimeSpan.FromSeconds(30))
        {
            Deserialize();
        }

        public DbManagement(bool test) : base(TimeSpan.FromSeconds(30))
        {
            if (test)
            {
                _prefix = "test_";
            }

            Deserialize();
        }

        private string DbNamesFileName => $"{_prefix}Dbs.json";
        public IEnumerable<IDb> LoadedDbs => _loadedDbs.Values;

        public IDb GetOrCreateDb(string directoryPath, string name)
        {
            var db = LoadedDbs.FirstOrDefault(i => i.Name == name);
            if (db == null)
            {
                if (Directory.Exists(directoryPath))
                {
                    AttachDb(directoryPath);
                    db = GetDb(name);
                }
                else
                {
                    db = CreateDb(directoryPath, name);
                }
            }
            return db;
        }

        public IDb CreateDb(string directoryPath, string name)
        {
            return WriterLock(() =>
            {
                var dirInfo = new DirectoryInfo(Path.Combine(directoryPath, name));
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }

                var metaData = new DbMetadata
                {
                    Name = name,
                    DbPath = dirInfo.FullName,
                    Id = Guid.NewGuid()
                };

                InitDbFromMetadata(name, metaData);

                return GetDb(name);
            });
        }

        public IDb GetDb(string name)
        {
            return ReaderLock(() =>
            {
                IDb db;
                if (!_loadedDbs.TryGetValue(name, out db))
                {
                    var path = _dbNames[name];
                    db = new Db(path.LoadFromFile<DbMetadata>());
                    _loadedDbs[name] = db;
                }
                return db;
            });
        }

        public IReadOnlyList<string> GetDbNames()
        {
            return ReaderLock(() => _dbNames.Keys.ToList());
        }

        public void DeleteDb(string name)
        {
            WriterLock(() =>
            {
                var directoryInfo = new FileInfo(_dbNames[name]).Directory;
                if (directoryInfo?.Exists ?? false)
                {
                    directoryInfo?.Delete(true);
                }
                _dbNames.Remove(name);
                _loadedDbs.Remove(name);
                Serialize();
            });
        }

        public void AttachDb(string dbPath)
        {
            WriterLock(() =>
            {
                var metadataPath = DbMetadata.GetMetadataPath(dbPath);
                var metaData = metadataPath.LoadFromFile<DbMetadata>();
                InitDbFromMetadata(metaData.Name, metaData);
            });
        }

        public void DetachDb(string dbName)
        {
            WriterLock(() =>
            {
                _dbNames.Remove(dbName);
                _loadedDbs.Remove(dbName);
                Serialize();
            });
        }

        public void DetachAllDbs()
        {
            WriterLock(() =>
            {
                _loadedDbs.Clear();
                _dbNames.Clear();
                Serialize();
            });
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

        private void InitDbFromMetadata(string name, DbMetadata metaData)
        {
            _dbNames[name] = metaData.DbMetadataPath;
            Serialize();
            var db = new Db(metaData);
            _loadedDbs[name] = db;
            db.SaveMetadata();
        }
    }
}