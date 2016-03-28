using Mqtt2SharpTsdb.Config;

namespace Mqtt2SharpTsdb.Items
{
    public interface IMeasurementItem
    {
        RuleConfiguration RuleConfiguration { get; set; }
        void ReceivedValue(object val);
        void Flush();
    }
}