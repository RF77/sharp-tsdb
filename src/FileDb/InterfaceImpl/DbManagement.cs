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
using DbInterfaces.Interfaces;

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
            //TODO: Safe list and metadat to db directory
        }

        public IDb GetDb(string name)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteDb(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}