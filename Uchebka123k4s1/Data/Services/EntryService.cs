using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uchebka123k4s1.Data.Local.IServices;
using Uchebka123k4s1.Domain.Contexts;

namespace Uchebka123k4s1.Data.Services
{
    public class EntryService : IEntryService
    {
        private readonly string _filePath;

        public EntryService()
        {
            string docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _filePath = Path.Combine(docs, "Uchebka", "user.txt");
        }

        public string Read()
        {
            return File.ReadAllText(_filePath);
        }
        public bool Read(out string id)
        {
            return !string.IsNullOrEmpty(id = File.ReadAllText(_filePath));
        }

        public void Remove()
        {
            if (Exists())
            {
                File.Delete(_filePath);
            }
        }

        public void Write(string id)
        {
            File.WriteAllText(_filePath, id);
        }

        public bool Exists()
        {
            return File.Exists(_filePath);
        }
    }
}
