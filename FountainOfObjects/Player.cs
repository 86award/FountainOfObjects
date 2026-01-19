public class Player
{
    public int Row { get; private set; }
    public int Column { get; private set; }

    // perhaps use a switch expression to increment/decrement col/row
    public void UpdatePlayerLocation(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public PlayerLocation GetPlayerLocation()
    {
        return new PlayerLocation(Row, Column);
    }
}

public struct PlayerLocation(int row, int column)
{
    public int Row { get; } = row;
    public int Column { get; } = column;
}