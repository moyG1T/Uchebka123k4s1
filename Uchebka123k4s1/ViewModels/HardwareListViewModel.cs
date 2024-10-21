using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Input;
using System.Windows;
using Uchebka123k4s1.Data.Remote.SqlModel;
using Uchebka123k4s1.Data.Services;
using Uchebka123k4s1.Domain.Commands;
using Uchebka123k4s1.Domain.Contexts;
using Uchebka123k4s1.Domain.IServices;
using Uchebka123k4s1.Domain.Utilities;
using System.Linq;
using System.Data.Entity;

namespace Uchebka123k4s1.ViewModels
{
    public class HardwareListViewModel : ViewModel
    {
        private readonly INavService _hardwareInteraction;
        private readonly UserContext _userContext;
        private readonly HardwareContext _hardwareContext;
        private readonly DbService _dbService;

        private CancellationTokenSource _searchCancellationTokenSource = new CancellationTokenSource();

        private ObservableCollection<Hardware> _hardwareCollection = new ObservableCollection<Hardware>();
        public ObservableCollection<Hardware> HardwareCollection
        {
            get => _hardwareCollection;
            set { _hardwareCollection = value; OnPropertyChanged(); }
        }

        public List<Hardware> ResultHardwareCollection
        {
            get
            {
                if (string.IsNullOrEmpty(SearchText) && SelectedWarehouse is null)
                {
                    return HardwareCollection.ToList();
                }
                else if (!string.IsNullOrEmpty(SearchText) && SelectedWarehouse != null)
                {
                    return HardwareCollection
                        .Where(it =>
                            it.Title.ToLower().Contains(SearchText.ToLower())
                            && it.WarehouseHardware.Select(w => w.WarehouseId).Contains(SelectedWarehouse.Id))
                        .ToList();
                }
                else if (!string.IsNullOrEmpty(SearchText))
                {
                    return HardwareCollection.Where(it => it.Title.ToLower().Contains(SearchText.ToLower())).ToList();
                }
                else
                {
                    return HardwareCollection
                        .Where(it => it.WarehouseHardware.Select(w => w.WarehouseId)
                            .Contains(SelectedWarehouse.Id))
                        .ToList();
                }
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();

                _searchCancellationTokenSource.Cancel();
                _searchCancellationTokenSource = new CancellationTokenSource();

                var token = _searchCancellationTokenSource.Token;

                Task.Run(async () =>
                {
                    await Task.Delay(500, token);

                    if (token.IsCancellationRequested)
                        return;

                    OnPropertyChanged(nameof(ResultHardwareCollection));
                    OnPropertyChanged(nameof(SearchCount));
                }, token);
            }
        }
        private string _error;
        public string Error
        {
            get => _error;
            set
            {
                _error = value;
                OnPropertyChanged();
            }
        }

        private Warehouse selectedWarehouse;
        public Warehouse SelectedWarehouse
        {
            get => selectedWarehouse;
            set
            {
                selectedWarehouse = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsWarehouseSelected));

                _searchCancellationTokenSource.Cancel();
                _searchCancellationTokenSource = new CancellationTokenSource();

                var token = _searchCancellationTokenSource.Token;

                Task.Run(async () =>
                {
                    await Task.Delay(500, token);

                    if (token.IsCancellationRequested)
                        return;

                    OnPropertyChanged(nameof(ResultHardwareCollection));
                    OnPropertyChanged(nameof(SearchCount));
                }, token);
            }
        }
        public List<Warehouse> Warehouses { get; set; } = new List<Warehouse>();

        public bool IsWarehouseSelected => SelectedWarehouse != null;
        public int SearchCount => ResultHardwareCollection.Count;
        public bool CanInteract => _userContext.User.RoleId == 4 || _userContext.User.RoleId == 5;

        public ICommand LogoutCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand RemoveSelectedWarehouseCommand { get; }
        public ICommand AddHardwareCommand { get; }
        public ICommand EditHardwareCommand { get; }
        public ICommand RemoveHardwareCommand { get; }

        public HardwareListViewModel(
            INavService logout,
            INavService goBack,
            INavService materialInteraction,
            UserContext userContext,
            HardwareContext hardwareContext,
            DbService dbService)
        {
            _hardwareInteraction = materialInteraction;
            _userContext = userContext;
            _hardwareContext = hardwareContext;
            _dbService = dbService;

            LogoutCommand = new NavigateAndDisposeCommand(logout);
            GoBackCommand = new GoBackCommand(goBack);
            AddHardwareCommand = new NavigateCommand(materialInteraction);

            EditHardwareCommand = new RelayCommand(EditHardware);
            RemoveSelectedWarehouseCommand = new RelayCommand(RemoveSelectedWarehouse);
            RemoveHardwareCommand = new RelayAsyncCommand(RemoveMaterial);

            if (_userContext.User.RoleId == 1)
            {
                Error = "Доступ запрещен";
            }
            else
            {
                Task.Run(LoadMaterials);
            }

            _hardwareContext.HardwareAdded += AddHardware;
        }

        private void AddHardware(Hardware hardware)
        {
            HardwareCollection.Insert(0, hardware);
            OnPropertyChanged(nameof(ResultHardwareCollection));
        }

        private void EditHardware(object param)
        {
            if (param is Hardware hardware)
            {
                _hardwareContext.SelectedHardware = hardware;
                _hardwareInteraction.Navigate();
            }
        }

        private async Task RemoveMaterial(object param)
        {
            if (param is Hardware hardware)
            {
                if (hardware.WarehouseHardware.Count == 0)
                {
                    var result = MessageBox.Show("Удаление", "Вы точно хотите удалить?", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }

                    _dbService.db.Hardware.Remove(hardware);
                    await _dbService.db.SaveChangesAsync();

                    HardwareCollection.Remove(hardware);

                    OnPropertyChanged(nameof(ResultHardwareCollection));
                }
                else
                {
                    MessageBox.Show("Материал есть на складе");
                }
            }
        }

        private void RemoveSelectedWarehouse()
        {
            SelectedWarehouse = null;
        }

        private async Task LoadMaterials()
        {
            HardwareCollection = new ObservableCollection<Hardware>(await _dbService.db.Hardware.ToListAsync());
            OnPropertyChanged(nameof(ResultHardwareCollection));
            OnPropertyChanged(nameof(SearchCount));

            Warehouses = await _dbService.db.Warehouse.ToListAsync();
            OnPropertyChanged(nameof(Warehouses));
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
