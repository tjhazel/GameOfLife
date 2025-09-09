using GameOfLife.Domain.Models;

namespace GameOfLife.Domain.Services;

public interface IBoardStateService
{
   Task<BoardStateRequest> GetLatestBoardStateRequest(string gameId);
   Task<BoardStateRequest> GetOriginalBoardStateRequest(string gameId);
   Task<BoardStateRequest> ConvertToBoardStateRequest(BoardState boardState);
   Task<BoardState> Get(string gameId);
   Task<BoardState> GetOriginal(string gameId);
   Task<BoardState> ConvertToBoardState(BoardStateRequest request);
   Task<string> Save(BoardState boardState);
   Task<string> SaveOriginal(BoardState boardState);
}
