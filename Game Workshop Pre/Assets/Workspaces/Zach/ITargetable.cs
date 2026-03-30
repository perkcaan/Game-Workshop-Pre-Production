using System.Diagnostics;

public enum TargetType { Player, Enemy, Trash, Null}


// Interface for things to implement so that AI knows what to target
public interface ITargetable
{
    public TargetType TargetType { get; set; }

    public void SwitchType(TargetType newType)
    {
        TargetType = newType;
    }
}
