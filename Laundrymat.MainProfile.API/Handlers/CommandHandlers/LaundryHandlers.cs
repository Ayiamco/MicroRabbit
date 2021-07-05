using AutoMapper;
using Laundromat.MainProfile.API.Enitities;
using Laundromat.MainProfile.API.Repositories;
using Laundromat.MainProfile.API.RequestModels.CommandRequests;
using Laundromat.MainProfile.API.ResponseModels;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Laundromat.MainProfile.API.Handlers.CommandHandlers
{
    
    public abstract  class LaundryCommandHandler
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IMapper mapper;

        public LaundryCommandHandler( IUnitOfWork unitOfWork,IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
    }

    public class UpdateLaundryCommandHandler :LaundryCommandHandler, IRequestHandler<UpdateLaundryRequestModel, HandlerResponse<string>>
    {
        public UpdateLaundryCommandHandler( IUnitOfWork unitOfWork,IMapper mapper):
            base(unitOfWork,mapper)
        {
            
        }
        public async Task<HandlerResponse<string>> Handle(UpdateLaundryRequestModel requestData, CancellationToken cancellationToken)
        {
            var laundryInDb=await  unitOfWork.LaundryRepo.Read(requestData.Id);
            if(laundryInDb == null) return new HandlerResponse<string>
            {
                Status = HandlerResponseStatus.Failed,
                Data = new APIResponse<string>{ Status = "failed", Message = "laundry does not exist",
                    Errors = JsonConvert.SerializeObject(new { laundryId = new string[] {"laundry Id is not a valid laundry " } })
                }
            };

            var updatedLaundry =mapper.Map<Laundry>(requestData);
            mapper.Map(updatedLaundry, laundryInDb);
            var rowsAffected=await unitOfWork.SaveAsync();
            if (rowsAffected ==1 ) return new HandlerResponse<string>
            {
                Status = HandlerResponseStatus.Succeeded,
                Data = new APIResponse<string>{ Status = "success", Message= $"{laundryInDb.Name} laundry updated successfully"}
            };

            return  new HandlerResponse<string>
            {
                Status = HandlerResponseStatus.Failed,
                Data = new APIResponse<string>{Status = "failed", Message="failed to update laundry"}
            };      
        } 
    }

    public class AddLaundryCommandHandler :LaundryCommandHandler, IRequestHandler<AddLaundryRequestModel, HandlerResponse<Guid>>
    {
        public AddLaundryCommandHandler(IUnitOfWork unitOfWork,IMapper mapper):base(unitOfWork,mapper)
        {
        }

        public async Task<HandlerResponse<Guid>> Handle(AddLaundryRequestModel requestData, CancellationToken cancellationToken)
        {
            var laundry = mapper.Map<Laundry>(requestData);
            laundry.Address = new Location();
            await unitOfWork.LaundryRepo.Create(laundry);
            var rowsChanged = await unitOfWork.SaveAsync();
            if (rowsChanged != 2) return new HandlerResponse<Guid> { Status=HandlerResponseStatus.Failed,
                Data= new APIResponse<Guid> {Status="failed",Message="laundry Failed to save" }
            };
            return new HandlerResponse<Guid> { Status=HandlerResponseStatus.Succeeded,
                Data= new APIResponse<Guid> {Status="success",Data=laundry.Id } };
        }
    }
}
