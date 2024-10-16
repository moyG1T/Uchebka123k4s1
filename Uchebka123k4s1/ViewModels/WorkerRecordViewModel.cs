using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Uchebka123k4s1.Data.Remote.IServices;
using Uchebka123k4s1.Data.Remote.SqlModel;
using Uchebka123k4s1.Data.Services;
using Uchebka123k4s1.Domain.Commands;
using Uchebka123k4s1.Domain.IServices;
using Uchebka123k4s1.Domain.Utilities;

namespace Uchebka123k4s1.ViewModels
{
    public class WorkerRecordViewModel : ViewModel
    {
        private ObservableCollection<User> _workers = new ObservableCollection<User>();
        private readonly DbService _dbService;

        public ObservableCollection<User> Workers
        {
            get => _workers;
            set
            {
                _workers = value;
                OnPropertyChanged();
            }
        }

        public WorkerRecordViewModel(DbService dbService)
        {
            _dbService = dbService;

            Task.Run(LoadWorkers);
        }

        private async Task LoadWorkers()
        {
            var users = await _dbService.db.User.Where(u => u.RoleId == 10).ToListAsync();

            Workers = new ObservableCollection<User>(users);
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
