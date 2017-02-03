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

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Nancy.Json;
using Timeenator.Interfaces;

namespace SharpTsdbClient
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Client
    {
        /// <summary>
        ///     Create a client to access the Sharp TSDB Server
        /// </summary>
        /// <param name="serverAddress">address of Server, e.g. localhost</param>
        /// <param name="port">Port of server (defaults to 9003)</param>
        public Client(string serverAddress,
#if DEBUG
            ushort port = 9004
#else
            ushort port = 9003
#endif

            )
        {
            ServerAddress = serverAddress;
            Port = port;
        }

        public string ServerAddress { get; }
        public ushort Port { get; }

        public DbClient Db(string name)
        {
            return new DbClient(this, name);
        }

        public async Task WriteAsync<T>(IEnumerable<ISingleDataRow<T>> points, string name)
        {
            var httpWebRequest =
                (HttpWebRequest) WebRequest.Create($"http://{ServerAddress}:{Port}/write?db=fux&meas={name}");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(points);

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse) await httpWebRequest.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = await streamReader.ReadToEndAsync();
            }
        }
    }
}