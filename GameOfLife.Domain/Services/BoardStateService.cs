using GameOfLife.Domain.Extensions;
using GameOfLife.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameOfLife.Domain.Services;

/// <summary>
/// TODO: this is touching the file system, which is not something we would *most likely not do in a real production app. 
/// - A better approach would be to use document storage 
/// </summary>
public class BoardStateService(ILogger<BoardStateService> logger) : IBoardStateService
{
   private readonly ILogger<BoardStateService> _logger = logger;
   //trying to match existing files from the wild
   static char[] activeBits = { '1', 'X', 'Y', 'O' }; //not 0 
   //using this to help locate the path
   const string ContentFolder = "Content";
   const string OriginalFolder = "Original";
   const string StatefulFolder = "Stateful";

   /// <summary>
   /// Get the latest saved request
   /// </summary>
   /// <param name="gameId"></param>
   /// <returns></returns>
   public async Task<BoardStateRequest> GetLatestBoardStateRequest(string gameId)
   {
      return await GetBoardStateRequest(gameId, true);
   }

   /// <summary>
   /// Get the original saved request
   /// </summary>
   /// <param name="gameId"></param>
   /// <returns></returns>
   public async Task<BoardStateRequest> GetOriginalBoardStateRequest(string gameId)
   {
      return await GetBoardStateRequest(gameId, false);
   }

   /// <summary>
   /// This will get the latest saved request as an actionable board
   /// </summary>
   /// <param name="gameId"></param>
   /// <returns></returns>
   public async Task<BoardState> Get(string gameId)
   {
      var request = await GetLatestBoardStateRequest(gameId);
      return await ConvertToBoardState(request);
   }

   /// <summary>
   /// This will get the original saved request as an actionable board
   /// </summary>
   /// <param name="gameId"></param>
   /// <returns></returns>
   public async Task<BoardState> GetOriginal(string gameId)
   {
      DeleteBoardState(gameId);
      var request = await GetOriginalBoardStateRequest(gameId);
      return await ConvertToBoardState(request);
   }

   /// <summary>
   /// string here is only because of our localized storage solution - helps us find the file
   /// </summary>
   /// <param name="boardState"></param>
   /// <returns></returns>
   public async Task<string> SaveOriginal(BoardState boardState)
   {
      return await SaveBoard(boardState, OriginalFolder);
   }

   /// <summary>
   /// string here is only because of our localized storage solution - helps us find the file
   /// </summary>
   /// <param name="boardState"></param>
   /// <returns></returns>
   public async Task<string> Save(BoardState boardState)
   {
      return await SaveBoard(boardState, StatefulFolder);
   }

   async Task<string> SaveBoard(BoardState boardState, string folderName)
   {
      var request = await ConvertToBoardStateRequest(boardState);
      return await SerializeBoardStateRequest(request, folderName);
   }

   /// <summary>
   /// Convert boards for saving or returning to the api client
   /// </summary>
   /// <param name="boardState"></param>
   /// <returns></returns>
   public async Task<BoardStateRequest> ConvertToBoardStateRequest(BoardState boardState)
   {
      if (boardState.Grid == null)
      {
         _logger.LogError("Unable to save BoardState with null rows");
      }
      else if (boardState.Grid.GetLength(0) == 0)
      {
         _logger.LogError("Unable to save BoardState with no rows");
      }

      List<string> rows = new();
      for (int rowIdx = 0; rowIdx < boardState.Grid!.GetLength(0); rowIdx++)
      {
         var row = boardState.Grid.GetRow(rowIdx);
         string rowString = string.Join("", row.Select(y => y == true ? "O" : "."));
         rows.Add(rowString);
      }

      BoardStateRequest request = new BoardStateRequest
      {
         GameId = boardState.GameId,
         Pattern = rows.ToArray(),
         Tick = boardState.Tick,
         StartActiveCellCount = boardState.StartActiveCellCount,
         FinishActiveCellCount = boardState.FinishActiveCellCount
      };

      return await Task.FromResult(request);
   }

   public async Task<BoardState> ConvertToBoardState(BoardStateRequest request)
   {
      if (request.Pattern == null)
      {
         _logger.LogError("Unable to save BoardStateRequest with null request.Pattern");
         throw new ArgumentException("Unable to save BoardStateRequest with null request.Pattern", "Pattern");
      }
      if (request.Pattern.Length == 0)
      {
         _logger.LogError("Unable to save BoardStateRequest with no rows");
         throw new ArgumentException("Unable to save BoardStateRequest with empty request.Pattern", "Pattern");
      }

      //used to keep track of active count at start
      int activeCellCount = 0;

      //avoid tipping over if one line is longer than the rest
      int maxRowLength = request.Pattern.Max(s => s.Length);

      bool[,] rows = new bool[request.Pattern.Length, maxRowLength];

      for (int lineIdx = 0; lineIdx < request.Pattern.Length; lineIdx++)
      {
         var colArr = request.Pattern[lineIdx].ToCharArray();
         for (int colIdx = 0; colIdx < maxRowLength; colIdx++)
         {
            bool selected = false;

            if (colIdx < colArr.Length) 
               selected = activeBits.Contains(colArr[colIdx]);

            if (selected) 
               Interlocked.Increment(ref activeCellCount);

            rows[lineIdx, colIdx] = selected;
         }
      }

      BoardState boardState = new()
      {
         GameId = request.GameId,
         Tick = request.Tick ?? 0,
         Grid = rows,
         StartActiveCellCount = activeCellCount,
         //FinishActiveCellCount //don't set as we should only set after processing
      };

      return await Task.FromResult(boardState);
   }

