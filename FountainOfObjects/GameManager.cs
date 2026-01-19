public class GameManager
{
    private bool gameActive = true;
    private bool isFountainEnabled = false;
    private bool isPlayerAtExit;

    public void InitialiseGame()
    {
        MapManager map = new MapManager(4, 4);
        Player player = new Player(0, 0);
        Console.WriteLine("---------------------------------------------------------------------------");
        Console.WriteLine("| You find yourself at the mouth of a dark cave system. You take a deep   |");
        Console.WriteLine("| breath and enter.                                                       |");
        Console.WriteLine("---------------------------------------------------------------------------");

        while (gameActive)
        {
            Console.WriteLine();
            DisplayPlayerLocString(player);
            // would be better to take the room description here and colour-code text for player options
            // get room co-ords and then description based on player location
            DisplayRoomDescription(player, map);

            string? playerAction;
            do
            {
                // it would be nice if the room that the player was in determined valid moves - add later
                Console.Write("Please enter a valid command e.g., ");
                WriteColourText("move north/south/east/west", ConsoleColor.Yellow);
                Console.Write(": ");
                playerAction = Console.ReadLine();
                // check for legal move here
            } while (playerAction == null);

            // this is quite limited an feels like a good place to use a generic so various actions can be handled
            // in fact, I should make a generic Action that can take a move, enable, search etc.
            MoveDirection newLocation = playerAction switch
            {
                "move north" => new MoveDirection(-1, 0),
                "move south" => new MoveDirection(1, 0),
                "move east" => new MoveDirection(0, -1),
                "move west" => new MoveDirection(0, 1),
                "" => new MoveDirection(0, 0),
            };

            if (IsRequestedMoveLegal(player, newLocation))
            {
                Console.WriteLine("Move successful.");
                // write out the room's description here
            }
            Console.WriteLine("---------------------------------------------------------------------------");
        }
    }

    // should I be using a struct to pass around co-ords instead of tuple
    public bool IsRequestedMoveLegal(Player player, MoveDirection newPlayerLocation)
    {
        // cache player's current location
        // get player's co-ords
        PlayerLocation originalLocation = player.GetPlayerLocation(); // this won't work becuase you'd be adding 0,0 i.e. no change
        // update player location
        player.SetRelativePlayerLocation(newPlayerLocation.Row, newPlayerLocation.Column);
        // logic to determine if player can make this move
        // if it's legal make the move
        PlayerLocation newLocationToTest = player.GetPlayerLocation();
        // if not, make no move   
        // check if they're trying to move out of bounds 
        if (newPlayerLocation.Row == 0 && newPlayerLocation.Column == 0)
        {
            WriteColourText("You need to move in a direction.", ConsoleColor.Red);
            Console.WriteLine();
            return false;
        }
        if (newLocationToTest.Column < 0 || // I want to get lower and upper bound here intead of magic number
            newLocationToTest.Row < 0)
        {
            WriteColourText("Move invalid; you can't move out of bounds.", ConsoleColor.Red);
            // reset player to original position
            player.SetAbsolutePlayerLocation(originalLocation.Row, originalLocation.Column);
            Console.WriteLine();
            return false;
        }
        else return true;
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