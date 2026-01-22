public class Weapon
{
    public Weapon(string name, int startingAmmunition)
    {
        WeaponName = name;
        AmmunitionCount = startingAmmunition;
    }

    public string? WeaponName { get; private set; }
    public int AmmunitionCount { get; set; }

    public void DisplayAmmoRemaining()
    {
        System.Console.WriteLine($"You have {AmmunitionCount} ammo remaining. ");
    }
}