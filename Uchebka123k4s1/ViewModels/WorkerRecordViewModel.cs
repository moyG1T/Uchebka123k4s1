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
using Uchebka123k4s1.Domain.Contexts;
using Uchebka123k4s1.Domain.IServices;
using Uchebka123k4s1.Domain.Utilities;

namespace Uchebka123k4s1.ViewModels
{
    public class WorkerRecordViewModel : ViewModel
    {
        private readonly UserContext _userContext;
        private readonly DbService _dbService;

        private ObservableCollection<User> _workers = new ObservableCollection<User>();
        public ObservableCollection<User> Workers
        {
            get => _workers;
            set
            {
                _workers = value;
                OnPropertyChanged();
            }
        }

        public ICommand LogoutCommand { get; }
        public ICommand AddWorkerCommand { get; }
        public ICommand RemoveWorkerCommand { get; }

        public WorkerRecordViewModel(
            INavService exitNavService, 
            INavService addWorkerNavService, 
            UserContext userContext, 
            DbService dbService)
        {
            _userContext = userContext;
            _dbService = dbService;

            LogoutCommand = new NavigateAndDisposeCommand(exitNavService);
            AddWorkerCommand = new NavigateCommand(addWorkerNavService);
            RemoveWorkerCommand = new RelayAsyncCommand(RemoveWorker);

            _dbService.WorkerAdded += this.AddWorker;

            Task.Run(LoadWorkers);
        }

        private void AddWorker(User worker)
        {
            Workers.Add(worker);
        }

        private async Task RemoveWorker(object param)
        {
            if (param is User user)
            {
                try
                {
                    var remoteUser = await _dbService.db.User.FirstOrDefaultAsync(it => it.Id == user.Id);
                    remoteUser.RoleId = 11;

                    await _dbService.db.SaveChangesAsync();

                    Workers.FirstOrDefault(u => u.Id == user.Id).RoleId = 11;
                }
                catch
                {
                    MessageBox.Show("Ошибка");
                }
            }
        }

        private async Task LoadWorkers()
        {
            if (_userContext.User.RoleId == 5)
            {
                var users = await _dbService.db.User.Where(u => u.RoleId == 10).ToListAsync();

                Workers = new ObservableCollection<User>(users);
            }
            else
            {
                MessageBox.Show("Доступ запрещен");
            }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
