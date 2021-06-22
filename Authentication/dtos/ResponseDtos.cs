using Authentication.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.dtos
{
    public class ServiceResponse
    {
        public AppServiceResult Result { get; set; }
        public string Data { get; set; }
    }
}
