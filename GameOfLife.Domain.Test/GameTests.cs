using GameOfLife.Domain.Models;
using GameOfLife.Domain.Services;
using GameOfLife.Domain.Test.Utilities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace GameOfLife.Domain.Test;

public class GameTests(ITestOutputHelper output)
{
   private readonly ITestOutputHelper _output = output;

   [Fact]
   [Trait("Category", "Unit")]
   public async Task UploadBoardState_Returns_String()
   {
      //arrange
      const string expectedGameId = "test";
      BoardStateRequest expectedRequest = new ()
      {
         GameId = expectedGameId,
         Pattern = [".", "0"]
      };

      bool[,] matrix = new bool[,]
      {
          {false, true}
      };

      BoardState expectedState = new ()
      {
         GameId = expectedGameId,
         Grid = new bool[,] {{false, true}}
      };

      XUnitLogger<Game> gameLogger = new(_output);

      Mock<IGridService> mockGridService = new();
      Mock<IBoardStateService> mockBoardStateService = new();
      mockBoardStateService.Setup(x => x.ConvertToBoardState(expectedRequest))
         .ReturnsAsync(expectedState);
      mockBoardStateService.Setup(x => x.SaveOriginal(It.IsAny<BoardState>()))
         .ReturnsAsync(@"full path\content\known");

      var game = new Game(gameLogger, mockBoardStateService.Object, mockGridService.Object);

      //act
      var actualResult = await game.UploadBoardState(expectedRequest);
      gameLogger.LogInformation($"UploadBoardState_Returns_String returned {actualResult}");
      //assert
      Assert.Equal(expectedRequest.GameId, actualResult);
   }

   [Fact]
   [Trait("Category", "Ignore")]
   public async Task GetOriginalBoardState_Returns_ReturnsLatestBoardStateRequest()
   {
      // TODO: Implement test
   }

   [Fact]
   [Trait("Category", "Ignore")]
   public async Task GetLatestBoardStateRequest_WithValidGameId_ReturnsLatestBoardStateRequest()
   {
      // TODO: Implement test
   }


   [Fact]
   [Trait("Category", "Ignore")]
   public async Task GetOriginalBoardStateRequest_WithValidGameId_ReturnsOriginalBoardStateRequest()
   {
      // TODO: Implement test
   }

}