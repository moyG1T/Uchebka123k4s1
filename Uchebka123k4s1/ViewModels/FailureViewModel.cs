using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Uchebka123k4s1.Data.Remote.SqlModel;
using Uchebka123k4s1.Data.Services;
using Uchebka123k4s1.Domain.Commands;
using Uchebka123k4s1.Domain.IServices;
using Uchebka123k4s1.Domain.Utilities;

namespace Uchebka123k4s1.ViewModels
{
    public class FailureViewModel : ViewModel
    {
        private readonly DbService _dbService;

        public HardwareFailure Failure { get; set; } = new HardwareFailure { Timestamp = DateTime.Now, FixTimestamp = DateTime.Now};
        public Hardware SelectedHardware { get; set; }
        public List<Hardware> Hardwares { get; set; } = new List<Hardware>();

        public ICommand LogoutCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand ApplyChangesCommand { get; }

        public FailureViewModel(
            INavService logout,
            INavService back,
            DbService dbService
            )
        {
            _dbService = dbService;

            LogoutCommand = new NavigateAndDisposeCommand(logout);
            GoBackCommand = new GoBackCommand(back);
            ApplyChangesCommand = new RelayAsyncCommand(SaveChanges);

            Task.Run(LoadContent);
        }

        private async Task LoadContent()
        {
            var hardwares = await _dbService.Hardware.ToListAsync();
            Hardwares = hardwares;
            OnPropertyChanged(nameof(Hardwares));
        }

        private async Task SaveChanges()
        {
            if (string.IsNullOrEmpty(Failure.Title)
                || string.IsNullOrEmpty(Failure.Subtitle)
                || string.IsNullOrEmpty(Failure.Timestamp.ToString())
                || string.IsNullOrEmpty(Failure.FixTimestamp.ToString())
                || SelectedHardware is null
                )
            {
                MessageBox.Show("Поля пустые");
                return;
            }

            if (Failure.Timestamp > Failure.FixTimestamp)
            {
                MessageBox.Show("Неправильное время");
                return;
            }

            try
            {
                Failure.Hardware = SelectedHardware;

                _dbService.HardwareFailure.Add(Failure);
                await _dbService.SaveChangesAsync();

                GoBackCommand.Execute(null);
                MessageBox.Show("Добавлено");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
