using ClientService.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientService.Data
{
    public class HomeBankingContext : DbContext
    {
        public HomeBankingContext(DbContextOptions<HomeBankingContext> options) : base(options) { }

        //dbsets
        public DbSet<Client> Clients { get; set; }
    }
}
