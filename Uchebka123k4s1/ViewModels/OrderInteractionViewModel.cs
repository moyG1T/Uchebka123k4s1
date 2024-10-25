using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
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
    public class OrderInteractionViewModel : ViewModel
    {
        private readonly OrderContext _orderContext;
        private readonly DbService _dbService;
        private readonly UserContext _userContext;

        private bool _isEditForm = false;

        public bool IsManager => _userContext.User.RoleId == 3;
        public bool IsCreationForm => Order.StateId == 1;
        public bool IsOrderIdFormed => Order.Id != 0;
        public bool IsClientNotFormed => Order.Id == 0;

        public User SelectedClient { get; set; }
        public List<User> Clients { get; set; }
        public ObservableCollection<OrderImage> SchemasList { get; set; } = new ObservableCollection<OrderImage>();
        public Order Order { get; set; } = new Order { Id = 0, StateId = 1 };

        public ICommand LogoutCommand { get; }
        public ICommand GoBackCommand { get; }

        public ICommand OpenFileDialogCommand { get; }
        public ICommand RemoveSchemaCommand { get; }

        public ICommand SaveChangesCommand { get; }

        public OrderInteractionViewModel(
            INavService logout,
            INavService goBack,
            OrderContext orderContext,
            DbService dbService,
            UserContext userContext
            )
        {
            _orderContext = orderContext;
            _dbService = dbService;
            _userContext = userContext;

            if (_orderContext.SelectedOrder != null)
            {
                Order = _orderContext.SelectedOrder;
                _isEditForm = true;
            }

            LogoutCommand = new NavigateAndDisposeCommand(logout);
            GoBackCommand = new GoBackCommand(goBack);
            OpenFileDialogCommand = new RelayCommand(AddSchema);
            RemoveSchemaCommand = new RelayCommand(RemoveSchema);

            SaveChangesCommand = new RelayAsyncCommand(SaveChanges);

            if (_userContext.User.RoleId == 3)
            {
                Task.Run(LoadManagerContent);
            }
        }

        private async Task LoadManagerContent()
        {
            var clients = await _dbService.db.User.Where(it => it.RoleId == 5 && it.UserFullName.Count > 0).ToListAsync();

            Clients = clients;
            OnPropertyChanged(nameof(Clients));

            if (_isEditForm)
            {
                var schemas = await _dbService.db.OrderImage.Where(it => it.OrderId == Order.Id).ToListAsync();
                SchemasList = new ObservableCollection<OrderImage>(schemas);
            }
        }

        private async Task SaveChanges()
        {
            if (_isEditForm)
            {
                if (string.IsNullOrEmpty(Order.Title) || string.IsNullOrEmpty(Order.Cost.ToString()))
                {
                    MessageBox.Show("Пустые поля");
                    return;
                }
                await _dbService.db.SaveChangesAsync();
            }
            else
            {
                Order.StartDate = DateTime.Now;
                if (IsManager)
                {
                    if (string.IsNullOrEmpty(Order.Title)
                        || string.IsNullOrEmpty(Order.Cost.ToString())
                        || string.IsNullOrEmpty(Order.EstimatedDate.ToString())
                        || SelectedClient == null)
                    {
                        MessageBox.Show("Пустые поля");
                        return;
                    }

                    Order.User1 = SelectedClient;
                    Order.User = _userContext.User;
                    Order.StateId = 3;

                    _dbService.db.Order.Add(Order);
                    await _dbService.db.SaveChangesAsync();

                    if (SchemasList.Count > 0)
                    {
                        foreach (var item in SchemasList)
                        {
                            item.OrderId = Order.Id;
                        }
                        _dbService.db.OrderImage.AddRange(SchemasList);

                        await _dbService.db.SaveChangesAsync();
                    }

                    _orderContext.NotifyOrderAdded(Order);
                }
                else
                {
                    if (string.IsNullOrEmpty(Order.Title)
                        || string.IsNullOrEmpty(Order.Cost.ToString())
                        || string.IsNullOrEmpty(Order.EstimatedDate.ToString()))
                    {
                        MessageBox.Show("Пустые поля");
                        return;
                    }

                    Order.User1 = _userContext.User;

                    _dbService.db.Order.Add(Order);
                    await _dbService.db.SaveChangesAsync();

                    if (SchemasList.Count > 0)
                    {
                        foreach (var item in SchemasList)
                        {
                            item.Order = Order;
                        }
                        _dbService.db.OrderImage.AddRange(SchemasList);

                        await _dbService.db.SaveChangesAsync();
                    }

                    _orderContext.NotifyOrderAdded(Order);
                }
            }

            GoBackCommand.Execute(null);
            MessageBox.Show(_isEditForm ? "Сохранено" : "Добавлено");
        }

        private void RemoveSchema(object param)
        {
            var schema = param as OrderImage;

            SchemasList.Remove(schema);
        }

        private void AddSchema()
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "*.jpg|*.jpg|*.png|*.png|*.jpeg|*.jpeg|All files|*.*",
            };

            if (fileDialog.ShowDialog().GetValueOrDefault())
            {
                try
                {
                    var orderImage = new OrderImage
                    {
                        ImagePath = Path.GetFileName(fileDialog.FileName),
                        ImageBin = File.ReadAllBytes(fileDialog.FileName),
                    };
                    if (_isEditForm)
                    {
                        orderImage.Order = Order;
                    }
                    SchemasList.Add(orderImage);
                }
                catch
                {
                    return;
                }
            }
        }

        public override void Dispose()
        {
            _orderContext.SelectedOrder = null;

            GC.SuppressFinalize(this);
        }
    }
}
