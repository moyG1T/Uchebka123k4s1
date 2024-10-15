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
        private readonly IEntryService _entryService;
        private readonly DbService _dbService;
        private readonly MainNavContext _mainNavContext;

        public ViewModel CurrentViewModel => _mainNavContext.CurrentViewModel;

        public MainViewModel(INavService loginNavService, IEntryService entryService, DbService dbService, MainNavContext mainNavContext)
        {
            _loginNavService = loginNavService;
            _entryService = entryService;
            _dbService = dbService;
            _mainNavContext = mainNavContext;

            _mainNavContext.ViewModelChanged += OnViewModelChanged;

            Task.Run(AutoLoginAsync);
        }

        private async Task AutoLoginAsync()
        {
            try
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
                        switch (user.RoleId)
                        {
                            case 1:
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch
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
