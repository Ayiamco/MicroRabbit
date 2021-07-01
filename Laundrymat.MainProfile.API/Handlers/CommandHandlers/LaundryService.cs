using Laundromat.MainProfile.API.Enitities;
using Laundromat.MainProfile.API.Repositories;
using Laundromat.MainProfile.API.Services;
using Laundromat.SharedKernel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laundromat.MainProfile.API.Services
{
    public class LaundryService : ILaundryService
    {
        private readonly ILaundryRepo laundryRepo;
        private readonly IUnitOfWork unitOfWork;

        public LaundryService(ILaundryRepo laundryRepo,IUnitOfWork unitOfWork)
        {
            this.laundryRepo = laundryRepo;
            this.unitOfWork = unitOfWork;
        }
        public async Task<Guid> AddLaundry(NewLaundryDto laundryDto)
        {
            var laundry=new Laundry { Name=laundryDto.LaundryName, CreatedAt=DateTime.Now, UpdatedAt=DateTime.Now};
            await laundryRepo.Create(laundry);
            var rowsChanged=await unitOfWork.SaveAsync();
            if (rowsChanged != 1) return new Guid();
            return laundry.Id;
        }
    }
}
