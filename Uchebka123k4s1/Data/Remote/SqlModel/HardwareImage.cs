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
    
    public partial class HardwareImage
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public byte[] ImageBin { get; set; }
        public Nullable<System.DateTime> Timestamp { get; set; }
        public string HardwareId { get; set; }
    
        public virtual Hardware Hardware { get; set; }
    }
}
