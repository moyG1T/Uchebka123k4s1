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

        private readonly INavService _interactOrder;
        private readonly INavService _controlOrder;
        private readonly OrderContext _orderContext;
        private readonly DbService _dbService;
        private readonly UserContext _userContext;

        public bool IsCtor => _userContext.User.RoleId == 2;
        public bool IsManager => _userContext.User.RoleId == 3;
        public bool IsDungeonMaster => _userContext.User.RoleId == 4;
        public bool IsClient => _userContext.User.RoleId == 5;

        public bool CanAddOrder => IsManager || IsClient;

        public ICommand LogoutCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand RemoveFilterCommand { get; }

        public ICommand AddOrderCommand { get; }
        public ICommand EditOrderCommand { get; }

        public ICommand SetManagerCommand { get; }
        public ICommand RemoveNewOrderCommand { get; }
        public ICommand AcceptOrderCommand { get; }
        public ICommand DeclineOrderCommand { get; }
        public ICommand SetOrderToProdCommand { get; }
        public ICommand CloseOrderCommand { get; }

        public ICommand ConfirmOrderCommand { get; }

        public ICommand ControlOrderCommand { get; }
        public ICommand SetOrderReadyCommand { get; }

        public OrderListViewModel(
            INavService logout,
            INavService goBack,
            INavService interactOrder,
            INavService controlOrder,
            OrderContext orderContext,
            DbService dbService,
            UserContext userContext
            )
        {
            _interactOrder = interactOrder;
            _controlOrder = controlOrder;
            _orderContext = orderContext;
            _dbService = dbService;
            _userContext = userContext;

            _orderContext.OrderAdded += AddOrder;

            LogoutCommand = new NavigateAndDisposeCommand(logout);
            GoBackCommand = new GoBackCommand(goBack);
            RemoveFilterCommand = new RelayCommand(RemoveFilter);

            AddOrderCommand = new NavigateCommand(interactOrder);
            EditOrderCommand = new RelayCommand(EditOrder);

            switch (_userContext.User.RoleId)
            {
                case 1:
                    Task.Run(LoadDirectorOrders);
                    break;
                case 2:
                    ConfirmOrderCommand = new RelayAsyncCommand(ConfirmOrder);

                    Task.Run(LoadCtorOrders);
                    break;
                case 3:
                    SetManagerCommand = new RelayAsyncCommand(SetManagerToOrder);
                    RemoveNewOrderCommand = new RelayAsyncCommand(RemoveNewOrder);
                    AcceptOrderCommand = new RelayAsyncCommand(AcceptOrder);
                    DeclineOrderCommand = new RelayAsyncCommand(DeclineOrder);
                    SetOrderToProdCommand = new RelayAsyncCommand(SetOrderToProd);
                    CloseOrderCommand = new RelayAsyncCommand(CloseOrder);

                    AddOrderCommand = new NavigateCommand(interactOrder);

                    Task.Run(LoadManagerOrders);
                    break;
                case 4:
                    ControlOrderCommand = new RelayCommand(ControlOrder);
                    SetOrderReadyCommand = new RelayAsyncCommand(SetOrderReady);

                    Task.Run(LoadDungeonMasterOrders);
                    break;
                case 5:
                    AddOrderCommand = new NavigateCommand(interactOrder);
                    RemoveNewOrderCommand = new RelayAsyncCommand(RemoveNewOrder);

                    Task.Run(LoadClientOrders);
                    break;
                default:
                    break;
            }
        }

        private async Task LoadManagerOrders()
        {
            var orders = await _dbService
                .Order
                .Where(it => it.ManagerId == _userContext.User.Id || it.StateId == 1)
                .ToListAsync();

            Orders = new ObservableCollection<Order>(orders);

            var states = await _dbService.OrderState.ToListAsync();
            States = states;
            OnPropertyChanged(nameof(States));
        }
        private async Task LoadDirectorOrders()
        {
            var orders = await _dbService
                .Order
                .ToListAsync();

            Orders = new ObservableCollection<Order>(orders);

            var states = await _dbService.OrderState.ToListAsync();
            States = states;
            OnPropertyChanged(nameof(States));
        }
        private async Task LoadClientOrders()
        {
            var orders = await _dbService
                .Order
                .Where(it => it.ClientId == _userContext.User.Id)
                .ToListAsync();

            Orders = new ObservableCollection<Order>(orders);

            var states = await _dbService.OrderState.ToListAsync();
            States = states;
            OnPropertyChanged(nameof(States));
        }
        private async Task LoadDungeonMasterOrders()
        {
            var orders = await _dbService
                .Order
                .Where(it => it.StateId == 6 || it.StateId == 7)
                .ToListAsync();

            Orders = new ObservableCollection<Order>(orders);

            var states = await _dbService.OrderState.ToListAsync();
            States = states;
            OnPropertyChanged(nameof(States));
        }
        private async Task LoadCtorOrders()
        {
            var orders = await _dbService
                .Order
                .Where(it => it.StateId == 3)
                .ToListAsync();

            Orders = new ObservableCollection<Order>(orders);

            var states = await _dbService.OrderState.ToListAsync();
            States = states;
            OnPropertyChanged(nameof(States));
        }

        private async Task SetManagerToOrder(object param)
        {
            var order = param as Order;

            order.OrderState = States.FirstOrDefault(it => it.Id == 3);
            order.User = _userContext.User;

            await _dbService.SaveChangesAsync();
        }
        private async Task RemoveNewOrder(object param)
        {
            var order = param as Order;

            _dbService.Order.Remove(order);
            await _dbService.SaveChangesAsync();

            Orders.Remove(order);
            OnPropertyChanged(nameof(ResultOrders));
        }
        private async Task AcceptOrder(object param)
        {
            var order = param as Order;

            order.OrderState = States.FirstOrDefault(it => it.Id == 5);
            await _dbService.SaveChangesAsync();
        }
        private async Task DeclineOrder(object param)
        {
            var order = param as Order;

            order.OrderState = States.FirstOrDefault(it => it.Id == 2);
            await _dbService.SaveChangesAsync();
        }
        private async Task SetOrderToProd(object param)
        {
            var order = param as Order;

            order.OrderState = States.FirstOrDefault(it => it.Id == 6);
            await _dbService.SaveChangesAsync();
        }
        private async Task CloseOrder(object param)
        {
            var order = param as Order;

            order.OrderState = States.FirstOrDefault(it => it.Id == 9);
            await _dbService.SaveChangesAsync();
        }
        private async Task ConfirmOrder(object param)
        {
            var order = param as Order;

            order.OrderState = States.FirstOrDefault(it => it.Id == 4);
            await _dbService.SaveChangesAsync();
        }
        private void ControlOrder(object param)
        {
            var order = param as Order;

            //order.OrderState = States.FirstOrDefault(it => it.Id == 7);
            //await _dbService.SaveChangesAsync();

            _orderContext.SelectedOrder = order;

            _controlOrder.Navigate();
        }
        private async Task SetOrderReady(object param)
        {
            var order = param as Order;

            order.OrderState = States.FirstOrDefault(it => it.Id == 8);
            await _dbService.SaveChangesAsync();
        }

        private void EditOrder(object param)
        {
            if (param is Order order)
            {
                _orderContext.SelectedOrder = order;
                _interactOrder.Navigate();
            }
        }

        private void AddOrder(Order order)
        {
            Orders.Add(order);
            OnPropertyChanged(nameof(ResultOrders));
        }

        private void RemoveFilter()
        {
            SelectedState = null;
        }

        public override void Dispose()
        {
            _orderContext.OrderAdded -= AddOrder;

            GC.SuppressFinalize(this);
        }
    }
}
