internal class Program
{
    private static void Main(string[] args)
    {
        MapManager map = new MapManager(4, 4);
        Console.WriteLine(map.Rooms[0, 2].ToString());
    }
}