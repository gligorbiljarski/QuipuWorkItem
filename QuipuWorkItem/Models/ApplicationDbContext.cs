// Models/ApplicationDbContext.cs
using System.Data.Entity;

namespace QuipuWorkItem.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("ApplicationDbContext")
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Address> Addresses { get; set; }
    }
}
