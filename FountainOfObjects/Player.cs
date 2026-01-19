public class Player
{
    public int Row { get; private set; }
    public int Column { get; private set; }

    public Player(int startingRow, int startingCol)
    {
        Row = startingRow;
        Column = startingCol;
    }

    // perhaps use a switch expression to increment/decrement col/row
    public void SetRelativePlayerLocation(int row, int column)
    {
        Row += row;
        Column += column;
    }

    public void SetAbsolutePlayerLocation(int row, int column)
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