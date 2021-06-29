using Laundromat.MainProfile.API.Enitities;
using Laundromat.SharedKernel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laundromat.MainProfile.API.Repositories
{
    public interface ILaundryRepo:IGenericRepo<Laundry,Guid>, IAddLaundryRepo
    {
    }
}
