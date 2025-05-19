namespace BattleshipGame.Models;    
public class Ship
    {
        public string Name { get; }
        public int Length { get; }
        public bool IsPlaced { get; set; }
        public bool IsHorizontal { get; set; }
        public (int Row, int Col)[] Coordinates { get; set; }
        public bool[] Hits { get; private set; }
        public bool IsSunk => Hits.All(hit => hit);

        public Ship(string name, int length)
        {
            Name = name;
            Length = length;
            IsPlaced = false;
            IsHorizontal = true;
            Coordinates = new (int Row, int Col)[length];
            Hits = new bool[length];
        }

        public bool TryHit(int row, int col)
        {
            for (int i = 0; i < Coordinates.Length; i++)
            {
                if (Coordinates[i].Row == row && Coordinates[i].Col == col)
                {
                    Hits[i] = true;
                    return true;
                }
            }
            return false;
        }

    public void Rotate()
    {
        IsHorizontal = !IsHorizontal;
    }
}
