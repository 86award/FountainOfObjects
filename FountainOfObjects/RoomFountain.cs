public class RoomFountain : Room
{
    public override bool IsInteractable { get; init; } = true;
    public override string? RoomDescription { get; init; } = "You hear water dripping in this room. The Fountain of Objects is here! \n";
    public override string? RoomSense { get; init; } = "you hear the faint drip, drip of water. ";
}