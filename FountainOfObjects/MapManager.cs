// Make this class static because there's only ever going to be one

public enum MapSizes
{
    Small,
    Medium,
    Large,
}

public enum CardinalPoints
{
    North,
    South,
    East,
    West,
    NE,
    SE,
    SW,
    NW,
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

                else if (i == 1 && j == 1) Rooms[i, j] = new RoomPit();

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

    public void DisplayAdjacentRoomDescriptions(PlayerLocation playerLocation)
    {
        string _descriptionString = "";
        // data driven array length
        Room[] _adjacentRooms = new Room[Enum.GetValues<CardinalPoints>().Length];

        // I could look at using some CONST values to represent compass points instead of using +/-1 i.e. playerLocation.North
        _adjacentRooms[(int)CardinalPoints.North] = playerLocation.Row > 0 ? ReturnRoomType(Rooms[playerLocation.Row - 1, playerLocation.Column]) : null;
        _adjacentRooms[(int)CardinalPoints.South] = playerLocation.Row < RowQty - 1 ? ReturnRoomType(Rooms[playerLocation.Row + 1, playerLocation.Column]) : null;
        _adjacentRooms[(int)CardinalPoints.East] = playerLocation.Column < ColQty - 1 ? ReturnRoomType(Rooms[playerLocation.Row, playerLocation.Column + 1]) : null;
        _adjacentRooms[(int)CardinalPoints.West] = playerLocation.Column > 0 ? ReturnRoomType(Rooms[playerLocation.Row, playerLocation.Column - 1]) : null;

        _adjacentRooms[(int)CardinalPoints.NE] = playerLocation.Row > 0 && playerLocation.Column < ColQty - 1 ? ReturnRoomType(Rooms[playerLocation.Row - 1, playerLocation.Column + 1]) : null;
        _adjacentRooms[(int)CardinalPoints.SE] = playerLocation.Row < RowQty - 1 && playerLocation.Column < ColQty - 1 ? ReturnRoomType(Rooms[playerLocation.Row + 1, playerLocation.Column + 1]) : null;
        _adjacentRooms[(int)CardinalPoints.SW] = playerLocation.Row < RowQty - 1 && playerLocation.Column < 0 ? ReturnRoomType(Rooms[playerLocation.Row + 1, playerLocation.Column - 1]) : null;
        _adjacentRooms[(int)CardinalPoints.NW] = playerLocation.Row > 0 && playerLocation.Column > 0 ? ReturnRoomType(Rooms[playerLocation.Row - 1, playerLocation.Column - 1]) : null;

        // foreach (Room room in _adjacentRooms)
        for (int i = 0; i < _adjacentRooms.Length; i++)
        {
            if (_adjacentRooms[i] != null)
            {
                // is this not the right place for a generic?
                if (_adjacentRooms[i].GetType() == typeof(RoomFountain) ||
                    _adjacentRooms[i].GetType() == typeof(RoomEntrance) ||
                    _adjacentRooms[i].GetType() == typeof(RoomPit))
                {
                    _descriptionString += i switch
                    {
                        0 => $"To the North {_adjacentRooms[i].RoomSense}",
                        1 => $"To the South {_adjacentRooms[i].RoomSense}",
                        2 => $"To the East {_adjacentRooms[i].RoomSense}",
                        3 => $"To the West {_adjacentRooms[i].RoomSense}",
                        4 => $"To the North East {_adjacentRooms[i].RoomSense}",
                        5 => $"To the South East {_adjacentRooms[i].RoomSense}",
                        6 => $"To the South West {_adjacentRooms[i].RoomSense}",
                        7 => $"To the North West {_adjacentRooms[i].RoomSense}",
                    };
                }
                else if (_adjacentRooms[i].monster != null)
                {
                    // not good to type check vs. a string
                    if (_adjacentRooms[i].monster.Name != null)
                    {
                        _descriptionString += i switch
                        {
                            0 => $"To the North {_adjacentRooms[i].monster.MonsterSenseDescription}",
                            1 => $"To the South {_adjacentRooms[i].monster.MonsterSenseDescription}",
                            2 => $"To the East {_adjacentRooms[i].monster.MonsterSenseDescription}",
                            3 => $"To the West {_adjacentRooms[i].monster.MonsterSenseDescription}",
                            4 => $"To the North East {_adjacentRooms[i].monster.MonsterSenseDescription}",
                            5 => $"To the South East {_adjacentRooms[i].monster.MonsterSenseDescription}",
                            6 => $"To the South West {_adjacentRooms[i].monster.MonsterSenseDescription}",
                            7 => $"To the North West {_adjacentRooms[i].monster.MonsterSenseDescription}",
                        };
                    }
                }
            }
            // else
            // {
            //     _descriptionString += i switch
            //     {
            //         0 => $"To the North there is no room. ",
            //         1 => $"To the South there is no room. ",
            //         2 => $"To the East there is no room. ",
            //         3 => $"To the West there is no room. ",
            //     };
            // }

        }
        Console.WriteLine(_descriptionString);
    }

    // I want to pass in relative cell references and get back the room type
    public Room ReturnRoomType(Room room)
    {
        // if (room.GetType() == typeof(Room)) return room;
        // else return null; // should I be returning a null - doesn't feel right

        // Had to get some AI assistance with this. 
        // what I really wanted was to return the room type and I think the ternary operator above prevents nulls from being passed to this method anyway.
        return room;
    }
}