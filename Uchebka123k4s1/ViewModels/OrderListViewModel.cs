using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Uchebka123k4s1.Data.Remote.SqlModel;
using Uchebka123k4s1.Data.Services;
using Uchebka123k4s1.Domain.Commands;
using Uchebka123k4s1.Domain.Contexts;
using Uchebka123k4s1.Domain.IServices;
using Uchebka123k4s1.Domain.Utilities;

namespace Uchebka123k4s1.ViewModels
{
    public class OrderListViewModel : ViewModel
    {
        private OrderState selectedState;

        private ObservableCollection<Order> _orders;
        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set
            {
                _orders = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ResultOrders));
            }
        }

        public List<Order> ResultOrders => SelectedState is null ?
            Orders.ToList() : Orders.Where(it => it.OrderState.Id == SelectedState.Id).ToList();

        public List<OrderState> States { get; set; }
        public OrderState SelectedState
        {
            get => selectedState;
            set
            {
                selectedState = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ResultOrders));
            }
        }

        private readonly DbService _dbService;
        private readonly UserContext _userContext;

        public bool CanInteract => _userContext.User.RoleId == 3;

        public ICommand LogoutCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand RemoveFilterCommand { get; }

        public OrderListViewModel(
            INavService logout,
            INavService goBack,
            DbService dbService,
            UserContext userContext
            )
        {
            _dbService = dbService;
            _userContext = userContext;

            LogoutCommand = new NavigateAndDisposeCommand(logout);
            GoBackCommand = new GoBackCommand(goBack);
            RemoveFilterCommand = new RelayCommand(RemoveFilter);

            Task.Run(LoadContent);
        }

        private async Task LoadContent()
        {
            var orders = await _dbService.db.Order.ToListAsync();
            var states = await _dbService.db.OrderState.ToListAsync();

            Orders = new ObservableCollection<Order>(orders);

            States = states;
            OnPropertyChanged(nameof(States));
        }

        private void RemoveFilter()
        {
            SelectedState = null;
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
