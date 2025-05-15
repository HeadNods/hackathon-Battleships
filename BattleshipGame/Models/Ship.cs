namespace BattleshipGame.Models;

public class Ship
{
    public string Name { get; }
    public int Length { get; }
    public bool IsPlaced { get; set; }
    public bool IsHorizontal { get; set; }
    public (int Row, int Col)[] Coordinates { get; set; }

    public Ship(string name, int length)
    {
        Name = name;
        Length = length;
        IsPlaced = false;
        IsHorizontal = true;
        Coordinates = new (int Row, int Col)[length];
    }

    public void Rotate()
    {
        IsHorizontal = !IsHorizontal;
    }
}
