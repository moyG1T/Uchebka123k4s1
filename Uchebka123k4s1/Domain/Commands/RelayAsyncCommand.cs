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
        private readonly Func<object, Task> _executeWithParams;

        public RelayAsyncCommand(Func<Task> action)
        {
            _execute = action;
        }
        public RelayAsyncCommand(Func<object, Task> action)
        {
            _executeWithParams = action;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if (_execute != null)
            {
                _execute();
            }
            if (_executeWithParams != null)
            {
                _executeWithParams(parameter);
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}
