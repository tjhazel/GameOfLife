using GameOfLife.Domain;
using GameOfLife.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameOfLife.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController(Game game) : Controller
{
   private readonly Game _game = game;

   [HttpPost]
   public async Task<string> UploadBoardState(BoardStateRequest request)
   {
      return await _game.UploadBoardState(request);
   }

   [HttpPatch, Route("[action]/{gameId}")]
   public async Task<BoardStateRequest> GetNextBoardState(string gameId)
   {
      return await _game.GetNextBoardState(gameId);
   }

   [HttpPatch, Route("[action]/{gameId}/{tick}")]
   public async Task<BoardStateRequest> GetBoardStateAtTick(string gameId, int tick)
   {
      return await _game.GetBoardStateAtTick(gameId, tick);
   }

   [HttpPatch, Route("[action]/{gameId}")]
   public async Task<BoardStateRequest> GetFinalState(string gameId)
   {
      return await _game.GetFinalState(gameId);
   }

   #region convience methods

   [HttpGet, Route("[action]/{gameId}")]
   public async Task<BoardStateRequest> GetOriginalBoardState(string gameId)
   {
      return await _game.GetOriginalBoardState(gameId);
   }

   [HttpGet, Route("[action]/{gameId}")]
   public async Task<BoardStateRequest> GetCurrentBoardState(string gameId)
   {
      return await _game.GetLatestBoardState(gameId);
   }

   [HttpDelete, Route("[action]/{gameId}")]
   public async Task<BoardStateRequest> ResetBoard(string gameId)
   {
      return await _game.ResetGame(gameId);
   }

   [HttpGet, Route("[action]")]
   public async Task<string[]> GetGameList()
   {
      return await _game.GetGameList();
   }

   #endregion convience methods
}
