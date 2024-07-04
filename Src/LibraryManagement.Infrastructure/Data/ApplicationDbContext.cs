using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using LibraryManagement.Core.Entities.Identity;
using LibraryManagement.Core.Entities;

namespace LibraryManagement.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Books> books { get; set; }
        public DbSet<Patron> patron { get; set; }
        public DbSet<BorrowingRecord> borrowingRecord { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Books>().
                HasMany(b => b.BorrowingRecords).
                WithOne(u => u.Book).
                HasForeignKey(t=>t.BookId);

            builder.Entity<ApplicationUser>().
              HasMany(b => b.book).
              WithOne(u => u.applicationUser).
              HasForeignKey(t => t.UserId);


            builder.Entity<Patron>().
               HasMany(b => b.BorrowingRecords).
               WithOne(c => c.patron).
               HasForeignKey(t=>t.PatronId);

            builder.Entity<ApplicationUser>().
              HasMany(b => b.patrons).
              WithOne(u => u.applicationUser).
              HasForeignKey(t => t.UserId);

            builder.Entity<ApplicationUser>().
              HasMany(b => b.borrowingRecords).
              WithOne(u => u.applicationUser).
              HasForeignKey(t => t.UserId);

            base.OnModelCreating(builder);
        }
    }
}

