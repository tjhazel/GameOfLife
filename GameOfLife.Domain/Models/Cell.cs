namespace GameOfLife.Domain.Models;

public struct Cell(int x, int y, bool isAlive)
{
   public readonly int X { get; } = x;
   public readonly int Y { get; } = y;
   public bool IsAlive { get; set; } = isAlive;
   public List<bool> Neighbors { get; } = [];
}
