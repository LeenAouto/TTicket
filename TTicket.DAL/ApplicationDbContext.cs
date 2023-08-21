using Microsoft.EntityFrameworkCore;
using TTicket.Abstractions.Security;
using TTicket.Models;

namespace TTicket.DAL
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IPasswordHasher _hasher;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, 
            IPasswordHasher hasher) : base(options)
        {
            _hasher = hasher;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Creating the manager account
            modelBuilder.Entity<User>()
                .HasData( new User { 
                    Id = Guid.NewGuid(),
                    Username = "manager",
                    Password = _hasher.Hash("p@ssword123"), 
                    Email = "leen.aouto@gmail.com",
                    MobilePhone = "0545529216",
                    FirstName = "Leen",
                    LastName = "Aouto",
                    DateOfBirth = new DateTime(2000, 8, 26),
                    Address = "Saudi Arabia, Qassim, Buraydah",
                    TypeUser = UserType.Manager,
                    StatusUser = UserStatus.Active
                }
                );

            //Setting Guid to be auto-gnerated 
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Ticket>()
                .Property(t => t.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Comment>()
                .Property(c => c.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Attachment>()
                .Property(a => a.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Product>()
                .Property(p => p.Id)
                .HasDefaultValueSql("NEWID()");

            //Setting two FKs to the User table from Ticket table
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.User)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Support)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
        }

        public DbSet<User> User { get; set; }
        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Attachment> Attachment { get; set; }
        public DbSet<Product> Product { get; set; }
    }
}