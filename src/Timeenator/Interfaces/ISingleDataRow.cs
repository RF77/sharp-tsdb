namespace Timeenator.Interfaces
{
    public interface ISingleDataRow<T> : IObjectSingleDataRow
    {
        new T Value { get; set; }
    }
}