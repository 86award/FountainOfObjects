public class Monster
{
    public virtual string? MonsterSenseDescription {get; protected set; }
    public virtual int Row { get; private set; }
    public virtual int Column { get; private set; }
    public virtual string? Name {get; init; }

    public Monster(int row, int column, string monsterName)
    {
        Row = row;
        Column = column;
        Name = monsterName;
    }

    public void MoveMonster(int newRow, int newColumn)
    {
        Row = newRow;
        Column = newColumn;
    }
}