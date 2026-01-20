public class Room
{
    public int Row { get; init; }
    public int Column { get; init; }
    
    // You can't make fields virtual - must be a property instead.
    public virtual string? RoomDescription { get; init; } = "You're in a non-descript, empty part of the cave. ";
    public virtual string? RoomSense { get; init; } = "you sense nothing. ";

    // A room needs to be able to contain (onlt ever one) monster
    public Monster? monster;
}