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

namespace TestExplorer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string? _assemblyPath;
        private string[]? _assemblies;
        private string? _connectionStatus = "Not tested.";
        private string? _meadowAddress;
        private RestTestDirector? _director;

        public ObservableCollection<string> AssembliesToSend { get; } = new ObservableCollection<string>();

        public MainWindowViewModel()
        {
            AssembliesToSend.CollectionChanged += (s, e) =>
            {
                this.RaisePropertyChanged(nameof(SendEnabled));
            };
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

        public string? ConnectionStatus
        {
            get => _connectionStatus;
            set => this.RaiseAndSetIfChanged(ref _connectionStatus, value);
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
            }
            catch (Exception ex)
            {
                ConnectionStatus = ex.Message;
                _director = null;
            }
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

    }
}