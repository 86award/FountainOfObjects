internal class Program
{
    private static void Main(string[] args)
    {
        MapManager map = new MapManager(4, 4);
        System.Console.WriteLine(map.Rooms[1,1].ToString());
    }
}