namespace Chytra_domacnost.Models;

public enum RuleTargetType
{
    Device = 1,
    Room = 2,
    Household = 3
}

public enum RuleTriggerMetric
{
    PowerConsumption = 1,
    State = 2,
    CurrentValue = 3,
    ActiveDeviceCount = 4,
    AllDevicesOff = 5
}

public enum RuleComparisonType
{
    None = 0,
    GreaterThan = 1,
    LessThan = 2,
    Equal = 3
}

public enum RuleActionType
{
    TurnOnDevice = 1,
    TurnOffDevice = 2,
    DeactivateDevice = 3,
    SetDeviceValue = 4,
    TurnOnAllDevicesInRoom = 5,
    TurnOffAllDevicesInRoom = 6
}

public class RuleTriggerDefinition
{
    public RuleTargetType TargetType { get; set; }
    public RuleTriggerMetric Metric { get; set; }
    public RuleComparisonType Comparison { get; set; }
    public int? DeviceId { get; set; }
    public int? RoomId { get; set; }
    public int? Value { get; set; }
    public Enums.DeviceState? TargetState { get; set; }
}

public class RuleActionDefinition
{
    public RuleActionType ActionType { get; set; }
    public int? DeviceId { get; set; }
    public int? RoomId { get; set; }
    public int? Value { get; set; }
}

public class Rule
{
    public string Name { get; set; }
    public int Id { get; set; }
    public RuleTriggerDefinition TriggerDefinition { get; set; }
    public RuleActionDefinition ActionDefinition { get; set; }

    public Rule()
    {
        Name = string.Empty;
        TriggerDefinition = new RuleTriggerDefinition();
        ActionDefinition = new RuleActionDefinition();
    }

    public Rule(string name, int id, RuleTriggerDefinition triggerDefinition, RuleActionDefinition actionDefinition)
    {
        Name = name;
        Id = id;
        TriggerDefinition = triggerDefinition;
        ActionDefinition = actionDefinition;
    }
}