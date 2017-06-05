using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace PortalSlubny.Models
{
    public class EfDbContext : IdentityDbContext<ApplicationUser>
    {
        public EfDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static EfDbContext Create()
        {
            return new EfDbContext();
        }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Company> Companies { get; set; }

        public virtual DbSet<City> Cities { get; set; }

        public virtual DbSet<Grade> Grades { get; set; }

        public virtual DbSet<Comment> Comments { get; set; }

        public virtual DbSet<Image> Images { get; set; }
    }
}