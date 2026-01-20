public class MonsterMaelstrom : Monster
{
    public override string? MonsterSenseDescription { get; protected set; } = "you hear the growling and groaning of a maelstrom. ";

    public MonsterMaelstrom(int row, int column, string monsterName) : base(row, column, monsterName)
    {
        // I'm guessing this just works by applying the MM inputs to the base properties
    }
}