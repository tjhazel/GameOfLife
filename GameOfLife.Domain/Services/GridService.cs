using GameOfLife.Domain.Extensions;
using GameOfLife.Domain.Models;
using Microsoft.Extensions.Logging;

namespace GameOfLife.Domain.Services;

public class GridService(ILogger<GridService> logger,
   IBoardStateService boardStateService) : IGridService
{
   private readonly ILogger<GridService> _logger = logger;
   private readonly IBoardStateService _boardStateService = boardStateService;

   public async Task<BoardState> GetNextBoardState(BoardState boardState)
   {
      BoardState nextBoardState = boardState.Clone();

      //be a great use of a producer \ consumer pattern here
      Cell[] cellsToProcess = new Cell[boardState.TotalCellCount];

      int currentIdx = -1;
      int colCount = boardState.Grid.GetLength(0);
      int rowCount = boardState.Grid.GetLength(1);
      //used to keep track of active count for after tick is finsished
      int activeCellCount = 0;

      //iterate grid to build the cell array with neighbors
      for (int x = 0; x < colCount; x++)
      {
         for (int y = 0; y < rowCount; y++)
         {
            var cell = GetNeighbors(x, y, rowCount, colCount, boardState);
            cellsToProcess[++currentIdx] = cell;
         }
      }

      await Parallel.ForEachAsync(cellsToProcess, 
         new ParallelOptions { 
            MaxDegreeOfParallelism = Environment.ProcessorCount 
         }, async (cell, cancellationToken) =>
      {
         var livingNeighbors = cell.Neighbors.Count(y => y == true);

         //Any live cell with fewer than two live neighbours dies, as if by underpopulation.
         if (livingNeighbors < 2)
         {
            nextBoardState.Grid[cell.X, cell.Y] = false;
         }
         //Any live cell with two or three live neighbours lives on to the next generation.
         else if (cell.IsAlive && livingNeighbors > 1 && livingNeighbors < 3)
         {
            nextBoardState.Grid[cell.X, cell.Y] = false;
         }
         //Any live cell with more than three live neighbours dies, as if by overpopulation.
         else if (cell.IsAlive && livingNeighbors > 3)
         {
            nextBoardState.Grid[cell.X, cell.Y] = false;
         }
         //Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
         else if (!cell.IsAlive && livingNeighbors == 3)
         {
            nextBoardState.Grid[cell.X, cell.Y] = true;
            Interlocked.Increment(ref activeCellCount);
         }
         else if (nextBoardState.Grid[cell.X, cell.Y] == true)
         {
            Interlocked.Increment(ref activeCellCount);
         }
      });

      //set finished active cell count
      nextBoardState.FinishActiveCellCount = activeCellCount;

      //increment the Tick
      nextBoardState.Tick++;

      //this method will create and store a new grid
      return nextBoardState;
   }

   //https://swharden.com/csdv/simulations/life/
   Cell GetNeighbors(int x, int y, int rowCount, int colCount, BoardState boardState)
   {
      var cell = new Cell(x, y, boardState.Grid[x, y]);

      // left & right --> out of bounds is -1
      int xLeftIndex = cell.X > 0 ? cell.X - 1 : -1;
      int xRightIndex = cell.X < colCount - 1 ? cell.X + 1 : -1;

      // left & right --> out of bounds is -1
      int yTopIndex = cell.Y > 0 ? cell.Y - 1 : -1;
      int yBottomIndex = cell.Y < rowCount - 1 ? cell.Y + 1 : -1;

      try
      {
         if (xLeftIndex > -1 && yTopIndex > -1)
            cell.Neighbors.Add(boardState.Grid[xLeftIndex, yTopIndex]);
         if (yTopIndex > -1)
            cell.Neighbors.Add(boardState.Grid[cell.X, yTopIndex]);
         if (xRightIndex > -1 && yTopIndex > -1)
            cell.Neighbors.Add(boardState.Grid[xRightIndex, yTopIndex]);

         if (xLeftIndex > -1)
            cell.Neighbors.Add(boardState.Grid[xLeftIndex, cell.Y]);
         if (xRightIndex > -1)
            cell.Neighbors.Add(boardState.Grid[xRightIndex, cell.Y]);

         if (xLeftIndex > -1 && yBottomIndex > -1)
            cell.Neighbors.Add(boardState.Grid[xLeftIndex, yBottomIndex]);
         if (yBottomIndex > -1)
            cell.Neighbors.Add(boardState.Grid[cell.X, yBottomIndex]);
         if (xRightIndex > -1 && yBottomIndex > -1)
            cell.Neighbors.Add(boardState.Grid[xRightIndex, yBottomIndex]);
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, $"GetNeighbors errors on x={x}, y={y}");
         throw;
      }
      return cell;
   }

   public async Task<BoardState> GetBoardStateAtTick(BoardState boardState, int requestedTick)
   {

      return await Task.FromResult(boardState);
   }

   public async Task<BoardState> GetFinalState(BoardState boardState)
   {

      return await Task.FromResult(boardState);
   }

   //NOTE: ProducerConsumer - if time
   //public async Task<BoardState> ProcessGrid(BoardState boardState)
   //{
   //   var blockingCollection = new BlockingCollection<int>(boundedCapacity: boardState.TotalCellCount);



   //   // Start the producer task
   //   var producer = await Parallel.ForEachAsync(boardState.Grid, async (number, cancellationToken) =>
   //   {
   //      await Task.Delay(500); // Simulate asynchronous work
   //      Console.WriteLine($"Processed number: {number}");
   //   });

   //   for (int i = 0; i < 20; i++)
   //   {
   //      blockingCollection.Add(i); // Add items to the collection
   //   }
   //   blockingCollection.CompleteAdding(); // Signal that no more items will be added



   //   // Start the consumer task
   //   var consumer = Task.Run(() =>
   //   {
   //      foreach (var item in blockingCollection.GetConsumingEnumerable())
   //      {
   //         Console.WriteLine($"Consumed: {item}");
   //         Thread.Sleep(150); // Simulate work
   //      }
   //   });

   //   // Wait for both tasks to complete
   //   Task.WaitAll(producer, consumer);

   //   Console.WriteLine("Processing complete.");
   //}



}
