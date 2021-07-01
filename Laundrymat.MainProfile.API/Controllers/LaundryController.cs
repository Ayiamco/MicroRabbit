using Laundromat.MainProfile.API.RequestModels.CommandRequests;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laundromat.MainProfile.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaundryController : ControllerBase
    {
        private IMediator mediator;

        public LaundryController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        
        [HttpPost]
        public async Task<IActionResult> UpdateLaundry([FromBody] UpdateLaundryRequestModel updateLaundryModel )
        {
           var resp=await  mediator.Send(updateLaundryModel);

            return Ok();
        }
            
    }
}
