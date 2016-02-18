using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Timeenator.Impl.Grouping;
using Timeenator.Interfaces;

namespace FileDb.Scripting
{
    public class Globals
    {
        public IDb db;
    }

    public class ScriptingEngine
    {
        private IDb _db;
        private string _expression;
        private object _result;
        private Script<IQueryResult> _script;
        private static ScriptOptions _options;
        private static Dictionary<string, Script<IQueryResult>> _scripts = new Dictionary<string, Script<IQueryResult>>();

        public IQueryResult Result => _result as IQueryResult;
        public IObjectQuerySerie ResultAsSerie => _result as IObjectQuerySerie;
        public IObjectQueryTable ResultAsTable => _result as IObjectQueryTable;

        public ScriptingEngine(IDb db, string expression)
        {
            _db = db;
            _expression = expression;
        }

        static ScriptingEngine()
        {
            CreateOptions();
        }

        public static async Task<object> ExecuteTestAsync(string expressionToExecute)
        {
            var script = CSharpScript.Create<object>(expressionToExecute, globalsType: typeof(Globals), options: _options);
            script.Compile();
            return (await script.RunAsync(new Globals())).ReturnValue;
        }


        public ScriptingEngine Execute()
        {
            CreateScript();
            var globals = new Globals()
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
                typeof (Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly,
                typeof (System.Dynamic.DynamicObject).Assembly,
                typeof (System.Dynamic.ExpandoObject).Assembly,
            }.Distinct());

            _options = _options.WithImports(new[]
            {
                "System",
                "System.Collections.Generic",
                "System.Diagnostics",
                "System.Linq",
                "DbInterfaces.Interfaces",
                "Timeenator.Interfaces",
                "FileDb.InterfaceImpl",
                "FileDb",
                "Timeenator.Impl.Grouping",
                "Timeenator.Impl.Scientific",
                "Timeenator.Impl.Converting",
                "Microsoft.CSharp",
                "System.Dynamic"
            });
            
        }

        private void CreateScript()
        {
            var scriptText = ScriptText;

            Script<IQueryResult> existingScript;

            if (_scripts.TryGetValue(scriptText, out existingScript))
            {
                _script = existingScript;
                return;
            }

            _script = CSharpScript.Create<IQueryResult>(scriptText, globalsType: typeof (Globals), options: _options);
            _script.Compile();
            _scripts[scriptText] = _script;
        }

        private string ScriptText => $"db.{_expression}";
    }
}
