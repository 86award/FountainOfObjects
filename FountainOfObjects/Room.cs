public class Room
{
    public int Row { get; init; }
    public int Column { get; init; }
    
    // You can't make fields virtual
    public virtual string? RoomDescription { get; private set; }
}