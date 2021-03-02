using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement_usingJwt.Models
{
    public class UserDbContext : IdentityDbContext
    {
        public UserDbContext(DbContextOptions options) : base(options)
        {
             
        }

        public DbSet<Profile> Profiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); 
        }
    }
}
