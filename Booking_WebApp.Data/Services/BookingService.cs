using Booking_WebApp.Data.Entities;
using Booking_WebApp.Data.Repository.EF;

using System.Text.RegularExpressions;

namespace Booking_WebApp.Data.Services;

public class BookingService
{
    private  readonly IRoomModelRepository _dbContext;

    private  static readonly object _getMonitor;
    private  static readonly object _lockBooking;
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
        _lockBooking = new object();
        _logMutex = new Mutex();
    }
    public BookingService(IRoomModelRepository dbContext)
    {
        _dbContext = dbContext;
    }

    public void RunBooking()
    {
        lock (_lockBooking)
        {
            cancellationSources = new List<CancellationTokenSource>();
            taskSources = new List<Task>();
            InitializeLogger();
            ClearRooms();

            Parallel.For(0, CountRooms, (int i) =>
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

    public void CancelBooking(int []numbers)
    {
        foreach (int i in numbers)
        {
            cancellationSources[i].Cancel();
        }
    }

    public async Task<string> StatisticBooking()
    {
        Regex regex = new Regex(@"Thread");
        Regex regexCancel = new Regex(@"Thread cancel");
        int count = 0;
        int cancelCount = 0;
        using (StreamReader reader = new StreamReader(_logFilePath))
        {
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (regex.IsMatch(line))
                {
                    count++;
                }

                if (regexCancel.IsMatch(line))
                {
                    cancelCount++;
                }
            }
        }
        return $"Отменено {cancelCount} из {count}";
    }

    private Room? TryReserveRoom()
    {
        lock (_getMonitor)
        {
            var room = _dbContext.GetRoom(); // должен вернуть свободный номер
            if (room == null) return null;

            room.IsBooked = true;
            _dbContext.UpdateRoom(room);
            return room;
        }
    }

    private void ClearRooms()
    {
        lock (_getMonitor)
        {
            _dbContext.ClearRooms();
        }
    }

    private async Task Booking(CancellationToken cancellationToken)
    {
        const int maxAttempts = 2;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            _bookingSemaphore.Wait();

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    await LogMessage($"Thread cancel ({Thread.CurrentThread.Name}): Operation was canceled");
                    return;
                }

                var room = TryReserveRoom();

                if (room != null)
                {
                    int roomId = room.Id;
                    await LogMessage($"Номер {roomId} успешно забронирован (ожидание оплаты).");

                    await PaymentAsync(roomId);

                    await LogMessage($"Оплата для номера {roomId} прошла успешно. Бронирование подтверждено." +
                                     $"\nThread ({Thread.CurrentThread.Name}) was succes");

                    return;
                }

            }
            catch (Exception _) when (_ is OperationCanceledException or TaskCanceledException)
            {
                await LogMessage($"{Thread.CurrentThread.Name}: Operation was canceled");
            }
            catch (Exception ex)
            {
                await LogMessage($"Ошибка при бронировании номера: {ex.Message}");
            }
            finally
            {
                _bookingSemaphore.Release();
            }

            var delay = 2000;
            await LogMessage($"Свободных номеров нет. Повтор через {delay} мс (попытка {attempt}/{maxAttempts}).");
            await Task.Delay(delay);
        }

        await LogMessage("Свободных номеров нет. Попытки исчерпаны." +
                        $"\nThread ({Thread.CurrentThread.Name}) was succes");
        return;
    }

    private async Task PaymentAsync(int roomId)
    {
        int delay = new Random().Next(1000, 3000);
        await Task.Delay(delay);
    }

    private static async Task LogMessage(string message)
    {
        _logMutex.WaitOne();
        try
        {
            using (StreamWriter writer = new StreamWriter(_logFilePath, true))
            {
                await writer.WriteLineAsync($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {message}\n");
            }
        }
        finally
        {
            _logMutex.ReleaseMutex();
        }
    }

    private static async void InitializeLogger()
    {
        _logMutex.WaitOne();
        try
        {
            if (!File.Exists(_logFilePath))
            {
                File.Create(_logFilePath);
            }
            File.CreateText(_logFilePath).Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при создании лог-файла: {ex.Message}");
        }
        finally
        {
            _logMutex.ReleaseMutex();
        }

        await LogMessage("--- Логирование бронирований начато ---");
    }
}