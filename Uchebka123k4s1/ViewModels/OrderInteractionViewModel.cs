using System;
using System.Windows.Input;
using Uchebka123k4s1.Data.Services;
using Uchebka123k4s1.Domain.Commands;
using Uchebka123k4s1.Domain.Contexts;
using Uchebka123k4s1.Domain.IServices;
using Uchebka123k4s1.Domain.Utilities;

namespace Uchebka123k4s1.ViewModels
{
    public class OrderInteractionViewModel : ViewModel
    {
        public ICommand LogoutCommand { get; }
        public ICommand GoBackCommand { get; }

        public OrderInteractionViewModel(
            INavService logout, 
            INavService goBack,
            DbService dbService,
            UserContext userContext
            )
        {
            LogoutCommand = new NavigateAndDisposeCommand(logout);
            GoBackCommand = new GoBackCommand(goBack);
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
