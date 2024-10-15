using System;
using System.Windows.Input;
using Uchebka123k4s1.Domain.Commands;
using Uchebka123k4s1.Domain.IServices;
using Uchebka123k4s1.Domain.Utilities;

namespace Uchebka123k4s1.ViewModels
{
    public class RegistrationViewModel : ViewModel
    {
        public ICommand GoBackCommand { get; }

        public RegistrationViewModel(INavService clientNavService)
        {
            GoBackCommand = new GoBackCommand(clientNavService);
        }

        public override void Dispose()
        {

        }
    }
}
