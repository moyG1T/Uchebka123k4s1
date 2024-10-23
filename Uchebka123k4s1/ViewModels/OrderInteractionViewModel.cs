using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
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
    public class OrderInteractionViewModel : ViewModel
    {
        private readonly DbService _dbService;
        private readonly UserContext _userContext;

        public bool IsManager => _userContext.User.RoleId == 3;

        public User SelectedClient { get; set; }
        public List<User> Clients { get; set; }
        public ObservableCollection<OrderImage> SchemasList { get; set; } = new ObservableCollection<OrderImage>();
        public Order Order { get; set; }

        public ICommand LogoutCommand { get; }
        public ICommand GoBackCommand { get; }

        public ICommand OpenFileDialogCommand { get; }
        public ICommand RemoveSchemaCommand { get; }

        public OrderInteractionViewModel(
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
            OpenFileDialogCommand = new RelayCommand(OpenFileDialog);
            RemoveSchemaCommand = new RelayCommand(RemoveSchema);

            Task.Run(LoadManagerContent);
        }

        private async Task LoadManagerContent()
        {
            var clients = await _dbService.db.User.Where(it => it.RoleId == 5 && it.UserFullName.Count > 0).ToListAsync();

            Clients = clients;
            OnPropertyChanged(nameof(Clients));
        }

        private void RemoveSchema(object param)
        {
            var schema = param as OrderImage;

            SchemasList.Remove(schema);
        }

        private void OpenFileDialog()
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
            GC.SuppressFinalize(this);
        }
    }
}
