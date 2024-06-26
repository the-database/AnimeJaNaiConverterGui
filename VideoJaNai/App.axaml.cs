using VideoJaNai.ViewModels;
using VideoJaNai.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using NuGet.Versioning;
using ReactiveUI;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace VideoJaNai
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (!Directory.Exists(Program.AppStateFolder))
            {
                Directory.CreateDirectory(Program.AppStateFolder);
            }

            if (!File.Exists(Program.AppStatePath))
            {
                File.Copy(Program.AppStateFilename, Program.AppStatePath);
            }

            var suspension = new AutoSuspendHelper(ApplicationLifetime!);
            RxApp.SuspensionHost.CreateNewAppState = () => new MainWindowViewModel();
            RxApp.SuspensionHost.SetupDefaultSuspendResume(Program.SuspensionDriver);
            suspension.OnFrameworkInitializationCompleted();
            
            // Load the saved view model state.
            var state = RxApp.SuspensionHost.GetAppState<MainWindowViewModel>();

            foreach (var wf in state.Workflows)
            {
                wf.Vm = state;
            }

            state.CurrentWorkflow?.Validate();

            new MainWindow { DataContext = state }.Show();
            base.OnFrameworkInitializationCompleted();
        }
    }
}