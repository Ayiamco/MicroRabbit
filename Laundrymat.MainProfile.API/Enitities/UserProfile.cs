using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laundromat.MainProfile.API.Enitities
{

    public enum Gender
    {
        Male, Female, NonBinary
    }
    public class UserProfile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Location Address { get; set; }
        public Gender Gender { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
