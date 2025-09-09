namespace GameOfLife.Domain.Models;

/// <summary>
/// Props that represent a clean board
/// </summary>
public class BoardStateRequest
{
   public required string GameId { get; set; }
   public string[] Pattern { get; set; } = [];
   public int? Tick { get; set; }
   public int? StartActiveCellCount { get; set; }
   public int? FinishActiveCellCount { get; set; }
}