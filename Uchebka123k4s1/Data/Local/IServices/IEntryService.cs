using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uchebka123k4s1.Data.Local.IServices
{
    public interface IEntryService
    {
        string Read();
        bool Read(out string id);
        void Write(string id);
        void Remove();
        bool Exists();
    }
}
