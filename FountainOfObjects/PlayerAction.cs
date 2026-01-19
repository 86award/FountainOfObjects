public class PlayerAction<T>
{
    // a player action is going to contain a string 
    public T value;
    // behaviour based on that string/input
    
}

public struct PlayerLocation(int row, int column)
{
    public int Row { get; } = row;
    public int Column { get; } = column;
}