using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext, IApplicationDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }

        IQueryable<Category> IApplicationDbContext.Categories => Categories;
        IQueryable<Transaction> IApplicationDbContext.Transactions => Transactions;
        IQueryable<User> IApplicationDbContext.Users => Users;

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            base.Add(entity);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optional: Configure relationships explicitly if you want to be safe
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId);
                
            // Good practice: Seed some default data so you aren't testing on empty tables
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Food" },
                new Category { Id = 2, Name = "Rent" }
            );

            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), 
                    Username = "admin", 
                    PasswordHash = "password", // In real apps, hash this!
                    Role = "Admin"
                },
                new User 
                { 
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), 
                    Username = "user", 
                    PasswordHash = "password", 
                    Role = "User"
                }
            );
        }
    }
}