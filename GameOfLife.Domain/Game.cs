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
   /// Get an uploaded board. Not part of the requirements, but makes is easier to work with the UploadBoardState Api
   /// </summary>
   /// <param name="gameId"></param>
   /// <returns></returns>
   public async Task<BoardStateRequest?> GetLatestBoardState(string gameId)
   {
      return await _boardStateService.GetBoardStateRequest(gameId);
   }

   public async Task<BoardStateRequest> GetNextBoardState(string gameId)
   {
      var boardState = await _boardStateService.Get(gameId);

      if (boardState?.StartActiveCellCount == 0)
      {
         _logger.LogDebug($"GameId {gameId} has no active cells");
         throw new ArgumentException($"GameId {gameId} has no active cells.", nameof(gameId));
      }

      var nextBoardState = await _gridService.GetNextBoardState(boardState);
      var cacheLocation = await _boardStateService.Save(nextBoardState);

      return await _boardStateService.ConvertToBoardStateRequest(nextBoardState);
   }

   public async Task<BoardStateRequest> GetBoardStateAtTick(string instance, string gameId, int requestedTick)
   {
      return await Task.FromResult(default(BoardStateRequest));
   }

   public async Task<BoardStateRequest> GetFinalState(string instance, string gameId)
   {
      return await Task.FromResult(default(BoardStateRequest));
   }
}
