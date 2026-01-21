public enum ActionType
{
    Move,
    Shoot,
    Interact,
    Help,
}
// public class PlayerAction<T>
// {
//     // a player action is going to contain a string 
//     public T value;
//     // behaviour based on that string/input
    
// }

public struct PlayerLocation(int row, int column) // REMINDER: doesn't make sense for this to exist here; move to player.
{
    public int Row { get; } = row;
    public int Column { get; } = column;
}