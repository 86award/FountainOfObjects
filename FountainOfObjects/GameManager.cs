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
        MonsterAmarok amarok = new MonsterAmarok(1, 3, "Amarok");
        if (map != null)
        {
            map.Rooms[maelstrom.Row, maelstrom.Column].AssignMonsterToRoom(maelstrom);
            map.Rooms[amarok.Row, amarok.Column].AssignMonsterToRoom(amarok);
        }

        // INTRO GAME
        DisplayIntroText();
        DateTime gameStartTime = DateTime.Now;

        // MAIN GAME LOOP
        while (gameActive && map != null)
        {
            // DISPLAY GAME STATE
            DisplayPlayerLocString(player);
            DisplayRoomDescription(player, map);
            player.Weapon.DisplayAmmoRemaining();
            map.DisplayAdjacentRoomDescriptions(player.GetPlayerLocation());

            // GET PLAYER INPUT ACTION
            playerInputActionText = GetPlayerInput();
            if (!IsInputActionTextValid(playerInputActionText))
            {
                if (playerInputActionText == "quit") continue;
                WriteColourText("Please enter a valid action, in full e.g. 'verb + direction' \n", ConsoleColor.Red);
                DrawLineBreak();
                continue;
            }
            playerActionType = AssignActionType(playerInputActionText);

            // HANDLE INPUT ACTION
            switch (playerActionType)
            {
                case ActionType.Help:
                    DisplayHelpText();
                    break;
                case ActionType.Move:
                    MoveDirection targetDirection = CreateMoveDirection(playerInputActionText);
                    if (IsRequestedMoveLegal(player, targetDirection, map))
                    {
                        player.SetRelativePlayerLocation(targetDirection.Row, targetDirection.Column);
                    }
                    break;
                case ActionType.Shoot:
                    ShootDirection shootDirection = player.CreateShootDirection(playerInputActionText);
                    if (IsRequestedShotLegal(player, shootDirection, map))
                    {
                        if (player.Weapon.AmmunitionCount > 0)
                        {
                            player.Weapon.AmmunitionCount--;
                            // report shooting in direction
                            ReportHitResult(player, shootDirection, map);
                        }
                        else
                        {
                            WriteColourText("You don't have any ammo left. \n", ConsoleColor.Red);
                            break;
                        }
                    }

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
                else if (_monster.GetType() == typeof(MonsterAmarok))
                {
                    WriteColourText("As you enter, you smell the thing before you see it. \n", ConsoleColor.Red);
                    WriteColourText("Oozing flesh and a maw full of teeth, it leaps at you. \n", ConsoleColor.Red);
                    WriteColourText("You died! \n", ConsoleColor.Red);
                    gameActive = false;
                    return;
                }
            }
            if (currentPlayerRoom is RoomEntrance) isPlayerAtExit = true;
            else isPlayerAtExit = false;

            // CHECK FOR WIN CONDITION
            if (isFountainEnabled && isPlayerAtExit)
            {
                WriteColourText("You win! \n", ConsoleColor.Blue);
                DateTime gameEndTime = DateTime.Now;
                TimeSpan playingTime = gameEndTime - gameStartTime;
                Console.WriteLine($"You were playing for {playingTime.TotalSeconds} seconds. ");
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
        void DisplayHelpText()
        {
            Console.WriteLine();
            Console.WriteLine("+----------------------------------------------------------------------------------+");
            Console.WriteLine("|                                     HELP                                         |");
            Console.WriteLine("| When prompted, you need to type one of the following commands:                   |");
            Console.WriteLine("| * Move North / Move South / Move East / Move West - move one room in direction.  |");
            Console.WriteLine("| * Interact / Enable / Activate - if room has interactable element, trigger it.   |");
            Console.WriteLine("| * Shoot North / Shoot South / Shoot East / Shoot West - shoot into room in that  |");
            Console.WriteLine("|   direction. Consumes one arrow and will kill and monsters in target room.       |");
            Console.WriteLine("| * Quit - terminates the application.                                             |");
            Console.WriteLine("+----------------------------------------------------------------------------------+\n");
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
            WriteColourText("Move invalid; you can't move out of bounds. \n", ConsoleColor.Red);
            // reset player to original position
            player.SetAbsolutePlayerLocation(originalLocation.Row, originalLocation.Column);
            return false;
        }
        else
        {
            // reset player's location so that move can be applied outside of this method
            player.SetAbsolutePlayerLocation(originalLocation.Row, originalLocation.Column);
            return true;
        }
    }
    public bool IsRequestedShotLegal(Player player, ShootDirection targetShootLocation, MapManager map)
    {
        PlayerLocation playerLocation = player.GetPlayerLocation();
        if (playerLocation.Row + targetShootLocation.Row < 0 ||
            playerLocation.Row + targetShootLocation.Row >= map.RowQty ||
            playerLocation.Row + targetShootLocation.Column < 0 ||
            playerLocation.Row + targetShootLocation.Row >= map.ColQty)
        {
            WriteColourText("You're trying to shoot out of bounds. \n", ConsoleColor.Red);
            return false;
        }
        else return true;
    }
    public bool IsInputActionTextValid(string inputActionText) // CAN I OUT THE STRING ITSELF
    {
        if (inputActionText == "move north" || inputActionText == "move south" || inputActionText == "move east" || inputActionText == "move west" ||
        inputActionText == "shoot north" || inputActionText == "shoot south" || inputActionText == "shoot east" || inputActionText == "shoot west" ||
        inputActionText == "interact" || inputActionText == "enable" || inputActionText == "activate" ||
        inputActionText == "help")
            return true;
        else if (inputActionText == "quit")
        {
            gameActive = false;
            return false;
        }
        else return false;
    }
    public void ReportHitResult(Player player, ShootDirection targetShootLocation, MapManager map)
    {
        // get the room at the location of the shot relative to the player
        int absTargetRow = player.Row + targetShootLocation.Row;
        int absTargetColumn = player.Column + targetShootLocation.Column;
        // check if there's a monster in the room
        Monster? targetMonster = map.Rooms[absTargetRow, absTargetColumn].Monster;
        if (targetMonster != null)
        {
            string report = $"You shoot at the {targetMonster.Name}, hearing a piercing cry as it falls to the ground, dead. \n";
            WriteColourText(report, ConsoleColor.Red);
            map.Rooms[absTargetRow, absTargetColumn].KillMonsterInRoom();
        }
        else
        {
            WriteColourText("You shoot into the empty void and hit nothing, losing your arrow. ", ConsoleColor.Red);
        }
        // kill monster if applicable
        // report back result
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
        else if (inputString == "help") return ActionType.Help;
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