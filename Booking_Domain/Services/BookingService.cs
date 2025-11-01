using Booking_Data.Entities;
using Booking_Data.Repository.EF;

public class RoomException : Exception
{
    public RoomException(string? message) : base(message) { }
}
public class BookingService
{
    private  readonly IRoomModelRepository _dbContext;

    private  static readonly object _getMonitor;
    private  static readonly object _lockBook;
    private  static SemaphoreSlim _bookingSemaphore;
    private  static readonly Mutex _logMutex;

    private const string _logFilePath = "booking_log.txt";

    private const int MaxConcurrentBookings = 5;
    private const int CountRooms = 20;

    private static List<CancellationTokenSource> cancellationSources;
    private static List<Task> taskSources;

    static BookingService()
    {
        _bookingSemaphore = new SemaphoreSlim(MaxConcurrentBookings, MaxConcurrentBookings);
        _getMonitor = new object();
        _lockBook = new object();
        _logMutex = new Mutex();

        InitializeLogger();
    }
    public BookingService(IRoomModelRepository dbContext)
    {
        _dbContext = dbContext;
    }

    private Room? GetRoom()
    {
        lock (_getMonitor)
        {
            return _dbContext.GetRoom();
        }
    }
    private async Task BussyRoom(Room room)
    {

        await _dbContext.UpdateRoom(room);

    }
    public async Task BookingStarter()
    {
        lock (_lockBook)
        {
            cancellationSources = new List<CancellationTokenSource>();
            taskSources = new List<Task>();
            Console.WriteLine("Броннируем 20 потокв");

            Parallel.For(1, CountRooms, (int i) =>
            {
                CancellationTokenSource cts = new();
                CancellationToken token = cts.Token;
                cancellationSources.Add(cts);

                var myTask = Task.Run(async () => await Booking(token));
                taskSources.Add(myTask);
            });
            Task.WaitAll(taskSources.ToArray());
        }
    }
    public async Task BookingCanceler()
    {
        if (cancellationSources.Count > 15)
        {
            cancellationSources[6].Cancel();
            cancellationSources[9].Cancel();
            cancellationSources[12].Cancel();
            cancellationSources[15].Cancel();
        }
    }

    private async Task Booking(CancellationToken cancellationToken)
    {
        _bookingSemaphore.Wait();
        try
        {
            if (cancellationToken.IsCancellationRequested) 
            {
                await LogMessage($"{Thread.CurrentThread.Name}: Operation was canceled")
                     .ConfigureAwait(true);
                return;
            }

            var roomToBook = GetRoom();
            if (roomToBook == null || roomToBook?.Id == null)
            {
                throw new RoomException($"Попытка забронировать несуществующий номер");
            }
            roomToBook.IsBooked = true;
            await BussyRoom(roomToBook);

            int roomId = roomToBook.Id;
            await LogMessage($"Номер {roomId} успешно забронирован (ожидание оплаты).")
                 .ConfigureAwait(true);

            await PaymentAsync(roomId)
                 .ConfigureAwait(true);

            await LogMessage($"Оплата для номера {roomId} прошла успешно. Бронирование подтверждено.")
                 .ConfigureAwait(true);
        }
        catch (Exception _) when (_ is OperationCanceledException or TaskCanceledException)
        {
            await LogMessage($"{Thread.CurrentThread.Name}: Operation was canceled")
                 .ConfigureAwait(true);
        }
        catch (RoomException ex)
        {
            await LogMessage($"{ex.Message}")
                 .ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            await LogMessage($"Ошибка при бронировании номера: {ex.Message}")
                 .ConfigureAwait(true);
        }
        finally
        {
            _bookingSemaphore.Release();
        }
    }

    private async Task PaymentAsync(int roomId)
    {
        int delay = new Random().Next(1000, 3000);
        await Task.Delay(10000);
    }

    private static async ValueTask LogMessage(string message)
    {
        _logMutex.WaitOne();
        try
        {
            using (StreamWriter writer = new StreamWriter(_logFilePath, true))
            {
                await writer.WriteLineAsync($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {message}\n")
                     .ConfigureAwait(true);
            }
        }
        finally
        {
            _logMutex.ReleaseMutex();
        }
    }

    private static void InitializeLogger()
    {
        try
        {
            if (!File.Exists(_logFilePath))
            {
                File.Delete(_logFilePath);
            }
            File.Create(_logFilePath).Close();
            LogMessage("--- Логирование бронирований начато ---");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при создании лог-файла: {ex.Message}");
        }
    }
}