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
        public static Dictionary<string, string> DbNames = new Dictionary<string, string>();

        public static Dictionary<string, IDb> LoadedDbs = new Dictionary<string, IDb>();

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
                DbNames = DbNamesFileName.LoadFromUserProfile<Dictionary<string, string>>();
            }
            catch (Exception ex)
            {
                Logger.Warn($"Deserialize(): Failed -> {ex.Message}");
            }
        }

        private void Serialize()
        {
            DbNames.SaveToUserProfile(DbNamesFileName);
        }

        public void CreateDb(string directoryPath, string name)
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

            metaData.SaveToFile(metaData.DbMetadataPath);
        }

        private void InitDbFromMetadata(string name, DbMetadata metaData)
        {
            DbNames[name] = metaData.DbMetadataPath;
            Serialize();
            LoadedDbs[name] = new Db(metaData);
        }


        public IDb GetDb(string name)
        {
            IDb db;
            if (!LoadedDbs.TryGetValue(name, out db))
            {
                var path = DbNames[name];
                db = new Db(path.LoadFromFile<DbMetadata>());
            }
            return db;
        }

        public IReadOnlyList<string> GetDbNames()
        {
            return DbNames.Values.ToList();
        }

        public void DeleteDb(string name)
        {
            new FileInfo(DbNames[name]).Directory.Delete(true);
        }

        public void AttachDb(string dbPath)
        {
            var metadataPath = DbMetadata.GetMetadataPath(dbPath);
            var metaData = metadataPath.LoadFromFile<DbMetadata>();
            InitDbFromMetadata(metaData.Name, metaData);
        }

        public void DetachDb(string dbName)
        {
            DbNames.Remove(dbName);
            LoadedDbs.Remove(dbName);
            Serialize();
        }

        public void DetachAllDbs()
        {
            LoadedDbs.Clear();
            DbNames.Clear();
            Serialize();
        }
    }
}