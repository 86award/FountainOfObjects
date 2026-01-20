public class GameManager
{
    private bool gameActive = true;
    private bool isFountainEnabled;
    private bool isPlayerAtExit;

    public void InitialiseGame()
    {
        // CREATE THE MAP
        MapManager? map = null;
        SetMapSize(ref map); // REMINDER: look-up ref keyword usage

        // INIT PLAYER
        Player player = new Player(0, 0); // REMINDER: player is currenly hard coded to 0,0

        // INIT MONSTERS
        MonsterMaelstrom maelstrom = new MonsterMaelstrom(3, 2, "Maelstrom"); // REMINDER: need to look for a data driven way to spawn monsters and traps
        if (map != null) map.Rooms[maelstrom.Row, maelstrom.Column].monster = maelstrom;

        // COMMENCE GAME
        DisplayIntroText();

        // MAIN GAME LOOP
        while (gameActive && map != null)
        {
            string? playerInputAction;
            Room currentPlayerRoom = map.ReturnCurrentRoom(player.GetPlayerLocation());

            // DISPLAY GAME STATE
            DisplayPlayerLocString(player);
            DisplayRoomDescription(player, map);
            map.DisplayAdjacentRoomDescriptions(player.GetPlayerLocation());

            // GET PLAYER INPUT ACTION
            playerInputAction = GetPlayerInput();
            
            MoveDirection targetDirection = new MoveDirection(0, 0);

            // PLAYER LOCATION CHECKS
            if (currentPlayerRoom.GetType() == typeof(RoomFountain))
            {
                if (!isFountainEnabled && playerInputAction == "enable")
                {
                    WriteColourText("You activate the fountain and water starts pouring over the marble. ", ConsoleColor.Blue);
                    isFountainEnabled = true;
                    Console.WriteLine();
                }
                else
                {
                    targetDirection = CreateMoveDirection(playerInputAction);
                }
            }
            else targetDirection = CreateMoveDirection(playerInputAction);
            if (IsRequestedMoveLegal(player, targetDirection, map))
            {
                player.SetRelativePlayerLocation(targetDirection.Row, targetDirection.Column);
                // Console.WriteLine("Move successful.");
            }
            if (currentPlayerRoom is RoomEntrance) isPlayerAtExit = true;
            //if (map.ReturnCurrentRoom(player.GetPlayerLocation()).GetType() == typeof(RoomEntrance)) isPlayerAtExit = true;
            else isPlayerAtExit = false;

            if (currentPlayerRoom is RoomPit)
            {
                Console.WriteLine();
                WriteColourText($"{map.ReturnCurrentRoom(player.GetPlayerLocation()).RoomDescription} You died!", ConsoleColor.Red);
                gameActive = false;
            }

            if (currentPlayerRoom.monster != null)
            {
                Monster _monster = map.ReturnCurrentRoom(player.GetPlayerLocation()).monster;
                if (_monster.GetType() == typeof(MonsterMaelstrom))
                {
                    // move player [-1, +2]
                    // if a row or column would be <0 or >max, set to 0 or max respectively
                    // though I think going back to the entrance is cool
                    WriteColourText("You've been blown back to the entrance by the maelstrom. ", ConsoleColor.Red);
                    player.SetAbsolutePlayerLocation(0, 0);
                    Console.WriteLine();
                }
            }
            Console.WriteLine("---------------------------------------------------------------------------");

            // CHECK FOR WIN CONDITION
            if (isFountainEnabled && isPlayerAtExit)
            {
                Console.WriteLine("You win! ");
                return;
            }
        }

        static void DisplayIntroText()
        {
            Console.WriteLine();
            Console.WriteLine("+---------------------------------------------------------------------------+");
            Console.WriteLine("| You enter the Cavern of Objects, a maze of rooms filled with pits, and    |");
            Console.WriteLine("| other foul dangers, in search of the lost Fountain of Objects.            |");
            Console.WriteLine("| The only light comes from the entrance; no other light is seen anywhere   |");
            Console.WriteLine("| in the caverns and you sense magic is the cause of the darkness.          |");
            Console.WriteLine("| You must navigate the Caverns with your senses alone.                     |");
            Console.WriteLine("| Find the Fountain of Objects, activate it, and return to the entrance.    |");
            Console.WriteLine("+---------------------------------------------------------------------------+\n");
        }
        static string GetPlayerInput()
        {
            string? enteredText;
            do
            {
                Console.Write("Please enter a valid action e.g., ");
                WriteColourText("move north/south/east/west", ConsoleColor.Yellow);
                Console.Write(": ");
                enteredText = Console.ReadLine()?.ToLower().Trim();
            } while (enteredText == null);
            return enteredText;
        }
    }

    public static void WriteColourText(string text, ConsoleColor colour)
    {
        Console.ForegroundColor = colour;
        Console.Write(text);
        Console.ForegroundColor = ConsoleColor.White;
    }
    public void DisplayPlayerLocString(Player player)
    {
        Console.Write($"The player is at [");
        WriteColourText($"{player.GetPlayerLocation().Row}", ConsoleColor.DarkMagenta);
        Console.Write(", ");
        WriteColourText($"{player.GetPlayerLocation().Column}", ConsoleColor.DarkMagenta);
        Console.WriteLine("]. ");
    }
    public void DisplayRoomDescription(Player player, MapManager map)
    {
        string? roomDescription = map.Rooms[player.Row, player.Column].RoomDescription;
        if (roomDescription != null) WriteColourText(roomDescription, ConsoleColor.Green);
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

    private static void SetMapSize(ref MapManager? map)
    {
        bool mapSizeSet = false;
        do
        {
            Console.Write($"Please choose a map size to determine the level of difficulty (small, medium, large): ");
            string mapSize = Console.ReadLine() ?? "small".ToLower().Trim();
            // I want to use an enum to check validity of the map size from a limited range
            if (Enum.TryParse<MapSizes>(mapSize, true, out MapSizes result)) // REMINDER: review of the out keyword.
            {
                map = MapManager.CreateMap(result); // Because ref keyword used, reference type map (line 10) is assigned
                mapSizeSet = true;
            }
            else
            {
                Console.WriteLine("Invalid map size.");
            }
        } while (mapSizeSet == false);
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

    public struct MoveDirection(int row, int column)
    {
        public int Row { get; } = row;
        public int Column { get; } = column;
    }
}