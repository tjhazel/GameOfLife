using GameOfLife.Domain.Models;
using GameOfLife.Domain.Services;
using Microsoft.Extensions.Logging;

namespace GameOfLife.Domain;

/// <summary>
///   grid will need state loaded and passed into here.
/// </summary>
/// <param name="grid"></param>
public class Game(ILogger<Game> logger,
   IBoardStateService boardStateService,
   IGridService gridService)
{
   private readonly ILogger<Game> _logger = logger;
   private readonly IGridService _gridService = gridService;
   private readonly IBoardStateService _boardStateService = boardStateService;

   /// <summary>
   /// Upload a fresh board
   /// </summary>
   /// <param name="request"></param>
   /// <returns></returns>
   public async Task<string> UploadBoardState(BoardStateRequest request)
   {
      ArgumentNullException.ThrowIfNullOrWhiteSpace("GameId", request?.GameId);
      request!.Tick = 0;

      //convert to a board state to make sure we don't save garbage
      var boardState = await _boardStateService.ConvertToBoardState(request);
      
      //this will convert back to request and save
      var cacheLocation = await _boardStateService.SaveOriginal(boardState);

      _logger.LogDebug($"Board State {boardState.GameId} saved to {cacheLocation}");

      return boardState.GameId;
   }

   /// <summary>
   /// Get original board. Not part of the requirements
   /// </summary>
   /// <param name="gameId"></param>
   /// <returns></returns>
   public async Task<BoardStateRequest> GetOriginalBoardState(string gameId)
   {
      return await _boardStateService.GetOriginalBoardStateRequest(gameId);
   }

   /// <summary>
   /// Get latest board. Not part of the requirements
   /// </summary>
   /// <param name="gameId"></param>
   /// <returns></returns>
   public async Task<BoardStateRequest> GetLatestBoardState(string gameId)
   {
      return await _boardStateService.GetLatestBoardStateRequest(gameId);
   }

   /// <summary>
   /// Process the next tick and return results
   /// </summary>
   /// <param name="gameId"></param>
   /// <returns></returns>
   /// <exception cref="ArgumentException"></exception>
   public async Task<BoardStateRequest> GetNextBoardState(string gameId)
   {
      var boardState = await _boardStateService.Get(gameId);

      if (boardState.StartActiveCellCount == 0)
      {
         _logger.LogDebug($"GameId {gameId} has no active cells");
         throw new ArgumentException($"GameId {gameId} has no active cells.", nameof(gameId));
      }

      var nextBoardState = await _gridService.GetNextBoardState(boardState);
      var cacheLocation = await _boardStateService.Save(nextBoardState);

      return await _boardStateService.ConvertToBoardStateRequest(nextBoardState);
   }

   public async Task<BoardStateRequest> ResetGame(string gameId)
   {
      var boardState = await _boardStateService.GetOriginal(gameId);
      return await _boardStateService.ConvertToBoardStateRequest(boardState);
   }

   public async Task<BoardStateRequest> GetBoardStateAtTick(string gameId, int tick)
   {
      var boardState = await _boardStateService.GetOriginal(gameId);

      if (boardState.StartActiveCellCount == 0)
      {
         _logger.LogDebug($"GameId {gameId} has no active cells");
         throw new ArgumentException($"GameId {gameId} has no active cells.", nameof(gameId));
      }

      do
      {
         try
         {
            boardState = await _gridService.GetNextBoardState(boardState);
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, $"GameId {gameId} failed to move to tick {tick}");
            break;
         }
      } while (boardState.Tick < tick || boardState.FinishActiveCellCount == 0);

      var cacheLocation = await _boardStateService.Save(boardState);

      return await _boardStateService.ConvertToBoardStateRequest(boardState);
   }

   public async Task<BoardStateRequest> GetFinalState(string gameId)
   {
      var boardState = await _boardStateService.Get(gameId);

      if (boardState.StartActiveCellCount == 0)
      {
         _logger.LogDebug($"GameId {gameId} has no active cells");
         throw new ArgumentException($"GameId {gameId} has no active cells.", nameof(gameId));
      }

      const int maxTicks = 100;
      BoardState nextBoardState = boardState;
      do
      {
         try
         {
            nextBoardState = await _gridService.GetNextBoardState(nextBoardState);
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, $"GameId {gameId} failed to move to next tick {nextBoardState.Tick+1}");
            break;
         }
         
         if (nextBoardState.Tick >= maxTicks)
         {
            _logger.LogInformation($"GameId {gameId} hit the max ticks limit of {maxTicks}");
            break;
         }

         if (nextBoardState.FinishActiveCellCount == 0)
         {
            _logger.LogInformation($"GameId {gameId} completed at {nextBoardState.Tick} ticks");
            break;
         }

      } while (nextBoardState.FinishActiveCellCount > 0);

      var cacheLocation = await _boardStateService.Save(nextBoardState);

      return await _boardStateService.ConvertToBoardStateRequest(nextBoardState);
   }
}
