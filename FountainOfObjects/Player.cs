public class Player
{
    public Player(int startingRow, int startingCol)
    {
        Row = startingRow;
        Column = startingCol;
        Weapon = new Weapon("Bow", 5);
    }

    public int Row { get; private set; }
    public int Column { get; private set; }
    public Weapon Weapon { get; private set; }

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
    public ShootDirection CreateShootDirection(string enteredText)
    {
        return enteredText switch
        {
            "shoot north" => new ShootDirection(-1, 0),
            "shoot south" => new ShootDirection(1, 0),
            "shoot east" => new ShootDirection(0, 1),
            "shoot west" => new ShootDirection(0, -1),
        };
    }
}

public struct ShootDirection(int row, int column)
{
    public int Row { get; } = row;
    public int Column { get; } = column;
}