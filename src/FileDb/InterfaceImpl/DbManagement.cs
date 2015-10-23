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
using DbInterfaces.Interfaces;
using Infrastructure;

namespace FileDb.InterfaceImpl
{
    public class DbManagement : IDbManagement
    {
        public static Dictionary<string, DbMetadata> Dbs = new Dictionary<string, DbMetadata>(); 

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

            Dbs[name] = metaData;

            Serialize();
            metaData.SaveToFile(metaData.DbMetadataPath);
        }

        private static void Serialize()
        {
            Dbs.Values.SaveToUserProfile("Dbs.json");
        }

        public IDb GetDb(string name)
        {
            return ListDbs().First(i => i.Metadata.Name == name);
        }

        public IReadOnlyList<IDb> ListDbs()
        {
            return Dbs.Values.Select(i => new Db(i)).ToList();
        }

        public void DeleteDb(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}