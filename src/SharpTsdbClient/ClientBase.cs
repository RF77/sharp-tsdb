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
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Nancy.Json;
using Serialize.Linq.Serializers;

namespace SharpTsdbClient
{
    public class ClientBase
    {
        protected static readonly ExpressionSerializer LinqSerializer = new ExpressionSerializer(new BinarySerializer());
        private readonly JavaScriptSerializer _scriptSerializer = new JavaScriptSerializer(null,false, Int32.MaxValue, Int32.MaxValue, true, false);
        public DbClient Db { get; set; }

        protected async Task<T> PostRequestAsync<T>(string url, object objectToserialize, bool asJson = true)
        {
            var httpWebRequest =
                (HttpWebRequest)
                    WebRequest.Create($"http://{Db.Client.ServerAddress}:{Db.Client.Port}/{url}");
            httpWebRequest.ContentType = asJson ? "application/json" : "application/octet-stream";
            httpWebRequest.Method = "POST";

            Stream requestStream = httpWebRequest.GetRequestStream();
            if (asJson)
            {
                using (var streamWriter = new StreamWriter(requestStream))
                {
                    string json = _scriptSerializer.Serialize(objectToserialize);
                    streamWriter.Write(json);
                }
            }
            else
            {
                byte[] bytes = (byte[]) objectToserialize;
                requestStream.Write(bytes, 0, bytes.Length);
            }

            var httpResponse = (HttpWebResponse) await httpWebRequest.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = await streamReader.ReadToEndAsync();
                return _scriptSerializer.Deserialize<T>(result);
            }
        }

        protected async Task<string> GetRequestAsync(string url)
        {
            var httpWebRequest =
                (HttpWebRequest)
                    WebRequest.Create($"http://{Db.Client.ServerAddress}:{Db.Client.Port}/{url}");
            //httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    string json = new JavaScriptSerializer().Serialize(objectToserialize);

            //    streamWriter.Write(json);
            //}

            var httpResponse = (HttpWebResponse) await httpWebRequest.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return await streamReader.ReadToEndAsync();
            }
        }
    }
}