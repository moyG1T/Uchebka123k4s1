using System;
using System.Windows.Input;
using Uchebka123k4s1.Domain.Commands;
using Uchebka123k4s1.Domain.IServices;
using Uchebka123k4s1.Domain.Utilities;

namespace Uchebka123k4s1.ViewModels
{
    public class DirectorPanelViewModel : ViewModel
    {
        public ICommand LogoutCommand { get; }
        public ICommand WorkersRecordCommand { get; }
        public ICommand MaterialRecordCommand { get; }

        public DirectorPanelViewModel(INavService logout, INavService workersRecord, INavService materialRecord)
        {
            LogoutCommand = new NavigateAndDisposeCommand(logout);
            WorkersRecordCommand = new NavigateCommand(workersRecord);
            MaterialRecordCommand = new NavigateCommand(materialRecord);
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
