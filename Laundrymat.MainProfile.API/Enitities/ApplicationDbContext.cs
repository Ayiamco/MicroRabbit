using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laundromat.MainProfile.API.Enitities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        DbSet<UserProfile> UserProfiles { get; set; }
        DbSet<Location> Locations { get; set; }
        DbSet<Laundry> Laundries { get; set; }
        

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Customer>().HasAlternateKey(vf => new { vf.Username, vf.LaundryId});
        //}

    }
}
