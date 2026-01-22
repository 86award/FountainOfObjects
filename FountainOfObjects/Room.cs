public class Room
{
    public int Row { get; init; }
    public int Column { get; init; }
    public virtual bool IsInteractable { get; init; } = false;
    public Monster? Monster { get; private set; }
    // You can't make fields virtual - must be a property instead.
    public virtual string? RoomDescription { get; init; } = "You're in a non-descript, empty part of the cave. \n";
    public virtual string? RoomSense { get; init; } = "you sense nothing. ";

    // A room needs to be able to contain (only ever one) monster
    public void AssignMonsterToRoom(Monster monster)
    {
        if (Monster == null) Monster = monster;
        else Console.WriteLine("ERROR: monster already exists in this room. ");
    }

    public void KillMonsterInRoom()
    {
        Monster = null;
    }
}