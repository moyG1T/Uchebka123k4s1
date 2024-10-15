using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uchebka123k4s1.Domain.IServices
{
    public interface INavService
    {
        void Navigate();
        void NavigateAndDispose();
        void GoBack();
    }
}
