using System.Dynamic;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping
{
    public class DynamicTable : DynamicObject
    {
        private readonly IObjectQueryTable _table;

        public DynamicTable(IObjectQueryTable table)
        {
            _table = table;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var serie = _table.TryGetSerie(binder.Name);
            if (serie != null)
            {
                result = serie;
                return true;
            }
            result = null;
            return false;
        }
    }
}
