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
            services.AddSingleton<MaterialContext>();
            services.AddSingleton<HardwareContext>();

            //services.AddSingleton<IDbService, DbService>();
            services.AddSingleton<DbService>();
            services.AddSingleton<IEntryService, EntryService>();

            services.AddSingleton<MainViewModel>(p =>
            {
                return new MainViewModel(
                    CreateLoginNavService(p),
                    CreateDirectorPanelNavService(p),
                    CreateClientPanelNavService(p),
                    CreateManagerPanelNavService(p),
                    CreateMasterPanelNavService(p),
                    CreateCtorPanelNavService(p),
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
                    CreateClientPanelNavService(p),
                    CreateManagerPanelNavService(p),
                    CreateMasterPanelNavService(p),
                    CreateCtorPanelNavService(p),
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
                    CreateMaterialInteractionNavService(p),
                    p.GetRequiredService<UserContext>(),
                    p.GetRequiredService<MaterialContext>(),
                    p.GetRequiredService<DbService>()
                    );
            });
            services.AddTransient<MaterialInteractionViewModel>(p =>
            {
                return new MaterialInteractionViewModel(
                    CreateLoginNavService(p),
                    CreateBackOnlyNavService(p),
                    p.GetRequiredService<MaterialContext>(),
                    p.GetRequiredService<DbService>()
                    );
            });
            services.AddTransient<HardwareListViewModel>(p =>
            {
                return new HardwareListViewModel(
                    CreateLoginNavService(p),
                    CreateBackOnlyNavService(p),
                    CreateHardwareInteractionNavService(p),
                    p.GetRequiredService<UserContext>(),
                    p.GetRequiredService<HardwareContext>(),
                    p.GetRequiredService<DbService>()
                    );
            });
            services.AddTransient<HardwareInteractionViewModel>(p =>
            {
                return new HardwareInteractionViewModel(
                    CreateLoginNavService(p),
                    CreateBackOnlyNavService(p),
                    p.GetRequiredService<HardwareContext>(),
                    p.GetRequiredService<DbService>()
                    );
            });
            services.AddTransient<OrderListViewModel>(p =>
            {
                return new OrderListViewModel(
                    CreateLoginNavService(p),
                    CreateBackOnlyNavService(p),
                    CreateOrderInteractionNavService(p),
                    p.GetRequiredService<DbService>(),
                    p.GetRequiredService<UserContext>()
                    );
            });
            services.AddTransient<OrderInteractionViewModel>(p =>
            {
                return new OrderInteractionViewModel(
                    CreateLoginNavService(p),
                    CreateBackOnlyNavService(p),
                    p.GetRequiredService<DbService>(),
                    p.GetRequiredService<UserContext>()
                    );
            });

            services.AddTransient<DirectorPanelViewModel>(p =>
            {
                return new DirectorPanelViewModel(
                    CreateLoginNavService(p),
                    CreateWorkerRecordNavService(p),
                    CreateMaterialListNavService(p),
                    CreateHardwareListNavService(p),
                    CreateOrderListNavService(p)
                    );
            });
            services.AddTransient<ClientPanelViewModel>(p =>
            {
                return new ClientPanelViewModel(
                    CreateLoginNavService(p),
                    CreateOrderListNavService(p));
            });
            services.AddTransient<ManagerPanelViewModel>(p =>
            {
                return new ManagerPanelViewModel(
                    CreateLoginNavService(p),
                    CreateOrderListNavService(p));
            });
            services.AddTransient<MasterPanelViewModel>(p =>
            {
                return new MasterPanelViewModel(
                    CreateLoginNavService(p),
                    CreateOrderListNavService(p));
            });
            services.AddTransient<ConstructorPanelViewModel>(p =>
            {
                return new ConstructorPanelViewModel(
                    CreateLoginNavService(p),
                    CreateOrderListNavService(p));
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
        private INavService CreateClientPanelNavService(IServiceProvider p) =>
            new MainNavService(
                p.GetRequiredService<MainNavContext>(),
                p.GetRequiredService<ClientPanelViewModel>
                );
        private INavService CreateManagerPanelNavService(IServiceProvider p) =>
            new MainNavService(
                p.GetRequiredService<MainNavContext>(),
                p.GetRequiredService<ManagerPanelViewModel>
                );
        private INavService CreateMasterPanelNavService(IServiceProvider p) =>
            new MainNavService(
                p.GetRequiredService<MainNavContext>(),
                p.GetRequiredService<MasterPanelViewModel>
                );
        private INavService CreateCtorPanelNavService(IServiceProvider p) =>
            new MainNavService(
                p.GetRequiredService<MainNavContext>(),
                p.GetRequiredService<ConstructorPanelViewModel>
                );

        private INavService CreateWorkerRecordNavService(IServiceProvider p) =>
            new MainNavService(
                p.GetRequiredService<MainNavContext>(),
                p.GetRequiredService<WorkerRecordViewModel>
                );
        private INavService CreateMaterialInteractionNavService(IServiceProvider p) =>
            new MainNavService(
                p.GetRequiredService<MainNavContext>(),
                p.GetRequiredService<MaterialInteractionViewModel>
                );
        private INavService CreateMaterialListNavService(IServiceProvider p) =>
            new MainNavService(
                p.GetRequiredService<MainNavContext>(),
                p.GetRequiredService<MaterialListViewModel>
                );
        private INavService CreateHardwareListNavService(IServiceProvider p) =>
            new MainNavService(
                p.GetRequiredService<MainNavContext>(),
                p.GetRequiredService<HardwareListViewModel>
                );
        private INavService CreateHardwareInteractionNavService(IServiceProvider p) =>
            // :)
            new MainNavService(
                p.GetRequiredService<MainNavContext>(),
                p.GetRequiredService<HardwareInteractionViewModel>
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

        private INavService CreateOrderListNavService(IServiceProvider p) =>
            new MainNavService(p.GetRequiredService<MainNavContext>(), p.GetRequiredService<OrderListViewModel>);
        private INavService CreateOrderInteractionNavService(IServiceProvider p) =>
            new MainNavService(p.GetRequiredService<MainNavContext>(), p.GetRequiredService<OrderInteractionViewModel>);
    }
}
