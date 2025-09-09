namespace GameOfLife.Domain.Extensions;

/// <summary>
/// Borrowed from <see cref="https://github.com/CommunityToolkit/WindowsCommunityToolkit/blob/6cefc60154fd34eb5397c7480e1c761e7a123b99/Microsoft.Toolkit/Extensions/ArrayExtensions.cs"/>
/// </summary>
public static class ArrayExtensions
{
   /// <summary>
   /// Yields a row from a rectangular array.
   /// </summary>
   /// <typeparam name="T">The element type of the array.</typeparam>
   /// <param name="rectarray">The source array.</param>
   /// <param name="row">Row record to retrieve, 0-based index.</param>
   /// <returns>Yielded row.</returns>
   public static IEnumerable<T> GetRow<T>(this T[,] rectarray, int row)
   {
      if (row < 0 || row >= rectarray.GetLength(0))
      {
         throw new ArgumentOutOfRangeException(nameof(row));
      }

      for (int c = 0; c < rectarray.GetLength(1); c++)
      {
         yield return rectarray[row, c];
      }
   }
}
