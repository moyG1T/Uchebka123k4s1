using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Uchebka123k4s1.Data.Services;
using Uchebka123k4s1.Domain.Commands;
using Uchebka123k4s1.Domain.Contexts;
using Uchebka123k4s1.Domain.IServices;
using Uchebka123k4s1.Domain.Utilities;

namespace Uchebka123k4s1.ViewModels
{
    public class OrderControlViewModel : ViewModel
    {
        private readonly OrderContext _orderContext;
        private readonly DbService _dbService;

        public ICommand LogoutCommand { get; }
        public ICommand GoBackCommand { get; }

        public OrderControlViewModel(INavService logout, INavService back, OrderContext orderContext, DbService dbService)
        {
            LogoutCommand = new NavigateAndDisposeCommand(logout);
            GoBackCommand = new GoBackCommand(back);

            _orderContext = orderContext;
            _dbService = dbService;
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
