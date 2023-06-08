using System;
using System.Threading.Tasks;
using System.Windows;
using FileTransfer.Models;
using FileTransfer.Services;
using FileTransfer.Services.Abstract;
using FileTransfer.ViewModels;
using FileTransfer.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace FileTransfer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            SetupExceptionHandling();

            _host = Host.CreateDefaultBuilder()
                .UseSerilog((_, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .WriteTo.File("Logs/log-.log", rollingInterval: RollingInterval.Day)
                        .WriteTo.Debug()
                        .MinimumLevel.Warning();
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<MainWindowViewModel>();
                    services.AddSingleton<MainWindow>(serviceProvider => new MainWindow
                    {
                        DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>()
                    });

                    services.AddSingleton(new UserConfig());
                    services.AddSingleton<IDialogService, DialogService>();
                    services.AddSingleton<IUserConfigService, UserConfigService>();
                    services.AddSingleton<IFileService, FileService>();
                    services.AddSingleton<IMemoryMappedFileService, MemoryMappedFileService>();
                })
                .Build();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _host.Start();

            MainWindow = _host.Services.GetRequiredService<MainWindow>();
            MainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();

            base.OnExit(e);
        }

        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");
            };

            DispatcherUnhandledException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }

        private void LogUnhandledException(Exception exception, string source)
        {
            var logger = _host.Services.GetService<ILogger<App>>();

            var message = $"Unhandled exception ({source})";

            try
            {
                var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
                message = $"Unhandled exception in {assemblyName.Name} v{assemblyName.Version}";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception in LogUnhandledException");
            }
            finally
            {
                logger.LogError(exception, message);
            }
        }
    }
}
