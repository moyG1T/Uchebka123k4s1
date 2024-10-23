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
using Microsoft.VisualBasic;

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

        public bool IsManager => _userContext.User.RoleId == 3;

        public ICommand LogoutCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand RemoveFilterCommand { get; }

        public ICommand AddOrderCommand { get; }

        public ICommand SetManagerCommand { get; }
        public ICommand RemoveNewOrderCommand { get; }
        public ICommand AcceptOrderCommand { get; }
        public ICommand DeclineOrderCommand { get; }
        public ICommand SetOrderReadyCommand { get; }
        public ICommand CloseOrderCommand { get; }

        public OrderListViewModel(
            INavService logout,
            INavService goBack,
            INavService addOrder,
            DbService dbService,
            UserContext userContext
            )
        {
            _dbService = dbService;
            _userContext = userContext;

            LogoutCommand = new NavigateAndDisposeCommand(logout);
            GoBackCommand = new GoBackCommand(goBack);
            RemoveFilterCommand = new RelayCommand(RemoveFilter);

            switch (_userContext.User.RoleId)
            {
                case 1:
                    Task.Run(LoadDirectorOrders);
                    break; 
                case 3:
                    SetManagerCommand = new RelayAsyncCommand(SetManagerToOrder);
                    RemoveNewOrderCommand = new RelayAsyncCommand(RemoveNewOrder);
                    AcceptOrderCommand = new RelayAsyncCommand(AcceptOrder);
                    DeclineOrderCommand = new RelayAsyncCommand(DeclineOrder);
                    SetOrderReadyCommand = new RelayAsyncCommand(SetOrderReady);
                    CloseOrderCommand = new RelayAsyncCommand(CloseOrder);

                    AddOrderCommand = new NavigateCommand(addOrder);

                    Task.Run(LoadManagerOrders);
                    break; // манагер
                default:
                    break;
            }
        }

        private async Task LoadManagerOrders()
        {
            var orders = await _dbService
                .db
                .Order
                .Where(it => it.ManagerId == _userContext.User.Id || it.StateId == 1)
                .ToListAsync();

            Orders = new ObservableCollection<Order>(orders);

            var states = await _dbService.db.OrderState.ToListAsync();
            States = states;
            OnPropertyChanged(nameof(States));
        }

        private async Task LoadDirectorOrders()
        {
            var orders = await _dbService
                .db
                .Order
                .ToListAsync();

            Orders = new ObservableCollection<Order>(orders);

            var states = await _dbService.db.OrderState.ToListAsync();
            States = states;
            OnPropertyChanged(nameof(States));
        }

        private async Task SetManagerToOrder(object param)
        {
            var order = param as Order;

            order.OrderState = States.FirstOrDefault(it => it.Id == 3);
            order.User = _userContext.User;

            await _dbService.db.SaveChangesAsync();
        }
        private async Task RemoveNewOrder(object param)
        {
            var order = param as Order;

            _dbService.db.Order.Remove(order);
            await _dbService.db.SaveChangesAsync();

            Orders.Remove(order);
            OnPropertyChanged(nameof(ResultOrders));
        }
        private async Task AcceptOrder(object param)
        {
            var order = param as Order;

            order.OrderState = States.FirstOrDefault(it => it.Id == 5);
            await _dbService.db.SaveChangesAsync();
        }
        private async Task DeclineOrder(object param)
        {
            var order = param as Order;

            order.OrderState = States.FirstOrDefault(it => it.Id == 2);
            await _dbService.db.SaveChangesAsync();
        }
        private async Task SetOrderReady(object param)
        {
            var order = param as Order;

            order.OrderState = States.FirstOrDefault(it => it.Id == 6);
            await _dbService.db.SaveChangesAsync();
        }
        private async Task CloseOrder(object param)
        {
            var order = param as Order;

            order.OrderState = States.FirstOrDefault(it => it.Id == 9);
            await _dbService.db.SaveChangesAsync();
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
