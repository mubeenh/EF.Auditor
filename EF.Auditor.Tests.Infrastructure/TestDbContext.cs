using EF.Auditor.Tests.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace EF.Auditor.Tests.Infrastructure
{
    public class TestDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Magazine> Magazines { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public readonly Auditor Auditor;

        public TestDbContext()
            : base("Commons.EF.TestDB")
        {
            Auditor = new Auditor(new AuditorContextAdapter(this));
        }

        public void Commit()
        {
            try
            {
                this.Save();
            }
            catch (DbEntityValidationException ex)
            {
                throw new Exception("Error while saving entities.");
            }
        }

        private void Save()
        {
            Auditor.LogAndSaveChanges("testuser");
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasOptional(e => e.Address).WithRequired();
            modelBuilder.Entity<User>().HasMany(e => e.Subscriptions).WithMany();

        }
    }
}