   /// <summary>
   /// pull list of game names based on existing test files
   /// </summary>
   /// <returns></returns>
   public async Task<string[]> GetGameList()
   {
      var targetFolderPath = GetContentFolder(OriginalFolder);

      string[] gameIds = Directory.GetFiles(targetFolderPath, "*.json")
         .Select(y => Path.GetFileNameWithoutExtension(y))
         .ToArray();

      return await Task.FromResult(gameIds);
   }

   #region Private Bits

   //TODO: should move File System bits somewhere behind a moqable interface
   async Task<string> SerializeBoardStateRequest(BoardStateRequest request, string targetFolder)
   {
      var safeFileName = ReplaceInvalidFileNameChars(request.GameId);
      var targetFolderPath = GetContentFolder(targetFolder);

      var targetFilePath = Path.Combine(targetFolderPath, $"{safeFileName}");

      if (targetFolder.Equals(OriginalFolder, StringComparison.OrdinalIgnoreCase) && File.Exists(targetFilePath))
      {
         _logger.LogDebug($"####  GameId: {request.GameId} - Original file already exists - choose a new gameId.");
         throw new ArgumentException($"Original file already exists: {request.GameId} - choose a new gameId.", "GameId");
      }

      JsonSerializerOptions jsonOptions = new()
      {
         PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
         WriteIndented = true,
         DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
      };
      try
      {
         var jsonString = JsonSerializer.Serialize(request, jsonOptions);
         _logger.LogInformation($"####  Saving GameId: {request.GameId}");
         _logger.LogInformation(jsonString);

         //TODO: Race condition in waiting - this will replace any prior state file, meaning there *should be only one logical consumer
         await File.WriteAllBytesAsync(targetFilePath, Encoding.UTF8.GetBytes(jsonString));
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, $"Failed to write state to {targetFilePath}");
         throw;
      }
      return targetFilePath;
   }

   /// <summary>
   /// This will look in the stateful folder, if not found then model has not 
   /// been started yet. The model will be pulled from the Stateful folder.
   /// </summary>
   /// <param name="gameId"></param>
   /// <returns></returns>
   /// <exception cref="ArgumentException"></exception>
   async Task<BoardStateRequest> GetBoardStateRequest(string gameId, bool latest)
   {
      var safeFileName = ReplaceInvalidFileNameChars(gameId);
      string? foundFilePath = null;
      if (latest)
      {
         //check root folder for a working state
         foundFilePath = GetLocalFolderPath(Path.Combine(GetContentFolder(StatefulFolder), safeFileName));
      }

      if (string.IsNullOrWhiteSpace(foundFilePath) || !File.Exists(foundFilePath))
      {
         //check for an original file
         foundFilePath = GetLocalFolderPath(Path.Combine(GetContentFolder(OriginalFolder), safeFileName));
      }

      if (!File.Exists(foundFilePath))
      {
         _logger.LogWarning($"Unable to find state at {foundFilePath}");
         throw new ArgumentException($"Unable to find state at {foundFilePath}", nameof(gameId));
      }

      string jsonString = File.ReadAllText(foundFilePath);
      if (string.IsNullOrWhiteSpace(jsonString))
      {
         _logger.LogWarning($"Empty file at {foundFilePath}");
         throw new ArgumentException($"Empty file at {foundFilePath}", nameof(gameId));
      }

      try
      {
         _logger.LogInformation($"####  Loading GameId: {gameId}");
         _logger.LogInformation(jsonString);

         JsonSerializerOptions jsonOptions = new()
         {
            PropertyNameCaseInsensitive = true
         };
         var request = JsonSerializer.Deserialize<BoardStateRequest>(jsonString, jsonOptions);
         return await Task.FromResult(request!);
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, $"Failed to deserialize state from {foundFilePath}");
         throw;
      }
   }
   void DeleteBoardState(string gameId)
   {
      var safeFileName = ReplaceInvalidFileNameChars(gameId);
      var targetFolderPath = GetContentFolder(StatefulFolder);

      var targetFilePath = Path.Combine(targetFolderPath, $"{safeFileName}");

      //could be a race, so send a debug message and try to delete
      if (!File.Exists(targetFilePath))
      {
         _logger.LogDebug($"####  GameId: {safeFileName} - no state file found.");
      }

      try
      {
         File.Delete(targetFilePath);
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, $"Failed to delete state file {targetFilePath}");
      }
   }

   static string GetContentFolder(string folderName)
   {
      return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ContentFolder, folderName);

   }

   static string GetLocalFolderPath(string startPath)
   {
      string relativePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, startPath);
      return relativePath;
   }

   static string ReplaceInvalidFileNameChars(string name)
   {
      string wipName = name;
      foreach (var c in Path.GetInvalidFileNameChars())
      {
         wipName = wipName.Replace(c, '-');
      }

      return wipName.EndsWith(".json") ? wipName : $"{wipName}.json";
   }

   #endregion Private Bits
}
