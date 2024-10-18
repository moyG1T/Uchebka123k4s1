using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Uchebka123k4s1.Data.Local.IServices;
using Uchebka123k4s1.Data.Remote.SqlModel;
using Uchebka123k4s1.Data.Services;
using Uchebka123k4s1.Domain.Commands;
using Uchebka123k4s1.Domain.Contexts;
using Uchebka123k4s1.Domain.IServices;
using Uchebka123k4s1.Domain.Utilities;

namespace Uchebka123k4s1.ViewModels
{
    public class RegistrationViewModel : ViewModel
    {
        public ICommand GoBackCommand { get; }
        public ICommand ConfirmCommand { get; }

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
            set
            {
                _passwordText = value;
                OnPropertyChanged();
            }
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
        private readonly INavService _clientNavService;
        private readonly UserContext _userContext;
        private readonly IEntryService _entryService;
        private readonly DbService _dbService;

        public bool RememberMe
        {
            get => _rememberMe;
            set { _rememberMe = value; OnPropertyChanged(); }
        }

        public RegistrationViewModel(
            INavService clientNavService,
            UserContext userContext,
            IEntryService entryService, 
            DbService dbService)
        {
            _clientNavService = clientNavService;
            _userContext = userContext;
            _entryService = entryService;
            _dbService = dbService;

            GoBackCommand = new GoBackCommand(_clientNavService);
            ConfirmCommand = new RelayAsyncCommand(RegistrateUser);
        }

        private async Task RegistrateUser()
        {
            if (string.IsNullOrEmpty(LoginText) || string.IsNullOrEmpty(PasswordText))
            {
                Error = "Пустые поля";
                return;
            }

            switch (PasswordText.Length)
            {
                case int n when n < 4:
                    Error = "Пароль слишком короткий";
                    return;
                case int n when n > 16:
                    Error = "Пароль слишком длинный";
                    return;
                default:
                    break;
            }

            bool containsExplicitChars = _passwordText.Contains("*")
                || _passwordText.Contains("{")
                || _passwordText.Contains("}")
                || _passwordText.Contains("|")
                || _passwordText.Contains("+");

            if (containsExplicitChars)
            {
                Error = "Запрещенные знаки *{}|+";
                return;
            }

            if (!_passwordText.Any(char.IsUpper))
            {
                Error = "Должны быть заглавные буквы";
                return;
            }

            if (!_passwordText.Any(char.IsDigit))
            {
                Error = "Должны быть цифры";
                return;
            }

            var user = new User
            {
                Login = LoginText,
                Password = _passwordText,
                RoleId = 5,
            };

            if (await _dbService.db.User.FirstOrDefaultAsync(u => u.Login == user.Login) is null)
            {
                _dbService.db.User.Add(user);
                await _dbService.db.SaveChangesAsync();

                if (RememberMe)
                {
                    _entryService.Write(user.Id.ToString());
                }

                _userContext.User = user;

                _clientNavService.NavigateAndDispose();
            }
            else
            {
                Error = "Такой пользователь уже существует";
            }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
