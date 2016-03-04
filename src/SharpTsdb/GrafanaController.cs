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

using System.Reflection;
using System.Web.Http;
using GrafanaAdapter.Queries;
using log4net;

namespace SharpTsdb
{
    [RoutePrefix("")]
    public class GrafanaController : ApiController
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ControllerLogger MeLog = new ControllerLogger(Logger);
        private readonly QueryHandler _handler = new QueryHandler();

        [Route("query")]
        [HttpGet]
        public QueryRoot Get(string db, string q)
        {
            using (MeLog.LogDebug($"db: {db}, q: {q}"))
            {
                return _handler.HandleQuery(db, q, DbService.DbManagement);
            }
        }
    }
}