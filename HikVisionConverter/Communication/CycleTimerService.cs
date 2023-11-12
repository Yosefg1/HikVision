using System.Timers;

namespace HikVisionConverter.Communication;

public class CycleTimerService
{
    private readonly System.Timers.Timer _fullStatusTimer;

    private const int FULLSTATUS_INTERVAL = 60 * 1000;

    private readonly MarsRepository _repo;

    public CycleTimerService(MarsRepository repo)
    {
        _repo = repo;

        _fullStatusTimer = new(FULLSTATUS_INTERVAL);
        _fullStatusTimer.Elapsed += FullStatusTimer_Elapsed;

    }

    public void Start()
    {
        _fullStatusTimer.Start();
    }

    private void FullStatusTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        foreach (var item in _repo.MarsClients)
        {
            try
            {
                if (item.Value is not null)
                    item.Value.SoapClient!.doDeviceStatusReportAsync(ResponseMapper.Map(_repo.FullStatusReport));
            }
            catch (Exception)
            {
                Console.WriteLine($"Error Sending FullStatusReport to {item.Key}");
            }
        }
    }
}
