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
        public UpdateLaundryCommandHandler(ILaundryRepo laundryRepo)
        {
            this.laundryRepo = laundryRepo;
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

            
            // Your business logic here

            return result;
        }
    }
}
