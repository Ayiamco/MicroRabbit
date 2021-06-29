using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Laundromat.SharedKernel.Core
{
    public class EmailMessage
    {
        public List<string> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public IFormFileCollection Attachments { get; set; }
    }
}
