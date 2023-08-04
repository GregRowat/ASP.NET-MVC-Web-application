using Lab4.Models;
using Microsoft.EntityFrameworkCore;

// ef class to represent a combination of the unit of work and repo patterns to faciliate the querying of database as a stored unit

namespace Lab4.Data
{
    public class NewsDbContext : DbContext 
    {
        public NewsDbContext(DbContextOptions<NewsDbContext> options) : base(options) 
        { 
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<NewsBoard> NewsBoards { get; set; }

        public DbSet<News> News { get; set; }     

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NewsBoard>().ToTable("NewsBoard");
            modelBuilder.Entity<Client>().ToTable("Client");
            modelBuilder.Entity<Subscription>().ToTable("Subscription");
            modelBuilder.Entity<News>().ToTable("News");

            modelBuilder.Entity<Subscription>().HasKey(c => new {c.ClientId, c.NewsBoardId});
        }

    }
}
