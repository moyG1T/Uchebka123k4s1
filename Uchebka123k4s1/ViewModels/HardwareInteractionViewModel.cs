using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Uchebka123k4s1.Data.Remote.SqlModel;
using Uchebka123k4s1.Data.Services;
using Uchebka123k4s1.Domain.Commands;
using Uchebka123k4s1.Domain.Contexts;
using Uchebka123k4s1.Domain.IServices;
using Uchebka123k4s1.Domain.Utilities;
using System.Data.Entity;
using System.Linq;

namespace Uchebka123k4s1.ViewModels
{
    public class HardwareInteractionViewModel : ViewModel
    {
        private readonly HardwareContext _hardwareContext;
        private readonly DbService _dbService;

        public Hardware Hardware { get; set; } = new Hardware();

        public Warehouse SelectedWarehouse { get; set; }

        public List<Supplier> Suppliers { get; set; } = new List<Supplier>();
        public List<HardwareUnit> Units { get; set; } = new List<HardwareUnit>();

        public ObservableCollection<Warehouse> AvailableWarehouses { get; set; }
            = new ObservableCollection<Warehouse>();

        public string InteractionName =>
            string.IsNullOrEmpty(Hardware.Id) ? "Добавление" : "Редактирование";
        public string ButtonInteractionName =>
            string.IsNullOrEmpty(Hardware.Id) ? "Добавить" : "Сохранить";
        public bool IsIdEditEnable => string.IsNullOrEmpty(Hardware.Id);
        public bool IsAdditionalEnabled => !string.IsNullOrEmpty(Hardware.Id);

        public ICommand LogoutCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand ApplyChangesCommand { get; }
        public ICommand AddContentCommand { get; }
        public ICommand RemoveContentCommand { get; }

        public HardwareInteractionViewModel(
            INavService logout,
            INavService goBack,
            HardwareContext hardwareContext,
            DbService dbService
            )
        {
            _hardwareContext = hardwareContext;
            _dbService = dbService;

            var local = _hardwareContext.SelectedHardware;
            if (local != null)
            {
                var warehouseList = new List<WarehouseHardware>();

                foreach (var content in local.WarehouseHardware)
                {
                    var newContent = new WarehouseHardware
                    {
                        Warehouse = content.Warehouse,
                        Count = content.Count,
                    };
                    warehouseList.Add(newContent);
                }

                Hardware = new Hardware
                {
                    Id = local.Id,
                    Title = local.Title,
                    WarehouseHardware = new ObservableCollection<WarehouseHardware>(warehouseList),
                    Supplier = local.Supplier,
                    Cost = local.Cost,
                    HardwareUnit = local.HardwareUnit,
                };
            }

            LogoutCommand = new NavigateAndDisposeCommand(logout);
            GoBackCommand = new GoBackCommand(goBack);
            ApplyChangesCommand = new RelayAsyncCommand(ApplyChanges);
            AddContentCommand = new RelayCommand(AddContent);
            RemoveContentCommand = new RelayCommand(RemoveContent);

            Task.Run(LoadContent);
        }

        private void AddContent()
        {
            if (SelectedWarehouse is null)
            {
                return;
            }

            var content = new WarehouseHardware
            {
                Warehouse = SelectedWarehouse,
                Count = 0,
            };

            Hardware.WarehouseHardware.Add(content);

            AvailableWarehouses.Remove(SelectedWarehouse);
        }

        private void RemoveContent(object param)
        {
            if (param is WarehouseHardware content)
            {
                AvailableWarehouses.Add(content.Warehouse);

                Hardware.WarehouseHardware.Remove(content);
            }
        }

        private async Task LoadContent()
        {
            Suppliers = await _dbService.db.Supplier.ToListAsync();
            OnPropertyChanged(nameof(Suppliers));
            Units = await _dbService.db.HardwareUnit.ToListAsync();
            OnPropertyChanged(nameof(Units));

            if (Hardware.Id == string.Empty)
            {
                var allWarehouses = await _dbService.db.Warehouse.ToListAsync();

                AvailableWarehouses = new ObservableCollection<Warehouse>(allWarehouses);
                OnPropertyChanged(nameof(AvailableWarehouses));
            }
            else
            {
                var contentOfWarehouses = await _dbService.db.WarehouseContent
                    .Where(c => c.MaterialId == Hardware.Id)
                    .ToListAsync();

                var whIds = contentOfWarehouses.Select(it => it.WarehouseId);

                var availableWarehouses = await _dbService.db.Warehouse
                    .Where(w => !whIds.Contains(w.Id))
                    .ToListAsync();

                AvailableWarehouses = new ObservableCollection<Warehouse>(availableWarehouses);
                OnPropertyChanged(nameof(AvailableWarehouses));
            }
        }

        private async Task ApplyChanges()
        {
            if (string.IsNullOrEmpty(_hardwareContext.SelectedHardware?.Id))
            {
                try
                {
                    var material = new Hardware
                    {
                        Id = Hardware.Id,
                        Title = Hardware.Title,
                        HardwareUnit = Hardware.HardwareUnit,
                        Cost = Hardware.Cost,
                    };

                    _dbService.db.Hardware.Add(material);
                    await _dbService.db.SaveChangesAsync();

                    _hardwareContext.SelectedHardware = material;
                    Hardware.WarehouseHardware = new ObservableCollection<WarehouseHardware>();

                    OnPropertyChanged(nameof(IsAdditionalEnabled));

                    _hardwareContext.AddMaterial(material);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                try
                {
                    var hardware = await _dbService.db.Hardware.FirstOrDefaultAsync(m => m.Id == Hardware.Id);

                    hardware.Title = Hardware.Title;
                    hardware.HardwareUnit = Hardware.HardwareUnit;
                    hardware.Cost = Hardware.Cost;
                    hardware.Supplier = Hardware.Supplier;
                    hardware.WarehouseHardware = Hardware.WarehouseHardware;

                    await _dbService.db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        public override void Dispose()
        {
            _hardwareContext.SelectedHardware = null;

            GC.SuppressFinalize(this);
        }
    }
}
