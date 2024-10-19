using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Uchebka123k4s1.Data.Remote.SqlModel;
using Uchebka123k4s1.Data.Services;
using Uchebka123k4s1.Domain.Commands;
using Uchebka123k4s1.Domain.Contexts;
using Uchebka123k4s1.Domain.IServices;
using Uchebka123k4s1.Domain.Utilities;

namespace Uchebka123k4s1.ViewModels
{
    public class MaterialInteractionViewModel : ViewModel
    {
        private readonly MaterialContext _materialContext;
        private readonly DbService _dbService;

        public Material Material { get; set; } = new Material();

        public Warehouse SelectedWarehouse { get; set; }

        public List<Supplier> Suppliers { get; set; } = new List<Supplier>();
        public List<MaterialUnit> Units { get; set; } = new List<MaterialUnit>();

        public ObservableCollection<Warehouse> AvailableWarehouses { get; set; }
            = new ObservableCollection<Warehouse>();

        public string InteractionName =>
            string.IsNullOrEmpty(Material.Id) ? "Добавление" : "Редактирование";
        public string ButtonInteractionName =>
            string.IsNullOrEmpty(Material.Id) ? "Добавить" : "Сохранить";
        public bool IsIdEditEnable => string.IsNullOrEmpty(Material.Id);
        public bool IsAdditionalEnabled => !string.IsNullOrEmpty(Material.Id);

        public ICommand LogoutCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand ApplyChangesCommand { get; }
        public ICommand AddContentCommand { get; }
        public ICommand RemoveContentCommand { get; }

        public MaterialInteractionViewModel(
            INavService logout,
            INavService goBack,
            MaterialContext materialContext,
            DbService dbService
            )
        {
            _materialContext = materialContext;
            _dbService = dbService;

            var local = _materialContext.SelectedMaterial;
            if (local != null)
            {
                var warehouseList = new List<WarehouseContent>();

                foreach (var content in local.WarehouseContent)
                {
                    var newContent = new WarehouseContent
                    {
                        Warehouse = content.Warehouse,
                        Count = content.Count,
                    };
                    warehouseList.Add(newContent);
                }

                Material = new Material
                {
                    Id = local.Id,
                    Title = local.Title,
                    WarehouseContent = new ObservableCollection<WarehouseContent>(warehouseList),
                    Supplier = local.Supplier,
                    Cost = local.Cost,
                    MaterialUnit = local.MaterialUnit,
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

            var content = new WarehouseContent
            {
                Warehouse = SelectedWarehouse,
                Count = 0,
            };

            Material.WarehouseContent.Add(content);

            AvailableWarehouses.Remove(SelectedWarehouse);
        }

        private void RemoveContent(object param)
        {
            if (param is WarehouseContent content)
            {
                AvailableWarehouses.Add(content.Warehouse);

                Material.WarehouseContent.Remove(content);
            }
        }

        private async Task LoadContent()
        {
            Suppliers = await _dbService.db.Supplier.ToListAsync();
            OnPropertyChanged(nameof(Suppliers));
            Units = await _dbService.db.MaterialUnit.ToListAsync();
            OnPropertyChanged(nameof(Units));

            if (Material.Id == string.Empty)
            {
                var allWarehouses = await _dbService.db.Warehouse.ToListAsync();

                AvailableWarehouses = new ObservableCollection<Warehouse>(allWarehouses);
                OnPropertyChanged(nameof(AvailableWarehouses));
            }
            else
            {
                var contentOfWarehouses = await _dbService.db.WarehouseContent
                    .Where(c => c.MaterialId == Material.Id)
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
            if (string.IsNullOrEmpty(_materialContext.SelectedMaterial?.Id))
            {
                try
                {
                    var material = new Material
                    {
                        Id = Material.Id,
                        Title = Material.Title,
                        MaterialUnit = Material.MaterialUnit,
                        Cost = Material.Cost,
                    };

                    _dbService.db.Material.Add(material);
                    await _dbService.db.SaveChangesAsync();

                    _materialContext.SelectedMaterial = material;
                    Material.WarehouseContent = new ObservableCollection<WarehouseContent>();

                    OnPropertyChanged(nameof(IsAdditionalEnabled));

                    _materialContext.AddMaterial(material);
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
                    var material = await _dbService.db.Material.FirstOrDefaultAsync(m => m.Id == Material.Id);

                    material.Title = Material.Title;
                    material.MaterialUnit = Material.MaterialUnit;
                    material.Cost = Material.Cost;
                    material.Supplier = Material.Supplier;
                    material.WarehouseContent = Material.WarehouseContent;

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
            _materialContext.SelectedMaterial = null;

            GC.SuppressFinalize(this);
        }
    }
}
