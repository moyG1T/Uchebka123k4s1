//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Uchebka123k4s1.Data.Remote.SqlModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class WarehouseContent
    {
        public int WarehouseId { get; set; }
        public string MaterialId { get; set; }
        public Nullable<int> Count { get; set; }
    
        public virtual Material Material { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }
}
