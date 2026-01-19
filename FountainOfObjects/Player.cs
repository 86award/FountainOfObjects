public class Player
{
    public int Column { get; private set; }
    public int Row { get; private set; }

    // perhaps use a switch expression to increment/decrement col/row
    public void UpdatePlayerLocation(int column, int row)
    {
        Column = column;
        Row = row;
    }

    public (int, int) GetPlayerLocation()
    {
        return (Column, Row);
    }
}