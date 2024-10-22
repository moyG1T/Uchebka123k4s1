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
    using System.Collections.ObjectModel;
    using Uchebka123k4s1.Domain.Utilities;

    public partial class User : ObservableObject
    {
        private int id;
        private string login;
        private string password;
        private int? roleId;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.UserAddress = new ObservableCollection<UserAddress>();
            this.UserFullName = new ObservableCollection<UserFullName>();
            this.UserAddress = new ObservableCollection<UserAddress>();
            this.UserDegree = new ObservableCollection<UserDegree>();
            this.UserImage = new ObservableCollection<UserImage>();
            this.UserPossibility = new ObservableCollection<UserPossibility>();
            this.UserQualification = new ObservableCollection<UserQualification>();
            this.UserSkill = new ObservableCollection<UserSkill>();
            this.Order = new HashSet<Order>();
            this.Order1 = new HashSet<Order>();
        }

        public int Id
        {
            get => id; set
            {
                id = value;
                OnPropertyChanged();
            }
        }
        public string Login
        {
            get => login; set
            {
                login = value;
                OnPropertyChanged();
            }
        }
        public string Password
        {
            get => password; set
            {
                password = value;
                OnPropertyChanged();
            }
        }
        public Nullable<int> RoleId
        {
            get => roleId; set
            {
                roleId = value;
                OnPropertyChanged();
            }
        }

        public virtual Role Role { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<UserAddress> UserAddress { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<UserDegree> UserDegree { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<UserFullName> UserFullName { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<UserImage> UserImage { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<UserPossibility> UserPossibility { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<UserQualification> UserQualification { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<UserSkill> UserSkill { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Order { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Order1 { get; set; }

    }
}
