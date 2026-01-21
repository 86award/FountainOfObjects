public class MonsterMaelstrom : Monster
{
    public override string? MonsterSense { get; init; } = "you hear the growling and groaning of a maelstrom. ";

    public MonsterMaelstrom(int row, int column, string monsterName) : base(row, column, monsterName)
    {
        // I'm guessing this just works by applying the MM inputs to the base properties
    }

    public void MaelstromPushback(Player player, MapManager map)
    {
        int rowPushback = 1;
        int colPushback = -2;

        // Clamp row: ensure we don't go below 0
        while (player.GetPlayerLocation().Row + rowPushback < 0)
        {
            rowPushback++;
        }
        // Clamp row: ensure we don't go past the map boundary
        while (player.GetPlayerLocation().Row + rowPushback >= map.RowQty)
        {
            rowPushback--;
        }
        
        // Clamp column: ensure we don't go below 0
        while (player.GetPlayerLocation().Column + colPushback < 0)
        {
            colPushback++;
        }
        // Clamp column: ensure we don't go past the map boundary
        while (player.GetPlayerLocation().Column + colPushback >= map.ColQty)
        {
            colPushback--;
        }
        
        player.SetRelativePlayerLocation(rowPushback, colPushback);
    }
}