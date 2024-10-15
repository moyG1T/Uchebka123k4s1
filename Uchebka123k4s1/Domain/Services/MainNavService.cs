using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uchebka123k4s1.Domain.Contexts;
using Uchebka123k4s1.Domain.IServices;
using Uchebka123k4s1.Domain.Utilities;

namespace Uchebka123k4s1.Domain.Services
{
    public class MainNavService : INavService
    {
        private readonly MainNavContext _navContext;
        private readonly Func<ViewModel> _createViewModel;

        public MainNavService(MainNavContext mainNavContext, Func<ViewModel> createViewModel = null)
        {
            _navContext = mainNavContext;
            _createViewModel = createViewModel;
        }

        public void GoBack()
        {
            var vmToDispose = _navContext.History?.Pop();
            vmToDispose?.Dispose();

            _navContext.CurrentViewModel = _navContext.History?.Peek();
        }
        public void Navigate()
        {
            if (_createViewModel != null)
            {
                var vm = _createViewModel();

                _navContext.History.Push(vm);

                _navContext.CurrentViewModel = vm;
            }
        }
        public void NavigateAndDispose()
        {
            if (_createViewModel != null)
            {
                foreach (var item in _navContext.History)
                {
                    item?.Dispose();
                }
                _navContext.History?.Clear();

                var vm = _createViewModel();
                _navContext.History.Push(vm);
                _navContext.CurrentViewModel = vm;
            }
        }
    }
}
