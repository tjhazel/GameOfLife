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

   [HttpGet, Route("[action]/{gameId}")]
   public Task<BoardStateRequest> GetOriginalBoardState(string gameId)
   {
      return _game.GetOriginalBoardState(gameId);
   }

   [HttpGet, Route("[action]/{gameId}")]
   public Task<BoardStateRequest> GetCurrentBoardState(string gameId)
   {
      return _game.GetLatestBoardState(gameId);
   }

   [HttpPatch, Route("[action]/{gameId}")]
   public Task<BoardStateRequest> GetNextBoardState(string gameId)
   {
      return _game.GetNextBoardState(gameId);
   }
}
