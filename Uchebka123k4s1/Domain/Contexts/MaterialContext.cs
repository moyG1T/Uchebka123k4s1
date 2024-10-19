using System;
using Uchebka123k4s1.Data.Remote.SqlModel;

namespace Uchebka123k4s1.Domain.Contexts
{
    public class MaterialContext
    {
        private Material selectedMaterial;
        public Material SelectedMaterial
        {
            get => selectedMaterial;
            set
            {
                selectedMaterial = value;
                MaterialChanged?.Invoke();
            }
        }

        public event Action MaterialChanged;
        public event Action<Material> MaterialAdded;
        public void AddMaterial(Material material)
        {
            MaterialAdded?.Invoke(material);
        }
    }
}
