using Laundromat.MainProfile.API.Enitities;
using Laundromat.SharedKernel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laundromat.MainProfile.API.Repositories
{
    public class LaundryRepo: GenericRepo<Laundry,Guid>,ILaundryRepo
    {
        public LaundryRepo(ApplicationDbContext context):base(context)
        {

        }

        public async Task AddLaundry(NewLaundryDto laundryDto)
        {
            var laundry = new Laundry { CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, Name = laundryDto.Name };
            await Create(laundry);
            return;
        }
    }
}
}
