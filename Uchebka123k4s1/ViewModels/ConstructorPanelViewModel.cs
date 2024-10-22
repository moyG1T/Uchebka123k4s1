using System;
using System.Windows.Input;
using Uchebka123k4s1.Domain.Commands;
using Uchebka123k4s1.Domain.IServices;
using Uchebka123k4s1.Domain.Utilities;

namespace Uchebka123k4s1.ViewModels
{
    public class ConstructorPanelViewModel : ViewModel
    {
        public ICommand LogoutCommand { get; }
        public ICommand OrderListCommand { get; }

        public ConstructorPanelViewModel(
            INavService logout,
            INavService orderList
            )
        {
            LogoutCommand = new NavigateAndDisposeCommand(logout);
            OrderListCommand = new NavigateCommand(orderList);
        }
        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
