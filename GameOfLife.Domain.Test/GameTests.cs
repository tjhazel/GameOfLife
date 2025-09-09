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
   public void Upload_Board_State_New_Successful()
   {
     // //arrange
     // Mock<IGridService> mockGridService = new();
     // XUnitLogger<Game> boardStateServiceLogger = new(_output);

     // BoardState boardState = new()
     // {
     //    GameId = $"Test_{DateTime.UtcNow.Ticks}"
     //    ,Grid = new []
     //    {
     //       new Row()
     //       {
     //          Cells = new []
     //          {
     //             new Cell(false),
     //             new Cell(false),
     //             new Cell(true),
     //             new Cell(true)
     //          }
     //       },
     //       new Row()
     //       {
     //          Cells = new []
     //          {
     //             new Cell(false),
     //             new Cell(false),
     //             new Cell(true),
     //             new Cell(true)
     //          }
     //       }
     //    }
     // };

     // //var game = new Game(mockGameLogger.Object, 
     // //  mockGridService.Object);


     // //action
     //// var testData = game.UploadBoardState(boardState);

     // //assert

   }

   //[Fact]
   //public void Get_Next_Board_State_New_Successful()
   //{
   //   //arrange
   //   var testData = GetTestData();

   //   //action

   //   //assert
   //}
}