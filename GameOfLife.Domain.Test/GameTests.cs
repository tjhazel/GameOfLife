using GameOfLife.Domain.Models;
using GameOfLife.Domain.Services;
using GameOfLife.Domain.Test.Utilities;
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
      Mock<IGridService> mockGridService = new();
      Mock<IBoardStateService> mockBoardStateService = new();
      XUnitLogger<Game> gameLogger = new(_output);

      var game = new Game(gameLogger, mockBoardStateService.Object, mockGridService.Object);

      BoardStateRequest expectedRequest = new BoardStateRequest()
      {
         GameId = "test",
         Pattern = [".", "0"]
      };

      //act
      var actualResult = await game.UploadBoardState(expectedRequest);

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