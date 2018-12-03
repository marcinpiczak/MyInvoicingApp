using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyInvoicingApp.Models;

namespace MyInvoicingApp.Contexts
{
    public class EFCDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<InvoiceLine> InvoiceLines { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Budget> Budgets { get; set; }

        public DbSet<DocumentSequence> DocumentSequences { get; set; }

        public DbSet<DocumentNumber> DocumentNumbers { get; set; }

        public DbSet<Attachment> Attachments { get; set; }

        //public DbSet<ModuleAccess> ModuleAccesses { get; set; }

        public DbSet<RoleModuleAccess> RoleModuleAccesses { get; set; }

        public DbSet<UserModuleAccess> UserModuleAccesses { get; set; }

        public EFCDbContext(DbContextOptions<EFCDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //InvoiceLines
            builder.Entity<InvoiceLine>()
                .Property(x => x.Price).HasColumnType("money");
            builder.Entity<InvoiceLine>()
                .Property(x => x.Netto).HasColumnType("money");
            builder.Entity<InvoiceLine>()
                .Property(x => x.Tax).HasColumnType("money");
            builder.Entity<InvoiceLine>()
                .Property(x => x.Gross).HasColumnType("money");
            builder.Entity<InvoiceLine>()
                .Property(x => x.BaseNetto).HasColumnType("money");
            builder.Entity<InvoiceLine>()
                .Property(x => x.BaseTax).HasColumnType("money");
            builder.Entity<InvoiceLine>()
                .Property(x => x.BaseGross).HasColumnType("money");
            builder.Entity<InvoiceLine>()
                .Property(x => x.TaxRate).HasColumnType("money");
            builder.Entity<InvoiceLine>()
                .Property(x => x.CurrencyRate).HasColumnType("money");

            //Budgets
            builder.Entity<Budget>()
                .Property(x => x.CommitedAmount).HasColumnType("money");
            builder.Entity<Budget>()
                .Property(x => x.InvoicedAmount).HasColumnType("money");

            //dodane z powodu błędu o multiple cascade paths
            builder.Entity<InvoiceLine>()
                .HasOne(x => x.Budget)
                .WithMany(x => x.InvoiceLines)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<InvoiceLine>()
                .HasOne(x => x.Invoice)
                .WithMany(x => x.InvoiceLines)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Budget>()
                .HasOne(x => x.CreatedBy)
                .WithMany(x => x.CreatedBudgets)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Budget>()
                .HasOne(x => x.LastModifiedBy)
                .WithMany(x => x.LastModifiedBudgets);

            builder.Entity<Budget>()
                .HasOne(x => x.Owner)
                .WithMany(x => x.OwnedBudgets);

            builder.Entity<Customer>()
                .HasOne(x => x.CreatedBy)
                .WithMany(x => x.CreatedCustomers)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DocumentNumber>()
                .HasOne(x => x.DocumentSequence)
                .WithMany(x => x.DocumentNumbers)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserModuleAccess>()
                .HasOne(x => x.CreatedBy)
                .WithMany(x => x.CreatedUserModuleAccesses)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserModuleAccess>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserModuleAccesses)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Invoice>()
                .HasOne(x => x.Owner)
                .WithMany(x => x.OwnedInvoices)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Invoice>()
                .HasOne(x => x.CreatedBy)
                .WithMany(x => x.CreatedInvoices);

            //primary key for ModuleAccesses
            //builder.Entity<ModuleAccess>()
            //    .HasIndex(x => new {x.RoleId, x.UserId, x.Module})
            //    .IsUnique();
            //.HasKey(x => new { x.RoleId, x.UserId, x.Module });

            builder.Entity<RoleModuleAccess>()
                .HasKey(x => new  { x.AccessorId, x.Module });

            builder.Entity<UserModuleAccess>()
                .HasKey(x => new { x.AccessorId, x.Module });

        }
    }
}