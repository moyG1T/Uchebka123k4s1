using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows.Input;
using Uchebka123k4s1.Data.Local.IServices;
using Uchebka123k4s1.Data.Services;
using Uchebka123k4s1.Domain.Commands;
using Uchebka123k4s1.Domain.IServices;
using Uchebka123k4s1.Domain.Utilities;

namespace Uchebka123k4s1.ViewModels
{
    public class LoginViewModel : ViewModel
    {
        private readonly INavService _registraionNavService;
        private readonly INavService _workerRecordNavService;
        private readonly IEntryService _entryService;
        private readonly DbService _dbService;

        private string _loginText;
        public string LoginText
        {
            get => _loginText;
            set { _loginText = value; OnPropertyChanged(); }
        }

        private string _passwordText;
        public string PasswordText
        {
            get => _passwordText;
            set { _passwordText = value; OnPropertyChanged(); }
        }

        private string _error;
        public string Error
        {
            get => _error;
            set { _error = value; OnPropertyChanged(); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); OnPropertyChanged(nameof(LoginButtonAbility)); }
        }

        public bool LoginButtonAbility => !IsLoading;

        private bool _rememberMe;
        public bool RememberMe
        {
            get => _rememberMe;
            set { _rememberMe = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand GoToRegistrationCommand { get; }
        public ICommand RememberMeCommand { get; }

        public LoginViewModel(
            INavService registraionNavService, 
            INavService workerRecordNavService, 
            IEntryService entryService,
            DbService dbService)
        {
            _registraionNavService = registraionNavService;
            _workerRecordNavService = workerRecordNavService;
            _entryService = entryService;
            _dbService = dbService;

            LoginCommand = new RelayAsyncCommand(LoginAsync);
            GoToRegistrationCommand = new NavigateCommand(registraionNavService);
            RememberMeCommand = new RelayCommand(ReverseRemberMe);
        }

        private void ReverseRemberMe()
        {
            RememberMe = !RememberMe;
        }

        private async Task LoginAsync()
        {
            IsLoading = true;

            var user = await _dbService
                .db
                .User
                .FirstOrDefaultAsync(u => u.Login == LoginText && u.Password == PasswordText);

            if (user is null)
            {
                Error = "Неверные данные";
            }
            else
            {
                if (RememberMe)
                {
                    _entryService.Write(user.Id.ToString());
                }

                switch (user.RoleId)
                {
                    case 1:
                        _workerRecordNavService.NavigateAndDispose();
                        break;
                    default:
                        break;
                }
            }

            IsLoading = false;
        }

        public override void Dispose()
        {

        }
    }
}
