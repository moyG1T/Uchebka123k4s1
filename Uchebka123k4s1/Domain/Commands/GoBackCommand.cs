﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uchebka123k4s1.Domain.IServices;

namespace Uchebka123k4s1.Domain.Commands
{
    public class GoBackCommand : CommandBase
    {
        private readonly INavService _navService;

        public GoBackCommand(INavService navService)
        {
            _navService = navService;
        }

        public override void Execute(object parameter)
        {
            _navService.GoBack();
        }
    }
}
