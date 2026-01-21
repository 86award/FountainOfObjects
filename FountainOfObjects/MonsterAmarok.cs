public class MonsterAmarok : Monster
{
    public override string? MonsterSense { get; init; } = "you can smell the rotten stench of an amarok. ";
    public MonsterAmarok(int row, int column, string monsterName): base(row, column, monsterName)
    {
        // Interesting that row doesn't need to be set and I guess is just inherited from base?
        // Row = row;

    }
}