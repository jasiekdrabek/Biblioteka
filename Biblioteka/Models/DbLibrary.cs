using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Biblioteka.Models
{
    public partial class DbLibrary : DbContext
    {
        public DbLibrary()
            : base("name=DbLibrary")
        {
        }

        public virtual DbSet<Book> Book { get; set; }
        public virtual DbSet<Loan> Loan { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .Property(e => e.Name)
                .IsFixedLength();

            modelBuilder.Entity<Book>()
                .Property(e => e.Author)
                .IsFixedLength();

            modelBuilder.Entity<User>()
                .Property(e => e.Name)
                .IsFixedLength();

            modelBuilder.Entity<User>()
                .Property(e => e.Password)
                .IsFixedLength();

            modelBuilder.Entity<User>()
                .Property(e => e.Role)
                .IsFixedLength();
        }
    }
}
