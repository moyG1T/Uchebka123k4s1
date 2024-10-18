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
            services.AddSingleton<IEntryService, EntryService>();

            services.AddSingleton<MainViewModel>(p =>
            {
                return new MainViewModel(
                    CreateLoginNavService(p),
                    CreateDirectorPanelNavService(p),
                    p.GetRequiredService<IEntryService>(),
                    p.GetRequiredService<UserContext>(),
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
                    CreateDirectorPanelNavService(p),
                    p.GetRequiredService<IEntryService>(),
                    p.GetRequiredService<UserContext>(),
                    p.GetRequiredService<DbService>());
            });
            services.AddTransient<RegistrationViewModel>(p =>
            {
                return new RegistrationViewModel(
                    CreateClientNavService(p),
                    p.GetRequiredService<UserContext>(),
                    p.GetRequiredService<IEntryService>(),
                    p.GetRequiredService<DbService>()
                    );
            });
            services.AddTransient<WorkerRecordViewModel>(p =>
            {
                return new WorkerRecordViewModel(
                    CreateLoginNavService(p), 
                    CreateBackOnlyNavService(p),
                    CreateAddWorkerNavService(p), 
                    p.GetRequiredService<UserContext>(),
                    p.GetRequiredService<DbService>()
                    );
            });
            services.AddTransient<DirectorPanelViewModel>(p =>
            {
                return new DirectorPanelViewModel(
                    CreateLoginNavService(p),
                    CreateWorkerRecordNavService(p),
                    CreateMaterialListNavService(p)
                    );
            });
            services.AddTransient<AddWorkerViewModel>(p =>
            {
                return new AddWorkerViewModel(
                    CreateLoginNavService(p), 
                    CreateBackOnlyNavService(p), 
                    p.GetRequiredService<DbService>()
                    );
            });
            services.AddTransient<MaterialListViewModel>(p =>
            {
                return new MaterialListViewModel(
                    CreateLoginNavService(p),
                    CreateBackOnlyNavService(p),
                    p.GetRequiredService<UserContext>(),
                    p.GetRequiredService<DbService>()
                    );
            });

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

        private INavService CreateDirectorPanelNavService(IServiceProvider p) =>
            new MainNavService(
                p.GetRequiredService<MainNavContext>(),
                p.GetRequiredService<DirectorPanelViewModel>
                );
        private INavService CreateWorkerRecordNavService(IServiceProvider p) =>
            new MainNavService(
                p.GetRequiredService<MainNavContext>(),
                p.GetRequiredService<WorkerRecordViewModel>
                );
        private INavService CreateMaterialListNavService(IServiceProvider p) =>
            new MainNavService(
                p.GetRequiredService<MainNavContext>(),
                p.GetRequiredService<MaterialListViewModel>
                );

        private INavService CreateClientNavService(IServiceProvider p) =>
            new MainNavService(
                p.GetRequiredService<MainNavContext>(),
                p.GetRequiredService<ClientViewModel>
                );
        private INavService CreateAddWorkerNavService(IServiceProvider p) =>
            new MainNavService(p.GetRequiredService<MainNavContext>(), p.GetRequiredService<AddWorkerViewModel>);
        private INavService CreateBackOnlyNavService(IServiceProvider p) =>
            new MainNavService(p.GetRequiredService<MainNavContext>());
    }
}
