namespace Chytra_domacnost.Models;

public class Rule
{
    public Func<bool> Trigger { get; set; }
    public Action Action { get; set; }
    public string Name { get; set; }
    public int Id { get; set; }
    public Action ActionToTrigger { get; set; }
    public Rule(Func<bool> trigger, string name,int id, Action actionToTrigger)
    {
        Trigger = trigger;
        ActionToTrigger = actionToTrigger;
        Name = name;
        Id = id;
    }
}