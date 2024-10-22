using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading;
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
    public class MaterialListViewModel : ViewModel
    {
        private readonly INavService _materialInteraction;
        private readonly UserContext _userContext;
        private readonly MaterialContext _materialContext;
        private readonly DbService _dbService;

        private CancellationTokenSource _searchCancellationTokenSource = new CancellationTokenSource();

        private ObservableCollection<Material> _materials = new ObservableCollection<Material>();
        public ObservableCollection<Material> Materials
        {
            get => _materials;
            set { _materials = value; OnPropertyChanged(); }
        }

        public List<Material> ResultMaterials
        {
            get
            {
                if (string.IsNullOrEmpty(SearchText) && SelectedWarehouse is null)
                {
                    return Materials.ToList();
                }
                else if (!string.IsNullOrEmpty(SearchText) && SelectedWarehouse != null)
                {
                    return Materials
                        .Where(it =>
                            it.Title.ToLower().Contains(SearchText.ToLower())
                            && it.WarehouseContent.Select(w => w.WarehouseId).Contains(SelectedWarehouse.Id))
                        .ToList();
                }
                else if (!string.IsNullOrEmpty(SearchText))
                {
                    return Materials.Where(it => it.Title.ToLower().Contains(SearchText.ToLower())).ToList();
                }
                else
                {
                    return Materials
                        .Where(it => it.WarehouseContent.Select(w => w.WarehouseId)
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

                    OnPropertyChanged(nameof(ResultMaterials));
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

                    OnPropertyChanged(nameof(ResultMaterials));
                    OnPropertyChanged(nameof(SearchCount));
                }, token);
            }
        }
        public List<Warehouse> Warehouses { get; set; } = new List<Warehouse>();

        public bool IsWarehouseSelected => SelectedWarehouse != null;
        public int SearchCount => ResultMaterials.Count;
        public bool CanInteract => _userContext.User.RoleId == 1 || _userContext.User.RoleId == 3;

        public ICommand LogoutCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand RemoveSelectedWarehouseCommand { get; }
        public ICommand AddMaterialCommand { get; }
        public ICommand EditMaterialCommand { get; }
        public ICommand RemoveMaterialCommand { get; }

        public MaterialListViewModel(
            INavService logout,
            INavService goBack,
            INavService materialInteraction,
            UserContext userContext,
            MaterialContext materialContext,
            DbService dbService)
        {
            _materialInteraction = materialInteraction;
            _userContext = userContext;
            _materialContext = materialContext;
            _dbService = dbService;

            LogoutCommand = new NavigateAndDisposeCommand(logout);
            GoBackCommand = new GoBackCommand(goBack);
            AddMaterialCommand = new NavigateCommand(materialInteraction);

            EditMaterialCommand = new RelayCommand(EditMaterial);
            RemoveSelectedWarehouseCommand = new RelayCommand(RemoveSelectedWarehouse);
            RemoveMaterialCommand = new RelayAsyncCommand(RemoveMaterial);

            if (_userContext.User.RoleId == 5)
            {
                Error = "Доступ запрещен";
            }
            else
            {
                Task.Run(LoadMaterials);
            }

            _materialContext.MaterialAdded += AddMaterial;
        }

        private void AddMaterial(Material material)
        {
            Materials.Insert(0, material);
            OnPropertyChanged(nameof(ResultMaterials));
        }

        private void EditMaterial(object param)
        {
            if (param is Material material)
            {
                _materialContext.SelectedMaterial = material;
                _materialInteraction.Navigate();
            }
        }

        private async Task RemoveMaterial(object param)
        {
            if (param is Material material)
            {
                if (material.WarehouseContent.Count == 0)
                {
                    var result = MessageBox.Show("Удаление", "Вы точно хотите удалить?", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }

                    _dbService.db.Material.Remove(material);
                    await _dbService.db.SaveChangesAsync();

                    Materials.Remove(material);

                    OnPropertyChanged(nameof(ResultMaterials));
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
            Materials = new ObservableCollection<Material>(await _dbService.db.Material.ToListAsync());
            OnPropertyChanged(nameof(ResultMaterials));
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
