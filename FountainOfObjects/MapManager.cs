// Make this class static because there's only ever going to be one
public class MapManager
{
    public int RowQty { get; private set; }
    public int ColQty { get; private set; }
    public Room[,] Rooms { get; private set; }

    public MapManager(int row, int column)
    {
        RowQty = row;
        ColQty = column;
        Rooms = new Room[RowQty, ColQty];
        PopulateMapWithRooms();
    }

    // need a way to setting the entrance and fountain locs
    private void PopulateMapWithRooms()
    {
        for (int i = 0; i < Rooms.GetLength(1); i++) // columns
        {
            for (int j = 0; j < Rooms.GetLength(0); j++) // rows
            {
                if (i == 0 && j == 2) Rooms[i, j] = new RoomFountain(); // this is hard-coded atm so needs refactoring
                else Rooms[i, j] = new Room(); // will populate L-R then next row
            }
        }
    }
}