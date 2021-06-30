using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laundromat.MainProfile.API.Enitities
{
    public class Location
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string LGA { get; set; }
        public string Street { get; set; }
    }
}
