using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TestExplorer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string? _assemblyPath;
        private string[]? _assemblies;

        public string? LocalAssemblyPath
        {
            get => _assemblyPath;
            set
            {
                if (value == LocalAssemblyPath) return;
                this.RaisePropertyChanged();

                // get assemblies
                var di = new DirectoryInfo(value);
                string[] files = null;
                if (di.Exists)
                {
                    files = di.GetFiles("*.dll").Select(fi => fi.Name).ToArray();
                }

                Assemblies = files;
            }
        }

        public string[]? Assemblies
        {
            get => _assemblies;
            set
            {
                this.RaiseAndSetIfChanged(ref _assemblies, value);
            }
        }
    }
}
