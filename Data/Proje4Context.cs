using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Proje4.Models;

namespace Proje4.Data
{
    public class Proje4Context : DbContext
    {
        public Proje4Context (DbContextOptions<Proje4Context> options)
            : base(options)
        {
        }

        public DbSet<Proje4.Models.User> User { get; set; } = default!;
        public DbSet<Proje4.Models.Project> Project { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .HasOne(p => p.User)
                .WithOne(m => m.Project)
                .HasForeignKey<Project>(p => p.UserId);
            



            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();


        }


        public DbSet<Proje4.Models.Message> Message { get; set; } = default!;
        public DbSet<Proje4.Models.Task> Task { get; set; } = default!;
    }
}
