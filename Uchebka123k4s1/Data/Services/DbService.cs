using System;
using Uchebka123k4s1.Data.Remote.SqlModel;

namespace Uchebka123k4s1.Data.Services
{
    public class DbService : UchebkaV2Entities
    {
        public event Action<User> WorkerAdded;
        public void AddWorker(User worker)
        {
            WorkerAdded?.Invoke(worker);
        }
    }
}
