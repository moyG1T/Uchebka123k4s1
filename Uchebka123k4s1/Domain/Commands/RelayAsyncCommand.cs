using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Uchebka123k4s1.Domain.Commands
{
    public class RelayAsyncCommand : ICommand
    {
        private readonly Func<Task> _execute;

        public RelayAsyncCommand(Func<Task> action)
        {
            _execute = action;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _execute();
        }
        public event EventHandler CanExecuteChanged;
    }
}
