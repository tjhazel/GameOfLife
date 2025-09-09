using GameOfLife.Domain.Models;

namespace GameOfLife.Domain.Services;

public interface IBoardStateService
{
   Task<BoardStateRequest?> GetBoardStateRequest(string gameId);
   Task<BoardState?> Get(string gameId);
   Task<BoardStateRequest> ConvertToBoardStateRequest(BoardState boardState);
   Task<BoardState> ConvertToBoardState(BoardStateRequest request);
   Task<string> Save(BoardState boardState);
   Task<string> SaveOriginal(BoardState boardState);
}
