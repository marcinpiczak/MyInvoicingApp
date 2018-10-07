﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyInvoicingApp.Contexts;

namespace MyInvoicingApp.Migrations
{
    [DbContext(typeof(EFCDbContext))]
    [Migration("20180917164041_add_base_table")]
    partial class add_base_table
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.2-rtm-30932")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("MyInvoicingApp.Models.ApplicationRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("MyInvoicingApp.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("MyInvoicingApp.Models.Budget", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BudgetNumber")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<decimal>("CommitedAmount")
                        .HasColumnType("money");

                    b.Property<string>("CreatedById")
                        .IsRequired();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<decimal>("InvoicesAmount")
                        .HasColumnType("money");

                    b.Property<string>("LastModifiedById");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("OwnerId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("LastModifiedById");

                    b.HasIndex("OwnerId");

                    b.ToTable("Budgets");
                });

            modelBuilder.Entity("MyInvoicingApp.Models.Customer", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("CreatedById")
                        .IsRequired();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("DefaultPaymentMethod")
                        .HasMaxLength(50);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("LastModifiedById");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Notes");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("LastModifiedById");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("MyInvoicingApp.Models.Invoice", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Budget");

                    b.Property<string>("CreatedById")
                        .IsRequired();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasMaxLength(5);

                    b.Property<string>("CustomerId")
                        .IsRequired();

                    b.Property<string>("InvoiceNumber")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<DateTime>("IssueDate");

                    b.Property<string>("LastModifiedById");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<DateTime>("PaymentDueDate");

                    b.Property<string>("PaymentMethod")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("CustomerId");

                    b.HasIndex("LastModifiedById");

                    b.ToTable("Invoices");
                });

            modelBuilder.Entity("MyInvoicingApp.Models.InvoiceLine", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("BaseGross")
                        .HasColumnType("money");

                    b.Property<decimal>("BaseNetto")
                        .HasColumnType("money");

                    b.Property<decimal>("BaseTax")
                        .HasColumnType("money");

                    b.Property<string>("BudgetId")
                        .IsRequired();

                    b.Property<string>("CreatedById")
                        .IsRequired();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasMaxLength(5);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<decimal>("Gross")
                        .HasColumnType("money");

                    b.Property<string>("InvoiceId")
                        .IsRequired();

                    b.Property<string>("ItemName")
                        .IsRequired();

                    b.Property<string>("LastModifiedById");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<int>("LineNumber");

                    b.Property<decimal>("Netto")
                        .HasColumnType("money");

                    b.Property<int>("Quantity");

                    b.Property<decimal>("Tax")
                        .HasColumnType("money");

                    b.Property<string>("TaxRate")
                        .IsRequired()
                        .HasMaxLength(5);

                    b.HasKey("Id");

                    b.HasIndex("BudgetId");

                    b.HasIndex("CreatedById");

                    b.HasIndex("InvoiceId");

                    b.HasIndex("LastModifiedById");

                    b.ToTable("InvoiceLines");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("MyInvoicingApp.Models.ApplicationRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("MyInvoicingApp.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("MyInvoicingApp.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("MyInvoicingApp.Models.ApplicationRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyInvoicingApp.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("MyInvoicingApp.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyInvoicingApp.Models.Budget", b =>
                {
                    b.HasOne("MyInvoicingApp.Models.ApplicationUser", "CreatedBy")
                        .WithMany("CreatedBudgets")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("MyInvoicingApp.Models.ApplicationUser", "LastModifiedBy")
                        .WithMany("LastModifiedBudgets")
                        .HasForeignKey("LastModifiedById");

                    b.HasOne("MyInvoicingApp.Models.ApplicationUser", "Owner")
                        .WithMany("OwnedBudgets")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyInvoicingApp.Models.Customer", b =>
                {
                    b.HasOne("MyInvoicingApp.Models.ApplicationUser", "CreatedBy")
                        .WithMany("CreatedCustomers")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("MyInvoicingApp.Models.ApplicationUser", "LastModifiedBy")
                        .WithMany("LastModifiedCustomers")
                        .HasForeignKey("LastModifiedById");
                });

            modelBuilder.Entity("MyInvoicingApp.Models.Invoice", b =>
                {
                    b.HasOne("MyInvoicingApp.Models.ApplicationUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyInvoicingApp.Models.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyInvoicingApp.Models.ApplicationUser", "LastModifiedBy")
                        .WithMany()
                        .HasForeignKey("LastModifiedById");
                });

            modelBuilder.Entity("MyInvoicingApp.Models.InvoiceLine", b =>
                {
                    b.HasOne("MyInvoicingApp.Models.Budget", "Budget")
                        .WithMany("InvoiceLines")
                        .HasForeignKey("BudgetId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("MyInvoicingApp.Models.ApplicationUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyInvoicingApp.Models.Invoice", "Invoice")
                        .WithMany("InvoiceLines")
                        .HasForeignKey("InvoiceId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("MyInvoicingApp.Models.ApplicationUser", "LastModifiedBy")
                        .WithMany()
                        .HasForeignKey("LastModifiedById");
                });
#pragma warning restore 612, 618
        }
    }
}
