﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class HomeEntities : DbContext
    {
        public HomeEntities()
            : base("name=HomeEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Description> Description { get; set; }
        public virtual DbSet<Equipment> Equipment { get; set; }
        public virtual DbSet<EquipmentDesc> EquipmentDesc { get; set; }
        public virtual DbSet<EquipmentType> EquipmentType { get; set; }
        public virtual DbSet<Gost> Gost { get; set; }
        public virtual DbSet<Hardware> Hardware { get; set; }
        public virtual DbSet<HardwareImage> HardwareImage { get; set; }
        public virtual DbSet<HardwareType> HardwareType { get; set; }
        public virtual DbSet<HardwareUnit> HardwareUnit { get; set; }
        public virtual DbSet<MaterialImage> MaterialImage { get; set; }
        public virtual DbSet<MaterialLength> MaterialLength { get; set; }
        public virtual DbSet<MaterialType> MaterialType { get; set; }
        public virtual DbSet<MaterialUnit> MaterialUnit { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductContent> ProductContent { get; set; }
        public virtual DbSet<ProductHardware> ProductHardware { get; set; }
        public virtual DbSet<ProductMaterial> ProductMaterial { get; set; }
        public virtual DbSet<ProductOperation> ProductOperation { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<UserImage> UserImage { get; set; }
        public virtual DbSet<UserAddress> UserAddress { get; set; }
        public virtual DbSet<UserDegree> UserDegree { get; set; }
        public virtual DbSet<UserOperation> UserOperation { get; set; }
        public virtual DbSet<UserPossibility> UserPossibility { get; set; }
        public virtual DbSet<UserQualification> UserQualification { get; set; }
        public virtual DbSet<UserSkill> UserSkill { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserFullName> UserFullName { get; set; }
        public virtual DbSet<Supplier> Supplier { get; set; }
        public virtual DbSet<WarehouseContent> WarehouseContent { get; set; }
        public virtual DbSet<Material> Material { get; set; }
        public virtual DbSet<Warehouse> Warehouse { get; set; }
    }
}
