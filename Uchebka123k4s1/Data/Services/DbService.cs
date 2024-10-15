using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uchebka123k4s1.Data.Remote.IServices;
using Uchebka123k4s1.Data.Remote.SqlModel;

namespace Uchebka123k4s1.Data.Services
{
    public class DbService : IDbService
    {
        public HomeEntities db = new HomeEntities();
    }
}
