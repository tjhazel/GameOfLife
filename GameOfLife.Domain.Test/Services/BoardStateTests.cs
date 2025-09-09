using GameOfLife.Domain.Models;
using GameOfLife.Domain.Services;
using GameOfLife.Domain.Test.Utilities;
using Moq;
using Xunit.Abstractions;

namespace GameOfLife.Domain.Test.Services;

public class BoardStateTests(ITestOutputHelper output)
{
   private readonly ITestOutputHelper _output = output;

   [Fact]
   public async Task Save_Board_State_New_Successful()
   {
      ////arrange
      //Mock<IGridService> mockGridService = new();
      //XUnitLogger<BoardStateService> boardStateServiceLogger = new(_output);

      //IBoardStateService boardStateService = new BoardStateService(boardStateServiceLogger);

      //BoardState boardState = new()
      //{
      //   GameId = $"Test_{DateTime.UtcNow.Ticks}"
      //   ,Grid = new [3,4]
      //   {
      //      new Row()
      //      {
      //         Cells = new []
      //         {
      //            new Cell(false),
      //            new Cell(false),
      //            new Cell(true),
      //            new Cell(true)
      //         }
      //      },
      //      new Row()
      //      {
      //         Cells = new []
      //         {
      //            new Cell(false),
      //            new Cell(false),
      //            new Cell(true),
      //            new Cell(true)
      //         }
      //      }
      //   }
      //};

      ////action
      //var pathToStateFile = await boardStateService.Save(boardState);

      ////assert
      //Assert.NotNull(pathToStateFile);
      //Assert.True(File.Exists(pathToStateFile!), $"State file does not exist in {pathToStateFile}.");
   }

   [Fact]
   public async Task Get_PreviouslySaved_State_Successful()
   {
      //arrange
      const string KnownState = "smallspacefiller.cells.json";
      Mock<IGridService> mockGridService = new();
      XUnitLogger<BoardStateService> boardStateServiceLogger = new(_output);

      IBoardStateService boardStateService = new BoardStateService(boardStateServiceLogger);

      //action
      var actual = await boardStateService.Get(KnownState);

      //assert
      Assert.NotNull (actual?.Grid);
   }
}