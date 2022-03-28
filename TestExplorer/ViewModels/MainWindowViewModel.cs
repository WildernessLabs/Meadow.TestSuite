using ReactiveUI;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Meadow.Workbench;

public class MainWindowViewModel : ViewModelBase
{
    public TestSuiteViewModel TestSuiteViewModel { get; }

    private string? _meadowAddress;
    private string? _status;
    private string? _deviceTime;

    public MainWindowViewModel()
    {
        TestSuiteViewModel = new TestSuiteViewModel();
        TestSuiteViewModel.StatusChanged += (s) =>
        {
            Status = s;
        };

        MeadowAddress = "192.168.1.87:8080"; // TODO: get from user settings (also test assembly folder)
    }

    public string? MeadowAddress
    {
        get => _meadowAddress;
        set
        {
            this.RaiseAndSetIfChanged(ref _meadowAddress, value);
            this.RaisePropertyChanged(nameof(AddressIsReasonable));
        }
    }

    public bool AddressIsReasonable
    {
        get => IPEndPoint.TryParse(MeadowAddress ?? String.Empty, out IPEndPoint _);
    }

    public string? Status
    {
        get => _status;
        set => this.RaiseAndSetIfChanged(ref _status, value);
    }

    public string? DeviceTime
    {
        get => _deviceTime;
        set => this.RaiseAndSetIfChanged(ref _deviceTime, value);
    }

    public async void OnTestConnection()
    {
        if (MeadowAddress == null) return;

        if (await TestSuiteViewModel.CreateDirector(IPEndPoint.Parse(MeadowAddress)))
        {
            IsConnected = true;
        }

        // TODO: start clock thread
        _ = Task.Run(async () =>
        {
            while (IsConnected)
            {
                var time = await TestSuiteViewModel.GetDeviceTime();
                DeviceTime = $"{time:HH:mm:ss}";
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        });
    }

    public bool IsConnected { get; private set; } = false;

    public async void SetClockCommand()
    {
        Status = $"Setting device clock...";
        await TestSuiteViewModel.SetDeviceTime(DateTime.Now);
        Status = $"Done.";
    }

    public async void ResetCommand()
    {
        Status = $"Resetting device...";
        await TestSuiteViewModel.ResetDevice();
        Status = $"Done.";
    }

    public async void DebugCommand()
    {
        Status = $"Turning on device Debug output...";
        await TestSuiteViewModel.EnableDebugOutput();
        Status = $"Done.";
    }

}
