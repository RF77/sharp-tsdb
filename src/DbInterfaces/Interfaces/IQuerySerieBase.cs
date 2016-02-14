namespace DbInterfaces.Interfaces
{
    public interface IQuerySerieBase<T> : IObjectQuerySerieBase where T : struct
    {

        /// <summary>
        /// Last value before the start time or null
        /// </summary>
        new ISingleDataRow<T> PreviousRow { get; set; }

        /// <summary>
        /// first value after end time or null
        /// </summary>
        new ISingleDataRow<T> NextRow { get; set; }

    }
}