public class GameManager
{
    // should I be using a struct to pass around co-ords instead of tuple
    public bool IsMoveLegal(PlayerLocation playerLocation)
    {
        // logic to determine if player can make this move
        // get player's co-ords
        // check if they're trying to move out of bounds 
        if (playerLocation.Column < 0 ||
            playerLocation.Row < 0)
        {
            return false;
        }
        else return true;
    }
}