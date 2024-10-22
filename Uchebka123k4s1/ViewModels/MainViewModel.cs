using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows;
using Uchebka123k4s1.Data.Local.IServices;
using Uchebka123k4s1.Data.Remote.SqlModel;
using Uchebka123k4s1.Data.Services;
using Uchebka123k4s1.Domain.Contexts;
using Uchebka123k4s1.Domain.IServices;
using Uchebka123k4s1.Domain.Utilities;

namespace Uchebka123k4s1.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private readonly INavService _loginNavService;
        private readonly INavService _director;
        private readonly INavService _client;
        private readonly INavService _manager;
        private readonly INavService _master;
        private readonly INavService _ctor;
        private readonly IEntryService _entryService;
        private readonly UserContext _userContext;
        private readonly DbService _dbService;
        private readonly MainNavContext _mainNavContext;

        public ViewModel CurrentViewModel => _mainNavContext.CurrentViewModel;

        public MainViewModel(
            INavService loginNavService,
            INavService director,
            INavService client,
            INavService manager,
            INavService master,
            INavService ctor,
            IEntryService entryService,
            UserContext userContext,
            DbService dbService,
            MainNavContext mainNavContext)
        {
            _director = director;
            _client = client;
            _manager = manager;
            _master = master;
            _ctor = ctor;

            _loginNavService = loginNavService;
            _entryService = entryService;
            _userContext = userContext;
            _dbService = dbService;
            _mainNavContext = mainNavContext;

            _mainNavContext.ViewModelChanged += OnViewModelChanged;

            Task.Run(AutoLoginAsync);
        }

        private async Task AutoLoginAsync()
        {
            if (_entryService.Exists())
            {
                if (_entryService.Read(out string id))
                {
                    var userId = int.Parse(id);
                    var user = await _dbService
                        .db
                        .User
                        .FirstOrDefaultAsync(u => u.Id == userId);

                    if (user is null)
                    {
                        _loginNavService.Navigate();
                    }
                    else
                    {
                        _userContext.User = user;
                        switch (user.RoleId)
                        {
                            case 1:
                                _director.NavigateAndDispose();
                                break;
                            case 2:
                                _ctor.NavigateAndDispose();
                                break;
                            case 3:
                                _manager.NavigateAndDispose();
                                break;
                            case 4:
                                _master.NavigateAndDispose();
                                break;
                            case 5:
                                _client.NavigateAndDispose();
                                break;
                            default:
                                _loginNavService.NavigateAndDispose();
                                break;
                        }
                    }
                }
                else
                {
                    _loginNavService.Navigate();
                }
            }
            else
            {
                _loginNavService.Navigate();
            }
        }

        private void OnViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        public override void Dispose()
        {
            _mainNavContext.ViewModelChanged -= OnViewModelChanged;
        }
    }
}
