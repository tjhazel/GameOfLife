namespace GameOfLife.Domain.Models;

public class BoardState
{
   public required string GameId { get; set; }
   public int Tick { get; set; } = 0;

   public required bool[,] Grid { get; set; }
   public int TotalCellCount => Grid?.GetLength(0) * Grid?.GetLength(1) ?? 0;
   public int StartActiveCellCount { get; set; } = -1;
   public int? FinishActiveCellCount { get; set; }

   public BoardState Clone()
   {
      BoardState clone = new()
      {
         GameId = GameId,
         Tick = Tick,
         Grid = (bool[,])Grid.Clone(),
         StartActiveCellCount = StartActiveCellCount
      };
      return clone;
   }
}
