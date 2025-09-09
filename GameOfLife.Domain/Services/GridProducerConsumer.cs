//using System;
//using System.Collections.Generic;
//using System.Collections.Concurrent;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Linq;

//namespace GameOfLife.Domain.Services;

///// <summary>
///// Simple producer-consumer that processes a fixed list of items with a custom consumer function
///// </summary>
///// <typeparam name="T">The type of items to process</typeparam>
//public class SimpleProducerConsumer<T>
//{
//   private readonly BlockingCollection<T> _queue;
//   private readonly Task[] _consumerTasks;
//   private readonly Action<T> _consumerFunc;

//   public SimpleProducerConsumer(IEnumerable<T> items, Action<T> consumerFunc, int consumerCount = 2)
//   {
//      if (items == null) throw new ArgumentNullException(nameof(items));
//      if (consumerFunc == null) throw new ArgumentNullException(nameof(consumerFunc));
//      if (consumerCount <= 0) throw new ArgumentException("Consumer count must be greater than 0");

//      _consumerFunc = consumerFunc;
//      _queue = new BlockingCollection<T>();
//      _consumerTasks = new Task[consumerCount];

//      // Add all items to the queue
//      foreach (var item in items)
//      {
//         _queue.Add(item);
//      }

//      // Signal that no more items will be added
//      _queue.CompleteAdding();

//      // Start consumer tasks
//      for (int i = 0; i < consumerCount; i++)
//      {
//         int consumerId = i + 1;
//         _consumerTasks[i] = Task.Run(() => ProcessItems(consumerId));
//      }
//   }

//   /// <summary>
//   /// Wait for all items to be processed
//   /// </summary>
//   public void Wait()
//   {
//      Task.WaitAll(_consumerTasks);
//      _queue.Dispose();
//   }

//   /// <summary>
//   /// Wait for all items to be processed with timeout
//   /// </summary>
//   /// <param name="timeout">Maximum time to wait</param>
//   /// <returns>True if all items were processed within timeout</returns>
//   public bool Wait(TimeSpan timeout)
//   {
//      bool completed = Task.WaitAll(_consumerTasks, timeout);
//      _queue.Dispose();
//      return completed;
//   }

//   /// <summary>
//   /// Consumer loop that processes work items using the provided function
//   /// </summary>
//   private void ProcessItems(int consumerId)
//   {
//      foreach (T item in _queue.GetConsumingEnumerable())
//      {
//         try
//         {
//            _consumerFunc(item);
//         }
//         catch (Exception ex)
//         {
//            Console.WriteLine($"Consumer {consumerId}: Error processing item '{item}': {ex.Message}");
//         }
//      }
//   }

//   /// <summary>
//   /// Get current queue count
//   /// </summary>
//   public int QueueCount => _queue.Count;
//}

////// Example usage
////public class Program
////{
////   public static void Main()
////   {
////      Console.WriteLine("Processing strings...");

////      // Example 1: Process strings
////      var stringItems = new[] { "Task-1", "Task-2", "Task-3", "Task-4", "Task-5" };

////      var stringProcessor = new SimpleProducerConsumer<string>(
////          items: stringItems,
////          consumerFunc: item =>
////          {
////             Console.WriteLine($"Processing: {item} on thread {Thread.CurrentThread.ManagedThreadId}");
////             Thread.Sleep(1000); // Simulate work
////             Console.WriteLine($"Completed: {item}");
////          },
////          consumerCount: 2
////      );

////      stringProcessor.Wait();
////      Console.WriteLine("All strings processed!\n");

////      // Example 2: Process numbers with custom logic
////      Console.WriteLine("Processing numbers...");

////      var numbers = Enumerable.Range(1, 8).ToList();

////      var numberProcessor = new SimpleProducerConsumer<int>(
////          items: numbers,
////          consumerFunc: number =>
////          {
////             int result = number * number;
////             Console.WriteLine($"Square of {number} = {result} (thread {Thread.CurrentThread.ManagedThreadId})");
////             Thread.Sleep(500);
////          },
////          consumerCount: 3
////      );

////      bool completed = numberProcessor.Wait(TimeSpan.FromSeconds(10));
////      Console.WriteLine(completed ? "All numbers processed!" : "Processing timed out!");

////      // Example 3: Process custom objects
////      Console.WriteLine("\nProcessing orders...");

////      var orders = new[]
////      {
////            new Order { Id = 1, Product = "Coffee", Quantity = 2 },
////            new Order { Id = 2, Product = "Tea", Quantity = 1 },
////            new Order { Id = 3, Product = "Cake", Quantity = 3 }
////        };

////      var orderProcessor = new SimpleProducerConsumer<Order>(
////          items: orders,
////          consumerFunc: order =>
////          {
////             Console.WriteLine($"Processing order {order.Id}: {order.Quantity}x {order.Product}");
////             Thread.Sleep(800);
////             Console.WriteLine($"Order {order.Id} shipped!");
////          },
////          consumerCount: 2
////      );

////      orderProcessor.Wait();
////      Console.WriteLine("All orders processed!");
////   }
////}

////// Example custom class
////public class Order
////{
////   public int Id { get; set; }
////   public string Product { get; set; }
////   public int Quantity { get; set; }
////}