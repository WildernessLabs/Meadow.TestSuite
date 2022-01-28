using Avalonia.Controls;
using Meadow.TestSuite;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestExplorer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string? _assemblyPath;
        private string[]? _assemblies;
        private string? _connectionStatus = "Not tested.";
        private string? _meadowAddress;
        private RestTestDirector? _director;
        private string? _status;
        private bool _localVisible;

        public ObservableCollection<string> AssembliesToSend { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> DeviceAssemblies { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> KnownTests { get; } = new ObservableCollection<string>();
        public ObservableCollection<TestResult> KnownResults { get; } = new ObservableCollection<TestResult>();

        public MainWindowViewModel()
        {
            AssembliesToSend.CollectionChanged += (s, e) =>
            {
                this.RaisePropertyChanged(nameof(SendEnabled));
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

        public GridLength Col0Width
        {
            get => LocalIsVisible ? GridLength.Star : new GridLength(0);
        }

        public GridLength Col1Width
        {
            get => LocalIsVisible ? new GridLength(80) : new GridLength(0);
        }

        public GridLength Col4Width
        {
            get => LocalIsVisible ? new GridLength(0) : GridLength.Star;
        }

        public bool LocalIsVisible
        {
            get => _localVisible;
            set
            {
                this.RaiseAndSetIfChanged(ref _localVisible, value);
                this.RaisePropertyChanged(nameof(Col0Width));
                this.RaisePropertyChanged(nameof(Col1Width));
                this.RaisePropertyChanged(nameof(Col4Width));
            }
        }

        public bool AddressIsReasonable
        {
            get => IPEndPoint.TryParse(MeadowAddress ?? String.Empty, out IPEndPoint _);
        }

        public string? ConnectionStatus
        {
            get => _connectionStatus;
            set => this.RaiseAndSetIfChanged(ref _connectionStatus, value);
        }

        public string? Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }

        public string? LocalAssemblyPath
        {
            get => _assemblyPath;
            set
            {
                if (value == LocalAssemblyPath) return;
                _assemblyPath = value;
                this.RaisePropertyChanged();

                // get assemblies
                string[]? files = null;
                if (LocalAssemblyPath != null)
                {
                    var di = new DirectoryInfo(LocalAssemblyPath);
                    if (di.Exists)
                    {
                        files = di.GetFiles("*.dll").Select(fi => fi.Name).ToArray();
                    }
                }

                Assemblies = files;
            }
        }

        public string[]? Assemblies
        {
            get => _assemblies;
            set => this.RaiseAndSetIfChanged(ref _assemblies, value);
        }

        public async void OnTestConnection()
        {
            if (MeadowAddress == null) return;

            _director = new RestTestDirector(IPEndPoint.Parse(MeadowAddress));

            try
            {
                ConnectionStatus = $"Testing...";

                var info = await _director.GetInfo();

                ConnectionStatus = $"Device: {info.Name} version {info.Version}";

                this.RaisePropertyChanged(nameof(SendEnabled));


                // TODO: start clock thread

                _ = RefreshRemoteAssemblies();
                _ = RefreshKnownTests();

            }
            catch (Exception ex)
            {
                ConnectionStatus = ex.Message;
                _director = null;
            }
        }

        public async void RefreshTestsCommand()
        {
            await RefreshKnownTests();
        }

        public async void RefreshAssembliesCommand()
        {
            await RefreshRemoteAssemblies();
        }

        public async void RefreshResultsCommand()
        {
            await RefreshKnownResults();
        }

        public void AddAssembliesCommand()
        {
            LocalIsVisible = !LocalIsVisible;
        }

        public string? SelectedTest
        {
            get; set;
        }

        public async void SetClockCommand()
        {
            if (_director == null) return;
            Status = $"Setting device clock...";
            await _director.SetDeviceTime(DateTime.Now);
            Status = $"Done.";
        }

        public async void ResetCommand()
        {
            if (_director == null) return;
            Status = $"Resetting device...";
            await _director.ResetDevice();
            Status = $"Done.";
        }

        public async void DebugCommand()
        {
            if (_director == null) return;
            Status = $"Turning on device Debug output...";
            await _director.SetDebug(true);
            Status = $"Done.";
        }

        public async void RunTestsCommand()
        {
            if (SelectedTest == null) return;
            if (_director == null) return;

            Status = $"Starting test...";

            await _director.ExecuteTest(SelectedTest);

            Status = $"Done.";
        }

        private async Task RefreshRemoteAssemblies()
        {
            if(_director == null) return;

            DeviceAssemblies.Clear();

            Status = $"Getting assemblies...";

            var remote = await _director.GetAssemblies();

            if (remote != null)
            {
                foreach (var a in remote)
                {
                    DeviceAssemblies.Add(a);
                }
            }

            Status = $"Done.";
        }

        private async Task RefreshKnownTests()
        {
            if (_director == null) return;

            KnownTests.Clear();

            Status = $"Getting tests...";

            var remote = await _director.GetTestNames();

            if (remote != null)
            {
                foreach (var a in remote)
                {
                    KnownTests.Add(a);
                }
            }

            Status = $"Done.";
        }

        private async Task RefreshKnownResults()
        {
            if (_director == null) return;

            KnownResults.Clear();

            Status = $"Getting results...";

            var remote = await _director.GetTestResults();

            if (remote != null)
            {
                foreach (var a in remote)
                {
                    KnownResults.Add(a);
                }
            }

            Status = $"Done.";
        }

        public bool SendEnabled
        {
            get
            {
                if (_director == null) return false;
                if (AssembliesToSend == null) return false;
                return AssembliesToSend.Count > 0;
            }
        }

        public async void SendAssemblies()
        {
            if(_director == null) return;

            foreach (var asm in AssembliesToSend)
            {
                Status = $"Sending {asm}...";

                var fi = new FileInfo(Path.Combine(LocalAssemblyPath ?? String.Empty, asm));
                if (fi.Exists)
                {
                    try
                    {
                        await _director.SendFile(fi);
                    }
                    catch (Exception ex)
                    {
                        Status = ex.Message;
                    }
                }
            }

            Status = $"Done.";

            await RefreshRemoteAssemblies();
        }
    }
}