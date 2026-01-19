// Make this class static because there's only ever going to be one
public class MapManager
{
    public int ColQty { get; private set; }
    public int RowQty { get; private set; }
    public Room[,] Rooms;

    public MapManager(int column, int row)
    {
        ColQty = column;
        RowQty = row;
        Rooms = new Room[ColQty, RowQty];
        PopulateMapWithRooms();
    }

    public void PopulateMapWithRooms()
    {
        for (int i = 0; i < Rooms.GetLength(0); i++)
        {
            for (int j = 0; j < Rooms.GetLength(1); j++)
            {
                Rooms[i, j] = new Room();
            }
        }
    }
}