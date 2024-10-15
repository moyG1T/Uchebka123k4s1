using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using Uchebka123k4s1.Data.Local.IServices;
using Uchebka123k4s1.Data.Remote.IServices;
using Uchebka123k4s1.Data.Services;
using Uchebka123k4s1.Domain.Contexts;
using Uchebka123k4s1.Domain.IServices;
using Uchebka123k4s1.Domain.Services;
using Uchebka123k4s1.ViewModels;

namespace Uchebka123k4s1
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider _provider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<MainNavContext>();
            services.AddSingleton<UserContext>();

            //services.AddSingleton<IDbService, DbService>();
            services.AddSingleton<DbService>();
            services.AddSingleton<IEntryService>(p =>
            {
                return new EntryService(p.GetRequiredService<UserContext>());
            });

            services.AddSingleton<MainViewModel>(p =>
            {
                return new MainViewModel(
                    CreateLoginNavService(p),
                    p.GetRequiredService<IEntryService>(),
                    p.GetRequiredService<DbService>(),
                    p.GetRequiredService<MainNavContext>());
            });
            services.AddSingleton<MainWindow>(p =>
            {
                return new MainWindow
                {
                    DataContext = p.GetRequiredService<MainViewModel>(),
                };
            });

            services.AddTransient<LoginViewModel>(p =>
            {
                return new LoginViewModel(
                    CreateRegistrationNavService(p),
                    CreateWorkerRecordNavService(p),
                    p.GetRequiredService<IEntryService>(),
                    p.GetRequiredService<DbService>());
            });
            services.AddTransient<RegistrationViewModel>(p =>
            {
                return new RegistrationViewModel(
                    CreateClientNavService(p)
                    );
            });
            services.AddTransient<WorkerRecordViewModel>();
            services.AddTransient<ClientViewModel>();

            _provider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = _provider.GetRequiredService<MainWindow>();
            MainWindow.Show();

            base.OnStartup(e);
        }

        private INavService CreateLoginNavService(IServiceProvider p) =>
            new MainNavService(
                p.GetRequiredService<MainNavContext>(),
                p.GetRequiredService<LoginViewModel>
                );
        private INavService CreateRegistrationNavService(IServiceProvider p) =>
            new MainNavService(
                p.GetRequiredService<MainNavContext>(),
                p.GetRequiredService<RegistrationViewModel>
                );

        private INavService CreateWorkerRecordNavService(IServiceProvider p) =>
            new MainNavService(
                p.GetRequiredService<MainNavContext>(),
                p.GetRequiredService<WorkerRecordViewModel>
                );

        private INavService CreateClientNavService(IServiceProvider p) =>
            new MainNavService(
                p.GetRequiredService<MainNavContext>(),
                p.GetRequiredService<ClientViewModel>
                );
    }
}
