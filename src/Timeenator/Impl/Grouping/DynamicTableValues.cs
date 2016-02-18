using System.Dynamic;
using DbInterfaces.Interfaces;

namespace QueryLanguage.Grouping
{
    public class DynamicTableValues : DynamicObject
    {
        private IObjectQueryTable _table;

        public DynamicTableValues(IObjectQueryTable table)
        {
            _table = table;
        }

        public int Index { get; set; }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var serie = _table.TryGetSerie(binder.Name);
            if (serie != null)
            {
                serie[Index] = value;
                return true;
            }
            return false;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var serie = _table.TryGetSerie(binder.Name);
            if (serie != null)
            {
                result = serie[Index];
                return true;
            }
            result = null;
            return false;
        }
    }
}
