using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laundromat.MainProfile.API.Enitities
{
    public class Laundry
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Location Address { get; set; }
        public int AddressId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
