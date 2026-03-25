public enum TargetType { Player, Enemy, Trash}


// Interface for things to implement so that AI knows what to target
public interface ITargetable
{
    public TargetType TargetType { get; }
}
