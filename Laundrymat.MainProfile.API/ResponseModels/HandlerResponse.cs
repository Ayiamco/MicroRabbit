
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laundromat.MainProfile.API.ResponseModels
{
    public enum HandlerResponseStatus
    {
        Failed, Succeeded
    }
    public class HandlerResponse<T>
    {
        public HandlerResponseStatus Status { get; set; }

        public APIResponse<T> Data { get; set; }
    }
}
