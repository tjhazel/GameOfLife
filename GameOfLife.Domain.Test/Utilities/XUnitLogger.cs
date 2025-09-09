using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace GameOfLife.Domain.Test.Utilities;

/// <summary>
/// Logger to allow unit tests to capture ILogger results. Borrowed 
/// from https://stackoverflow.com/a/47713709/2661476
/// </summary>
/// <typeparam name="T"></typeparam>
internal class XUnitLogger<T>(ITestOutputHelper output) : ILogger<T>, IDisposable
{
   private readonly ITestOutputHelper _output = output;

   public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
   {
      _output.WriteLine(state!.ToString());
   }

   public bool IsEnabled(LogLevel logLevel)
   {
      return true;
   }

   public IDisposable BeginScope<TState>(TState state)
   {
      return this;
   }

   public void Dispose()
   {
   }
}