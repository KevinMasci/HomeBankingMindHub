using Microsoft.EntityFrameworkCore;

namespace HomeBankingMindHub.Models
{
    public class HomeBankingContext : DbContext
    {
        public HomeBankingContext(DbContextOptions<HomeBankingContext> options) : base(options) { }
    }
}
