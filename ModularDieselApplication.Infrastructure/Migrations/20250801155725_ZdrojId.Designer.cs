﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ModularDieselApplication.Infrastructure.Persistence;

#nullable disable

namespace ModularDieselApplication.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250801155725_ZdrojId")]
    partial class ZdrojId
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Identity")
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("Role", "Identity");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims", "Identity");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims", "Identity");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins", "Identity");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles", "Identity");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens", "Identity");
                });

            modelBuilder.Entity("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.DebugLogModel", b =>
                {
                    b.Property<string>("IdLog")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("IdDieslovani")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("IdOdstavky")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LogMessage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OdstavkyID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("IdLog");

                    b.HasIndex("IdDieslovani");

                    b.HasIndex("OdstavkyID");

                    b.ToTable("DebugModel", "Data");
                });

            modelBuilder.Entity("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableDieslovani", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("IdOdstavky")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("IdTechnik")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Odchod")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Vstup")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.HasIndex("IdOdstavky");

                    b.HasIndex("IdTechnik");

                    b.ToTable("TableDieslovani", "Data");
                });

            modelBuilder.Entity("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableFirma", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Nazev")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("TableFirma", "Data");
                });

            modelBuilder.Entity("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableLokalita", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Adresa")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Baterie")
                        .HasColumnType("int");

                    b.Property<bool>("DA")
                        .HasColumnType("bit");

                    b.Property<string>("Klasifikace")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nazev")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegionID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("Zasuvka")
                        .HasColumnType("bit");

              

                    b.HasKey("ID");

                    b.HasIndex("RegionID");


                    b.ToTable("TableLokalita", "Data");
                });

            modelBuilder.Entity("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableOdstavka", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Distributor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Do")
                        .HasColumnType("datetime2");

                    b.Property<string>("LokalitaID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("LokalitaID");

                    b.Property<DateTime>("Od")
                        .HasColumnType("datetime2");

                    b.Property<string>("Popis")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("LokalitaID");

                    b.ToTable("TableOdstavka", "Data");
                });

            modelBuilder.Entity("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TablePohotovost", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("IdTechnik")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Konec")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Zacatek")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.HasIndex("IdTechnik");

                    b.ToTable("TablePohotovost", "Data");
                });

            modelBuilder.Entity("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableRegion", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FirmaID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Nazev")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("FirmaID");

                    b.ToTable("TableRegion", "Data");
                });

            modelBuilder.Entity("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableTechnik", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FirmaId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("IdUser")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("Taken")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("FirmaId");

                    b.HasIndex("IdUser");

                    b.ToTable("TableTechnik", "Data");
                });

            modelBuilder.Entity("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("Jmeno")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("Prijmeni")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("User", "Identity");
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
                    b.HasOne("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableUser", null)
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

                    b.HasOne("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.DebugLogModel", b =>
                {
                    b.HasOne("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableDieslovani", "Dieslovani")
                        .WithMany()
                        .HasForeignKey("IdDieslovani");

                    b.HasOne("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableOdstavka", "Odstavky")
                        .WithMany()
                        .HasForeignKey("OdstavkyID");

                    b.Navigation("Dieslovani");

                    b.Navigation("Odstavky");
                });

            modelBuilder.Entity("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableDieslovani", b =>
                {
                    b.HasOne("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableOdstavka", "Odstavka")
                        .WithMany()
                        .HasForeignKey("IdOdstavky")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableTechnik", "Technik")
                        .WithMany("DieslovaniList")
                        .HasForeignKey("IdTechnik")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Odstavka");

                    b.Navigation("Technik");
                });

            modelBuilder.Entity("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableLokalita", b =>
                {
                    b.HasOne("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableRegion", "Region")
                        .WithMany()
                        .HasForeignKey("RegionID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

               
                    b.Navigation("Region");

                });

            modelBuilder.Entity("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableOdstavka", b =>
                {
                    b.HasOne("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableLokalita", "Lokality")
                        .WithMany("OdstavkyList")
                        .HasForeignKey("LokalitaID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Lokality");
                });

            modelBuilder.Entity("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TablePohotovost", b =>
                {
                    b.HasOne("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableTechnik", "Technik")
                        .WithMany()
                        .HasForeignKey("IdTechnik")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Technik");
                });

            modelBuilder.Entity("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableRegion", b =>
                {
                    b.HasOne("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableFirma", "Firma")
                        .WithMany()
                        .HasForeignKey("FirmaID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Firma");
                });

            modelBuilder.Entity("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableTechnik", b =>
                {
                    b.HasOne("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableFirma", "Firma")
                        .WithMany()
                        .HasForeignKey("FirmaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableUser", "User")
                        .WithMany()
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Firma");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableLokalita", b =>
                {
                    b.Navigation("OdstavkyList");
                });

            modelBuilder.Entity("ModularDieselApplication.Infrastructure.Persistence.Entities.Models.TableTechnik", b =>
                {
                    b.Navigation("DieslovaniList");
                });
#pragma warning restore 612, 618
        }
    }
}
