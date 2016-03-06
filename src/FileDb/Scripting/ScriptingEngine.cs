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
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using CustomDbExtensions;
using DbInterfaces.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CSharp.RuntimeBinder;
using Timeenator.Interfaces;

namespace FileDb.Scripting
{
    public class Globals
    {
        public IDb db;
    }

    public class ScriptingEngine
    {
        private static ScriptOptions _options;

        private static readonly Dictionary<string, Script<IQueryResult>> _scripts =
            new Dictionary<string, Script<IQueryResult>>();

        private readonly IDb _db;
        private object _result;
        private Script<IQueryResult> _script;

        static ScriptingEngine()
        {
            CreateOptions();
        }

        public ScriptingEngine(IDb db, string expression)
        {
            _db = db;
            ScriptText = expression;
        }

        public IQueryResult Result => _result as IQueryResult;
        private string ScriptText { get; }

        public static async Task<object> ExecuteTestAsync(string expressionToExecute)
        {
            var script = CSharpScript.Create<object>(expressionToExecute, globalsType: typeof (Globals),
                options: _options);
            script.Compile();
            return (await script.RunAsync(new Globals())).ReturnValue;
        }

        public ScriptingEngine Execute()
        {
            CreateScript();
            var globals = new Globals
            {
                db = _db
            };

            ScriptState state = null;

            _script.RunAsync(globals).ContinueWith(s => state = s.Result).Wait();

            _result = state.ReturnValue;

            return this;
        }

        private static void CreateOptions()
        {
            _options = ScriptOptions.Default;
            _options = _options.WithReferences(new[]
            {
                typeof (DateTime).Assembly,
                typeof (IEnumerable<>).Assembly,
                typeof (Enumerable).Assembly,
                typeof (IObjectQuerySerie).Assembly,
                typeof (IDb).Assembly,
                typeof (ScriptingEngine).Assembly,
                typeof (CSharpArgumentInfo).Assembly,
                typeof (DynamicObject).Assembly,
                typeof (ExpandoObject).Assembly,
                typeof (DbExtensions).Assembly
            }.Distinct());

            _options = _options.WithImports("System", "System.Collections.Generic", "System.Diagnostics", "System.Linq",
                "DbInterfaces.Interfaces", "Timeenator.Interfaces", "FileDb.Interfaces", "FileDb", "Timeenator.Impl",
                "Timeenator.Impl.Grouping", "Timeenator.Extensions.Grouping", "Timeenator.Extensions.Scientific",
                "Timeenator.Extensions.Converting", "Microsoft.CSharp", "System.Dynamic", "DbInterfaces.Interfaces",
                "CustomDbExtensions");
        }

        private void CreateScript()
        {
            var scriptText = ScriptText;
            lock (typeof(ScriptingEngine))
            {
                Script<IQueryResult> existingScript;

                if (_scripts.TryGetValue(scriptText, out existingScript))
                {
                    _script = existingScript;
                    return;
                }                
            }

            _script = CSharpScript.Create<IQueryResult>(scriptText, globalsType: typeof (Globals), options: _options);
            _script.Compile();

            lock (typeof (ScriptingEngine))
            {
                _scripts[scriptText] = _script;
            }
        }
    }
}