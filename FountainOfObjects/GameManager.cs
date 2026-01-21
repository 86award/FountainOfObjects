public class GameManager
{
    private bool gameActive = true;
    private bool isFountainEnabled;
    private bool isPlayerAtExit;
    private string? playerInputActionText;
    private ActionType playerActionType;

    public void InitialiseGame()
    {
        // CREATE THE MAP
        MapManager? map = null;
        SetMapSize(ref map); // REMINDER: look-up ref keyword usage

        // INIT PLAYER
        Player player = new Player(0, 0); // REMINDER: player is currenly hard coded to 0,0

        // INIT MONSTERS
        MonsterMaelstrom maelstrom = new MonsterMaelstrom(3, 2, "Maelstrom"); // REMINDER: need to look for a data driven way to spawn monsters and traps
        if (map != null) map.Rooms[maelstrom.Row, maelstrom.Column].AssignMonsterToRoom(maelstrom);

        // INTRO GAME
        DisplayIntroText();

        // MAIN GAME LOOP
        while (gameActive && map != null)
        {
            // DISPLAY GAME STATE
            DisplayPlayerLocString(player);
            DisplayRoomDescription(player, map);
            map.DisplayAdjacentRoomDescriptions(player.GetPlayerLocation());

            // GET PLAYER INPUT ACTION
            playerInputActionText = GetPlayerInput();
            if (!IsInputActionTextValid(playerInputActionText))
            {
                WriteColourText("Please enter a valid action, in full e.g. 'verb + direction' \n", ConsoleColor.Red);
                DrawLineBreak();
                continue;
            }
            playerActionType = AssignActionType(playerInputActionText);

            // HANDLE INPUT ACTION
            switch (playerActionType)
            {
                case ActionType.Move:
                    MoveDirection targetDirection = CreateMoveDirection(playerInputActionText);
                    if (IsRequestedMoveLegal(player, targetDirection, map))
                    {
                        player.SetRelativePlayerLocation(targetDirection.Row, targetDirection.Column);
                    }
                    break;
                case ActionType.Shoot:
                    break;
                case ActionType.Interact:
                    Room playerRoomCheckForInteract = map.ReturnCurrentRoom(player.GetPlayerLocation());
                    if (playerRoomCheckForInteract.IsInteractable == true)
                    {
                        if (playerRoomCheckForInteract.GetType() == typeof(RoomFountain))
                        {
                            if (!isFountainEnabled)
                            {
                                WriteColourText("You activate the fountain and water starts pouring over the marble. \n", ConsoleColor.Blue);
                                isFountainEnabled = true;
                            }
                            else WriteColourText("The fountain is already active. \n", ConsoleColor.Blue);
                        }
                        else WriteColourText("You search around and find nothing. \n", ConsoleColor.Blue);
                    }
                    else WriteColourText("There's nothing to interact with in this room. \n", ConsoleColor.Red);
                    break;
            }

            // UPDATE PLAYER STATE
            Room currentPlayerRoom = map.ReturnCurrentRoom(player.GetPlayerLocation());
            if (currentPlayerRoom is RoomPit)
            {
                WriteColourText($"{currentPlayerRoom.RoomDescription}You died! \\n", ConsoleColor.Red);
                gameActive = false;
                return;
            }
            if (currentPlayerRoom.Monster != null)
            {
                Monster _monster = currentPlayerRoom.Monster;
                if (_monster.GetType() == typeof(MonsterMaelstrom))
                {
                    WriteColourText("You walk into the room and see a swirling maelstrom. \n", ConsoleColor.Red);
                    WriteColourText("As if sentient, the maelstrom appears to notice your arrival and moves towards you. You black out. \n", ConsoleColor.Red);
                    WriteColourText("You come to and reaslise you're in a different room. \n", ConsoleColor.Red);
                    ((MonsterMaelstrom)_monster).MaelstromPushback(player, map);
                }
            }
            if (currentPlayerRoom is RoomEntrance) isPlayerAtExit = true;
            else isPlayerAtExit = false;
            
            // CHECK FOR WIN CONDITION
            if (isFountainEnabled && isPlayerAtExit)
            {
                WriteColourText("You win! ", ConsoleColor.Blue);
                return;
            }

            DrawLineBreak();
        }

        static void DisplayIntroText()
        {
            Console.WriteLine();
            Console.WriteLine("+----------------------------------------------------------------------------------+");
            Console.WriteLine("| You enter the Cavern of Objects, a maze of rooms filled with pits, and other     |");
            Console.WriteLine("| foul dangers, in search of the lost Fountain of Objects.                         |");
            Console.WriteLine("| The only light comes from the entrance; no other light is seen anywhere in the   |");
            Console.WriteLine("| caverns and you sense magic is the cause of the darkness.                        |");
            Console.WriteLine("| You must navigate the Caverns with your senses alone.                            |");
            Console.WriteLine("| Find the Fountain of Objects, activate it, and return to the entrance.           |");
            Console.WriteLine("+----------------------------------------------------------------------------------+\n");
        }
        static string GetPlayerInput()
        {
            string? enteredText;
            do
            {
                Console.Write("Enter a valid action e.g., ");
                WriteColourText($"move north/south/east/west, interact or attack", ConsoleColor.Yellow);
                Console.Write(": ");
                enteredText = Console.ReadLine()?.ToLower().Trim();
            } while (enteredText == null);
            return enteredText;
        }
    }

    public static void DrawLineBreak()
    {
        Console.WriteLine("------------------------------------------------------------------------------------\n");
    }
    public static void WriteColourText(string text, ConsoleColor colour)
    {
        Console.ForegroundColor = colour;
        Console.Write(text);
        Console.ForegroundColor = ConsoleColor.White;
    }
    public void DisplayPlayerLocString(Player player)
    {
        Console.Write($"Your position is [");
        WriteColourText($"{player.GetPlayerLocation().Row}", ConsoleColor.DarkMagenta);
        Console.Write(", ");
        WriteColourText($"{player.GetPlayerLocation().Column}", ConsoleColor.DarkMagenta);
        Console.Write("]. ");
    }
    public void DisplayRoomDescription(Player player, MapManager map)
    {
        string? roomDescription = map.Rooms[player.Row, player.Column].RoomDescription;
        if (roomDescription != null) WriteColourText(roomDescription, ConsoleColor.Green);
    }
    public bool IsRequestedMoveLegal(Player player, MoveDirection targetPlayerLocation, MapManager map)
    {
        // cache player's original location
        PlayerLocation originalLocation = player.GetPlayerLocation();
        player.SetRelativePlayerLocation(targetPlayerLocation.Row, targetPlayerLocation.Column);
        PlayerLocation newLocationToTest = player.GetPlayerLocation();

        if (newLocationToTest.Column < 0 ||
            newLocationToTest.Column >= map.ColQty ||
            newLocationToTest.Row < 0 ||
            newLocationToTest.Row >= map.RowQty)
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
    public bool IsInputActionTextValid(string inputActionText) // CAN I OUT THE STRING ITSELF
    {
        if (inputActionText == "move north" || inputActionText == "move south" || inputActionText == "move east" || inputActionText == "move west" ||
        inputActionText == "shoot north" || inputActionText == "shoot south" || inputActionText == "shoot east" || inputActionText == "shoot west" ||
        inputActionText == "interact" || inputActionText == "enable" || inputActionText == "activate")
            return true;
        else return false;
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
    private ActionType AssignActionType(string inputString)
    {
        if (inputString.Contains("move")) return ActionType.Move;
        else if (inputString.Contains("shoot")) return ActionType.Shoot;
        else return ActionType.Interact;
    }
    private MoveDirection CreateMoveDirection(string enteredText)
    {
        return enteredText switch
        {
            "move north" => new MoveDirection(-1, 0),
            "move south" => new MoveDirection(1, 0),
            "move east" => new MoveDirection(0, 1),
            "move west" => new MoveDirection(0, -1),
            _ => new MoveDirection(0, 0),
        };
    }

    public struct MoveDirection(int row, int column)
    {
        public int Row { get; } = row;
        public int Column { get; } = column;
    }
}