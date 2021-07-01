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
        
        public IActionResult UpdateLaundry([FromBody] Update updateLaundryModel )
        {
            mediator.Send(upd)
        }
            
    }
}
