using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uchebka123k4s1.Data.Remote.SqlModel;

namespace Uchebka123k4s1.Domain.Contexts
{
    public class HardwareContext
    {
        public Hardware SelectedHardware { get; set; }

        public event Action HardwareChanged;
        public event Action<Hardware> HardwareAdded;
        public void AddMaterial(Hardware hardware)
        {
            HardwareAdded?.Invoke(hardware);
        }
    }
}
