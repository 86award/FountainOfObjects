using System.Runtime.InteropServices.Swift;

public class GameManager
{
    private bool gameActive = true;
    private bool isFountainEnabled = false;
    private bool isPlayerAtExit;

    public void InitialiseGame()
    {
        MapManager? map = null;
        
        bool mapSizeSet = false;
        
        do
        {
            Console.Write($"Please choose a map size to determine the level of difficulty (small, medium, large): ");
            string mapSize = Console.ReadLine() ?? "small".ToLower().Trim();
            // I want to use an enum to force the map size from a limited range
            if (Enum.TryParse<MapSizes>(mapSize, true, out MapSizes result))
            {
                map = MapManager.CreateMap(result);
                mapSizeSet = true;
            }
            else
            {
                Console.WriteLine("Invalid map size.");
            }
        } while (mapSizeSet == false);

        Player player = new Player(0, 0);

        Console.WriteLine();
        Console.WriteLine("---------------------------------------------------------------------------");
        Console.WriteLine("| You find yourself at the mouth of a dark cave system. You take a deep   |");
        Console.WriteLine("| breath and enter.                                                       |");
        Console.WriteLine("---------------------------------------------------------------------------");

        while (gameActive)
        {
            string? enteredText;

            Console.WriteLine();
            DisplayPlayerLocString(player);
            DisplayRoomDescription(player, map);

            do
            {
                // it would be nice if the room that the player was in determined valid moves - add later
                Console.Write("Please enter a valid action e.g., ");
                WriteColourText("move north/south/east/west", ConsoleColor.Yellow);
                Console.Write(": ");
                enteredText = Console.ReadLine().ToLower().Trim();
            } while (enteredText == null);

            MoveDirection targetDirection = new MoveDirection(0, 0);

            if (map.ReturnCurrentRoom(player.GetPlayerLocation()).GetType() == typeof(RoomFountain))
            {
                if (!isFountainEnabled && enteredText == "enable")
                {
                    WriteColourText("The fountain has been activated and water starts pouring over the marble.", ConsoleColor.Blue);
                    isFountainEnabled = true;
                    Console.WriteLine();
                }
                else
                {
                    targetDirection = CreateMoveDirection(enteredText);
                }
            }
            else
            {
                targetDirection = CreateMoveDirection(enteredText);
            }

            if (IsRequestedMoveLegal(player, targetDirection, map))
            {
                player.SetRelativePlayerLocation(targetDirection.Row, targetDirection.Column);
                Console.WriteLine("Move successful.");
            }

            Console.WriteLine("---------------------------------------------------------------------------");

            if (map.ReturnCurrentRoom(player.GetPlayerLocation()) is RoomEntrance) isPlayerAtExit = true;
            //if (map.ReturnCurrentRoom(player.GetPlayerLocation()).GetType() == typeof(RoomEntrance)) isPlayerAtExit = true;
            else isPlayerAtExit = false;

            if (isFountainEnabled && isPlayerAtExit)
            {
                Console.WriteLine("You win");
                return;
            }
        }
    }

    private static MoveDirection CreateMoveDirection(string enteredText)
    {
        return enteredText switch
        {
            "move north" => new MoveDirection(-1, 0),
            "move south" => new MoveDirection(1, 0),
            "move east" => new MoveDirection(0, 1),
            "move west" => new MoveDirection(0, -1),
            "" => new MoveDirection(0, 0),
        };
    }

    public bool IsRequestedMoveLegal(Player player, MoveDirection targetPlayerLocation, MapManager map)
    {
        if (targetPlayerLocation.Row == 0 && targetPlayerLocation.Column == 0 && map.ReturnCurrentRoom(player.GetPlayerLocation()).GetType() != typeof(RoomFountain))
        {
            WriteColourText("You need to pick a direction.", ConsoleColor.Red);
            Console.WriteLine();
            return false;
        }
        // cache player's original location
        PlayerLocation originalLocation = player.GetPlayerLocation();
        player.SetRelativePlayerLocation(targetPlayerLocation.Row, targetPlayerLocation.Column);
        PlayerLocation newLocationToTest = player.GetPlayerLocation();

        if (newLocationToTest.Column < 0 ||
            newLocationToTest.Column > map.ColQty ||
            newLocationToTest.Row < 0 ||
            newLocationToTest.Row > map.RowQty)
        {
            WriteColourText("Move invalid; you can't move out of bounds.", ConsoleColor.Red);
            // reset player to original position
            player.SetAbsolutePlayerLocation(originalLocation.Row, originalLocation.Column);
            Console.WriteLine();
            return false;
        }
        else
        {
            // reset player's location so that move can be applied outside of this method
            player.SetAbsolutePlayerLocation(originalLocation.Row, originalLocation.Column);
            return true;
        }
    }

    public void DisplayRoomDescription(Player player, MapManager map)
    {
        string roomDescription = map.Rooms[player.Row, player.Column].RoomDescription;
        WriteColourText(roomDescription, ConsoleColor.Green);
        Console.WriteLine();
    }

    public void DisplayPlayerLocString(Player player)
    {
        Console.Write($"The player is at [");
        WriteColourText($"{player.GetPlayerLocation().Row}", ConsoleColor.DarkMagenta);
        Console.Write(", ");
        WriteColourText($"{player.GetPlayerLocation().Column}", ConsoleColor.DarkMagenta);
        Console.WriteLine("]. ");
    }

    public void WriteColourText(string text, ConsoleColor colour)
    {
        Console.ForegroundColor = colour;
        Console.Write(text);
        Console.ForegroundColor = ConsoleColor.White;
    }

    public struct MoveDirection(int row, int column)
    {
        public int Row { get; } = row;
        public int Column { get; } = column;
    }
}