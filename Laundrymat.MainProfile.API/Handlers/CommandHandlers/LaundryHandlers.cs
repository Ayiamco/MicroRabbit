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
    public class UpdateLaundryCommandHandler : IRequestHandler<UpdateLaundryRequestModel, HandlerResponse<string>>
    {
        private readonly ILaundryRepo laundryRepo;
        private readonly IUnitOfWork unitOfWork;

        public UpdateLaundryCommandHandler(ILaundryRepo laundryRepo, IUnitOfWork unitOfWork)
        {
            this.laundryRepo = laundryRepo;
            this.unitOfWork = unitOfWork;
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
}
