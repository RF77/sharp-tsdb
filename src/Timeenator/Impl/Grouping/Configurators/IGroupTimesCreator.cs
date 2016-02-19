using System.Collections.Generic;

namespace Timeenator.Impl.Grouping.Configurators
{
    internal interface IGroupTimesCreator
    {
        IReadOnlyList<StartEndTime> CreateGroupTimes();
    }
}