using HRMS.Data.Core;
using HRMS.Data.SqlFunctions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        #region SQL Functions

        [NotMapped]
        public DbSet<MenuList> MenuList { get; set; }

        [NotMapped]
        public DbSet<MenuListAccess> MenuListAccess { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().HasIndex(a => new { a.PersonalNumber }).IsUnique(true);
            builder.Entity<MenuList>().HasNoKey();
            builder.Entity<MenuListAccess>().HasNoKey();

            base.OnModelCreating(builder);
        }
    }
}
