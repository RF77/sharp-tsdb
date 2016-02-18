using DbInterfaces.Interfaces;

namespace QueryLanguage.DataRows
{
    public class NamedDataRow<T> where T:struct 
    {
        public string Name { get; set; }
        public ISingleDataRow<T> Rows { get; set; }

        public NamedDataRow(string name, ISingleDataRow<T> rows)
        {
            Name = name;
            Rows = rows;
        }
    }
}
