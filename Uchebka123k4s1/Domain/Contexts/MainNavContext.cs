using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uchebka123k4s1.Domain.Utilities;

namespace Uchebka123k4s1.Domain.Contexts
{
    public class MainNavContext
    {
        public Stack<ViewModel> History = new Stack<ViewModel>();

        private ViewModel _currentViewModel;
        public ViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                ViewModelChanged?.Invoke();
            }
        }

        public event Action ViewModelChanged;
    }
}
