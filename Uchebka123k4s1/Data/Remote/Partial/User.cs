﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uchebka123k4s1.Data.Remote.SqlModel
{
    public partial class User
    {
        public UserFullName UserFullNameCopy { get => UserFullName.FirstOrDefault(); }
        public string FIO
        {
            get
            {
                if (UserFullNameCopy is null)
                {
                    return "Нет ФИО";
                }
                else
                {
                    return string.Concat(UserFullNameCopy.LastName, " ", UserFullNameCopy.FirstName, " ", UserFullNameCopy.Patronymic);
                }
            }
        }
    }
}
