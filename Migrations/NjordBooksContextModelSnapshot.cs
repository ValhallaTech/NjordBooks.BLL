﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NjordBooks.BLL.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace NjordBooks.BLL.Migrations
{
    [DbContext(typeof(NjordBooksContext))]
    partial class NjordBooksContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("NjordBooks.BLL.Models.Attachment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasColumnType("character varying(40)")
                        .HasMaxLength(40);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("character varying(40)")
                        .HasMaxLength(40);

                    b.Property<byte[]>("FileData")
                        .HasColumnType("bytea");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("character varying(40)")
                        .HasMaxLength(40);

                    b.Property<int>("HouseholdId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("HouseholdId");

                    b.ToTable("Attachment");
                });

            modelBuilder.Entity("NjordBooks.BLL.Models.BankAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal?>("CurrentBalance")
                        .HasColumnType("decimal(10,2)");

                    b.Property<int>("HouseholdId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying(40)")
                        .HasMaxLength(40);

                    b.Property<string>("NjordBooksUserId")
                        .HasColumnType("character varying(40)")
                        .HasMaxLength(40);

                    b.Property<decimal>("StartingBalance")
                        .HasColumnType("decimal(10,2)");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("HouseholdId");

                    b.HasIndex("NjordBooksUserId");

                    b.ToTable("BankAccount");
                });

            modelBuilder.Entity("NjordBooks.BLL.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("character varying(40)")
                        .HasMaxLength(40);

                    b.Property<int>("HouseholdId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying(40)")
                        .HasMaxLength(40);

                    b.HasKey("Id");

                    b.HasIndex("HouseholdId");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("NjordBooks.BLL.Models.CategoryItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal>("ActualAmount")
                        .HasColumnType("decimal(6,2)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("character varying(40)")
                        .HasMaxLength(40);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying(40)")
                        .HasMaxLength(40);

                    b.Property<decimal>("TargetAmount")
                        .HasColumnType("decimal(6,2)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("CategoryItem");
                });

            modelBuilder.Entity("NjordBooks.BLL.Models.History", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal>("Balance")
                        .HasColumnType("numeric");

                    b.Property<int>("BankAccountId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("BankAccountId");

                    b.ToTable("History");
                });

            modelBuilder.Entity("NjordBooks.BLL.Models.Household", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTimeOffset>("Established")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Greeting")
                        .IsRequired()
                        .HasColumnType("character varying(40)")
                        .HasMaxLength(40);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying(40)")
                        .HasMaxLength(40);

                    b.HasKey("Id");

                    b.ToTable("Household");
                });

            modelBuilder.Entity("NjordBooks.BLL.Models.Invitation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("Accepted")
                        .HasColumnType("boolean");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("character varying(180)")
                        .HasMaxLength(180);

                    b.Property<Guid>("Code")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("EmailTo")
                        .IsRequired()
                        .HasColumnType("character varying(40)")
                        .HasMaxLength(40);

                    b.Property<DateTimeOffset>("Expires")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("HouseholdId")
                        .HasColumnType("integer");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasColumnType("character varying(40)")
                        .HasMaxLength(40);

                    b.HasKey("Id");

                    b.HasIndex("HouseholdId");

                    b.ToTable("Invitation");
                });

            modelBuilder.Entity("NjordBooks.BLL.Models.NjordBooksUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<byte[]>("FileData")
                        .HasColumnType("bytea");

                    b.Property<string>("FileName")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<int?>("HouseholdId")
                        .HasColumnType("integer");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("HouseholdId");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("NjordBooks.BLL.Models.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("character varying(300)")
                        .HasMaxLength(300);

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("HouseholdId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsRead")
                        .HasColumnType("boolean");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasColumnType("character varying(40)")
                        .HasMaxLength(40);

                    b.HasKey("Id");

                    b.HasIndex("HouseholdId");

                    b.ToTable("Notification");
                });

            modelBuilder.Entity("NjordBooks.BLL.Models.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(10,2)");

                    b.Property<int>("BankAccountId")
                        .HasColumnType("integer");

                    b.Property<int?>("CategoryItemId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Memo")
                        .IsRequired()
                        .HasColumnType("character varying(40)")
                        .HasMaxLength(40);

                    b.Property<string>("NjordBooksUserId")
                        .HasColumnType("character varying(40)")
                        .HasMaxLength(40);

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BankAccountId");

                    b.HasIndex("CategoryItemId");

                    b.HasIndex("NjordBooksUserId");

                    b.ToTable("Transaction");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("NjordBooks.BLL.Models.NjordBooksUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("NjordBooks.BLL.Models.NjordBooksUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NjordBooks.BLL.Models.NjordBooksUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("NjordBooks.BLL.Models.NjordBooksUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NjordBooks.BLL.Models.Attachment", b =>
                {
                    b.HasOne("NjordBooks.BLL.Models.Household", "Household")
                        .WithMany("Attachments")
                        .HasForeignKey("HouseholdId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NjordBooks.BLL.Models.BankAccount", b =>
                {
                    b.HasOne("NjordBooks.BLL.Models.Household", "Household")
                        .WithMany("BankAccounts")
                        .HasForeignKey("HouseholdId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NjordBooks.BLL.Models.NjordBooksUser", "NjordBooksUser")
                        .WithMany("BankAccounts")
                        .HasForeignKey("NjordBooksUserId");
                });

            modelBuilder.Entity("NjordBooks.BLL.Models.Category", b =>
                {
                    b.HasOne("NjordBooks.BLL.Models.Household", "Household")
                        .WithMany("Categories")
                        .HasForeignKey("HouseholdId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NjordBooks.BLL.Models.CategoryItem", b =>
                {
                    b.HasOne("NjordBooks.BLL.Models.Category", "Category")
                        .WithMany("CategoryItems")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NjordBooks.BLL.Models.History", b =>
                {
                    b.HasOne("NjordBooks.BLL.Models.BankAccount", "BankAccount")
                        .WithMany("Histories")
                        .HasForeignKey("BankAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NjordBooks.BLL.Models.Invitation", b =>
                {
                    b.HasOne("NjordBooks.BLL.Models.Household", "Household")
                        .WithMany("Invitations")
                        .HasForeignKey("HouseholdId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NjordBooks.BLL.Models.NjordBooksUser", b =>
                {
                    b.HasOne("NjordBooks.BLL.Models.Household", "Household")
                        .WithMany("NjordBooksUsers")
                        .HasForeignKey("HouseholdId");
                });

            modelBuilder.Entity("NjordBooks.BLL.Models.Notification", b =>
                {
                    b.HasOne("NjordBooks.BLL.Models.Household", "Household")
                        .WithMany("Notifications")
                        .HasForeignKey("HouseholdId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NjordBooks.BLL.Models.Transaction", b =>
                {
                    b.HasOne("NjordBooks.BLL.Models.BankAccount", "BankAccount")
                        .WithMany("Transactions")
                        .HasForeignKey("BankAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NjordBooks.BLL.Models.CategoryItem", "CategoryItem")
                        .WithMany("Transactions")
                        .HasForeignKey("CategoryItemId");

                    b.HasOne("NjordBooks.BLL.Models.NjordBooksUser", "NjordBooksUser")
                        .WithMany("Transactions")
                        .HasForeignKey("NjordBooksUserId");
                });
#pragma warning restore 612, 618
        }
    }
}
