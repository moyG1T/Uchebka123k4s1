using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Uchebka123k4s1.Domain.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Action<object> _executeWithParam;

        public RelayCommand(Action action)
        {
            _execute = action;
        }
        public RelayCommand(Action<object> action)
        {
            _executeWithParam = action;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _execute?.Invoke();
            _executeWithParam?.Invoke(parameter);
        }
        public event EventHandler CanExecuteChanged;
    }
}
