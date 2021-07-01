using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laundromat.MainProfile.API
{
    public class APIResponse<T>
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public string Errors { get; set; }
        public T Data { get; set; }
    }
}
