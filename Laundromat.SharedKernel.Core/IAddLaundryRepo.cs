﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Laundromat.SharedKernel.Core
{
    public interface IAddLaundryRepo
    {
        Task AddLaundry(NewLaundryDto laundryDto);
    }
}
