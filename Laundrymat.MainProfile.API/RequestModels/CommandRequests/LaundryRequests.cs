using Laundromat.MainProfile.API.Enitities;
using Laundromat.MainProfile.API.ResponseModels;
using Laundromat.SharedKernel.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laundromat.MainProfile.API.RequestModels.CommandRequests
{
    public class UpdateLaundryRequestModel : IRequest<HandlerResponse<string>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Location Address { get; set; }
    }

    public class AddLaundryRequestModel : NewLaundry, IRequest<HandlerResponse<Guid>>
    {
        
    }
}
