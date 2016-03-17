using System.Collections.Generic;
using System.Collections.Immutable;

namespace MqttRules.Items
{
    public interface IGroup
    {
        string Name { get; set; }
        IReadOnlyList<IMqttItem> Items { get; }
        ImmutableHashSet<IMqttItem> FlattenItems { get; }
        ImmutableHashSet<IGroup> Parents { get; }
        ImmutableHashSet<IGroup> Children { get; }
        ImmutableHashSet<IGroup> FlattenChildren { get; }
    }
}