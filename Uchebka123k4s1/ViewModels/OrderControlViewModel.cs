using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Uchebka123k4s1.Data.Remote.SqlModel;
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

        private readonly bool hadControllers = false;

        public Controller SelectedController { get; set; }
        public ObservableCollection<Controller> Controllers { get; set; } = new ObservableCollection<Controller>();
        public ObservableCollection<OrderController> OrderControllers { get; set; } = new ObservableCollection<OrderController>();

        public ICommand LogoutCommand { get; }
        public ICommand GoBackCommand { get; }

        public ICommand AddControlCommand { get; }
        public ICommand RemoveControlCommand { get; }
        public ICommand SaveChangesCommand { get; }
        public ICommand SetReadyCommand { get; }

        public OrderControlViewModel(INavService logout, INavService back, OrderContext orderContext, DbService dbService)
        {
            LogoutCommand = new NavigateAndDisposeCommand(logout);
            GoBackCommand = new GoBackCommand(back);
            AddControlCommand = new RelayCommand(AddControlToOrder);
            RemoveControlCommand = new RelayCommand(RemoveControlFromOrder);
            SaveChangesCommand = new RelayAsyncCommand(SaveChangesAsync);
            SetReadyCommand = new RelayAsyncCommand(SetReadyAsync);

            _orderContext = orderContext;
            _dbService = dbService;

            if (_orderContext.SelectedOrder != null)
            {
                if (_orderContext.SelectedOrder.OrderController.Count > 0)
                {
                    hadControllers = true;
                    OrderControllers = new ObservableCollection<OrderController>(_orderContext.SelectedOrder.OrderController);
                }
            }

            Task.Run(LoadContent);
        }

        private async Task LoadContent()
        {
            var list = await _dbService.Controller.ToListAsync();

            if (OrderControllers.Count > 0)
            {
                foreach (var item in OrderControllers)
                {
                    list.Remove(item.Controller);
                }
            }

            Controllers = new ObservableCollection<Controller>(list);
            OnPropertyChanged(nameof(Controllers));
        }

        private void AddControlToOrder()
        {
            if (SelectedController != null)
            {
                var controller = new OrderController
                {
                    Controller = SelectedController,
                    Comm = "",
                    IsFine = false,
                    Order = _orderContext.SelectedOrder,
                };
                OrderControllers.Add(controller);
                Controllers.Remove(SelectedController);
            }
        }
        private void RemoveControlFromOrder(object param)
        {
            if (param is OrderController controller)
            {
                Controllers.Add(controller.Controller);
                OrderControllers.Remove(controller);
            }
        }

        private async Task SaveChangesAsync()
        {
            if (OrderControllers.Count == 0)
            {
                MessageBox.Show("Список пуст");
                return;
            }

            if (!hadControllers)
            {
                _dbService.OrderController.AddRange(OrderControllers);
                _orderContext.SelectedOrder.StateId = 7;
            }

            await _dbService.SaveChangesAsync();

            GoBackCommand.Execute(null);
            MessageBox.Show("Сохранено");
        }
        private async Task SetReadyAsync()
        {
            if (_orderContext.SelectedOrder.OrderController.Count == 0)
            {
                MessageBox.Show("Список пуст");
                return;
            }
            if (_orderContext.SelectedOrder.OrderController.Any(it => it.IsFine == false))
            {
                MessageBox.Show("Выполнено не все");
                return;
            }

            _orderContext.SelectedOrder.StateId = 8;
            await _dbService.SaveChangesAsync();

            GoBackCommand.Execute(null);
            MessageBox.Show("Сохранено");
        }

        public override void Dispose()
        {
            _orderContext.SelectedOrder = null;
            GC.SuppressFinalize(this);
        }
    }
}
