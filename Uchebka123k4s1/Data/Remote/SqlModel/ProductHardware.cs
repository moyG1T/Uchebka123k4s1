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
    
    public partial class ProductHardware
    {
        public int Id { get; set; }
        public Nullable<int> ProductId { get; set; }
        public string HardwareId { get; set; }
        public Nullable<int> Amount { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual Hardware Hardware { get; set; }
    }
}
