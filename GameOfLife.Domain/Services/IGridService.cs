using GameOfLife.Domain.Models;

namespace GameOfLife.Domain.Services;

public interface IGridService
{
   Task<BoardState> GetNextBoardState(BoardState boardState);
   Task<BoardState> GetBoardStateAtTick(BoardState boardState, int requestedTick);
   Task<BoardState> GetFinalState(BoardState boardState);
}
