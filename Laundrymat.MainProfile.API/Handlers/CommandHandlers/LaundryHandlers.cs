using Laundromat.MainProfile.API.Enitities;
using Laundromat.MainProfile.API.Repositories;
using Laundromat.MainProfile.API.RequestModels.CommandRequests;
using Laundromat.MainProfile.API.ResponseModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Laundromat.MainProfile.API.Handlers.CommandHandlers
{
    
    public abstract  class LaundryCommandHandler
    {
        protected readonly ILaundryRepo laundryRepo;
        protected readonly IUnitOfWork unitOfWork;

        public LaundryCommandHandler(ILaundryRepo laundryRepo, IUnitOfWork unitOfWork)
        {
            this.laundryRepo = laundryRepo;
            this.unitOfWork = unitOfWork;
        }
    }
    public class UpdateLaundryCommandHandler :LaundryCommandHandler, IRequestHandler<UpdateLaundryRequestModel, HandlerResponse<string>>
    {
        public UpdateLaundryCommandHandler(ILaundryRepo laundryRepo, IUnitOfWork unitOfWork):
            base(laundryRepo,unitOfWork)
        {
            
        }
        public async Task<HandlerResponse<string>> Handle(UpdateLaundryRequestModel request, CancellationToken cancellationToken)
        {
            var result = new HandlerResponse<string>
            {
                Status = HandlerResponseStatus.Succeeded,
                Data = new APIResponse<string>
                {
                    Status="success"
                }
            };

            await unitOfWork.SaveAsync();
            return result;
        }

       
    }

    public class AddLaundryCommandHandler :LaundryCommandHandler, IRequestHandler<AddLaundryRequestModel, HandlerResponse<Guid>>
    {
        public AddLaundryCommandHandler(ILaundryRepo laundryRepo,IUnitOfWork unitOfWork):
            base(laundryRepo,unitOfWork)
        {

        }

        public async Task<HandlerResponse<Guid>> Handle(AddLaundryRequestModel laundryDto, CancellationToken cancellationToken)
        {
            var laundry = new Laundry { Name = laundryDto.LaundryName, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
            await laundryRepo.Create(laundry);
            var rowsChanged = await unitOfWork.SaveAsync();
            if (rowsChanged != 1) return new HandlerResponse<Guid> { Status=HandlerResponseStatus.Failed,
                Data= new APIResponse<Guid> {Status="failed",Message="laundry Failed to save" }
            };
            return new HandlerResponse<Guid> { Status=HandlerResponseStatus.Succeeded,
                Data= new APIResponse<Guid> {Status="success",Data=laundry.Id } };
        }
    }
}
