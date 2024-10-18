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
using Uchebka123k4s1.Domain.IServices;
using Uchebka123k4s1.Domain.Utilities;

namespace Uchebka123k4s1.ViewModels
{
    public class AddWorkerViewModel : ViewModel
    {
        private readonly DbService _dbService;

        public User Worker { get; set; } = new User() { RoleId = 10 };

        public UserFullName WorkerFullName { get; set; } = new UserFullName();
        public UserAddress WorkerAddress { get; set; } = new UserAddress();
        public UserDegree WorkerDegree { get; set; } = new UserDegree();
        public UserQualification WorkerQualification { get; set; } = new UserQualification();

        private string _error;
        public string Error
        {
            get => _error; set
            {
                _error = value;
                OnPropertyChanged();
            }
        }

        private UserOperation _selectedOperation;
        public UserOperation SelectedOperation
        {
            get => _selectedOperation; set
            {
                _selectedOperation = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<UserOperation> Operations { get; set; } = new ObservableCollection<UserOperation>();
        public ObservableCollection<UserPossibility> WorkerPossibilities { get; set; } = new ObservableCollection<UserPossibility>();

        public ICommand GoBackCommand { get; }
        public ICommand AddWorkerCommand { get; }
        public ICommand AddOperationCommand { get; }
        public ICommand RemoveOperationCommand { get; }

        public AddWorkerViewModel(INavService goBackNavService, DbService dbService)
        {
            this._dbService = dbService;

            GoBackCommand = new GoBackCommand(goBackNavService);
            AddWorkerCommand = new RelayAsyncCommand(AddWorker);
            AddOperationCommand = new RelayCommand(AddOperation);
            RemoveOperationCommand = new RelayCommand(RemoveOperation);

            Task.Run(LoadPossibilities);
        }

        private async Task LoadPossibilities()
        {
            var operations = await _dbService.db.UserOperation.ToListAsync();

            Operations = new ObservableCollection<UserOperation>(operations);
        }

        private async Task AddWorker()
        {
            if (string.IsNullOrEmpty(Worker.Login) 
                || string.IsNullOrEmpty(Worker.Password) 
                || string.IsNullOrEmpty(WorkerFullName.LastName) 
                || string.IsNullOrEmpty(WorkerFullName.FirstName) 
                || string.IsNullOrEmpty(WorkerFullName.Patronymic) 
                || string.IsNullOrEmpty(WorkerFullName.Age.ToString()) 
                )
            {
                Error = "Обязательные поля пустые";
                _ = Task.Run(async () => { await Task.Delay(2500); Error = string.Empty; });
                return;
            }

            _dbService.db.User.Add(Worker);
            await _dbService.db.SaveChangesAsync();

            var fullName = new UserFullName()
            {
                LastName = WorkerFullName.LastName,
                FirstName = WorkerFullName.FirstName,
                Patronymic = WorkerFullName.Patronymic,
                Age = WorkerFullName.Age,
                UserId = Worker.Id
            };

            _dbService.db.UserFullName.Add(fullName);

            if (!string.IsNullOrEmpty(WorkerAddress.Address))
            {
                var address = new UserAddress { Address = WorkerAddress.Address, UserId = Worker.Id };
                _dbService.db.UserAddress.Add(address);

                Worker.UserAddress.Add(address);
            }

            if (!string.IsNullOrEmpty(WorkerDegree.Title))
            {
                var degree = new UserDegree { Title = WorkerAddress.Address, UserId = Worker.Id };
                _dbService.db.UserDegree.Add(degree);

                Worker.UserDegree.Add(degree);
            }

            if (!string.IsNullOrEmpty(WorkerQualification.Title))
            {
                var qual = new UserQualification { Title = WorkerQualification.Title, UserId = Worker.Id };
                _dbService.db.UserQualification.Add(qual);

                Worker.UserQualification.Add(qual);
            }

            if (WorkerPossibilities.Count > 0)
            {
                var possies = new List<UserPossibility>(WorkerPossibilities);

                possies.ForEach(it => it.UserId = Worker.Id);

                _dbService.db.UserPossibility.AddRange(possies);

                Worker.UserPossibility = new ObservableCollection<UserPossibility>(possies);
            }

            await _dbService.db.SaveChangesAsync();
            _dbService.AddWorker(Worker);

            GoBackCommand.Execute(null);
        }

        private void AddOperation()
        {
            if (SelectedOperation != null)
            {
                var newUserOper = new UserPossibility
                {
                    OperId = SelectedOperation.Id,
                    UserOperation = SelectedOperation,
                };
                WorkerPossibilities.Add(newUserOper);

                Operations.Remove(SelectedOperation);
            }
        }

        private void RemoveOperation(object param)
        {
            if (param is UserPossibility possibility)
            {
                var oper = possibility.UserOperation;

                WorkerPossibilities.Remove(possibility);
                Operations.Add(oper);
            }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
