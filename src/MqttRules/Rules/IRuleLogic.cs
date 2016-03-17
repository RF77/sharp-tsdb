namespace MqttRules.Rules
{
    public interface IRuleLogic<T> : IRuleLogic
    {
        
    }
    public interface IRuleLogic : IRuleTask
    {
        IRuleTrigger Or();
    }
}