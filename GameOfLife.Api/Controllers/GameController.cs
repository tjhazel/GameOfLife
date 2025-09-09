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
      var gameId = await _game.UploadBoardState(request);
      return gameId;
   }

   [HttpGet, Route("[action]/{gameId}")]
   public Task<BoardStateRequest?> GetOriginalBoardState(string gameId)
   {
      var board = _game.GetLatestBoardState(gameId);
      return board;
   }


   [HttpPatch, Route("[action]/{gameId}")]
   public Task<BoardStateRequest?> GetNextBoardState(string gameId)
   {
      var board = _game.GetNextBoardState(gameId);
      return board;
   }
}
