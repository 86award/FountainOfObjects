// Make this class static because there's only ever going to be one
public enum MapSizes
{
    Small,
    Medium,
    Large,
}

public class MapManager
{
    public int RowQty { get; private set; }
    public int ColQty { get; private set; }
    public Room[,] Rooms { get; private set; }
    public MapSizes MapSize { get; private set; }

    // I think this is the factory pattern - using a static to effectively call a constructor?
    public static MapManager CreateMap(MapSizes mapSize)
    {
        switch (mapSize)
        {
            case MapSizes.Large:
                return new MapManager(8, 8, MapSizes.Large);
            case MapSizes.Medium:
                return new MapManager(6, 6, MapSizes.Medium);
            case MapSizes.Small: // fallthrough into default
            default:
                return new MapManager(4, 4, MapSizes.Small);
        }
    }

    public MapManager(int row, int column, MapSizes mapSizes)
    {
        RowQty = row;
        ColQty = column;
        MapSize = mapSizes;
        Rooms = new Room[RowQty, ColQty];
        PopulateMapWithRooms(MapSize);
    }

    // need a way to setting the entrance and fountain locs
    private void PopulateMapWithRooms(MapSizes mapSize)
    {
        (int row, int column) fountainLocation = GetFountainLocationForMap(mapSize);

        for (int i = 0; i < Rooms.GetLength(1); i++) // columns
        {
            for (int j = 0; j < Rooms.GetLength(0); j++) // rows
            {
                if (i == 0 && j == 0) Rooms[i, j] = new RoomEntrance();

                // Here I can have a nest switch that puts the fountain in a pre-determined cell based on map size
                else if (i == fountainLocation.row && j == fountainLocation.column) Rooms[i, j] = new RoomFountain(); // this is hard-coded atm so needs refactoring
                // not overly happy with how this works

                else Rooms[i, j] = new Room(); // will populate L-R then next row
            }
        }
    }

    private (int row, int column) GetFountainLocationForMap(MapSizes mapSize)
    {
        return mapSize switch
        {
            MapSizes.Large => (6, 5),
            MapSizes.Medium => (3, 4),
            _ => (0, 2), // small
        };
    }

    public Room ReturnCurrentRoom(PlayerLocation playerLocation)
    {
        return Rooms[playerLocation.Row, playerLocation.Column];
    }
}