﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using login_data_access.Contexts.SecurityContext;

namespace login_data_access.Migrations
{
    [DbContext(typeof(SecurityContext))]
    partial class SecurityContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.12");

            modelBuilder.Entity("login_data_access.Contexts.SecurityContext.Models.Application", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("char(32)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(16)");

                    b.Property<string>("Status")
                        .HasColumnType("longtext");

                    b.HasKey("Id")
                        .HasName("pk_applications");

                    b.HasIndex("Name")
                        .HasDatabaseName("ind_applications_name");

                    b.ToTable("applications");
                });

            modelBuilder.Entity("login_data_access.Contexts.SecurityContext.Models.Sesion", b =>
                {
                    b.Property<byte[]>("Token")
                        .HasColumnType("varbinary(16)")
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

                    b.Property<int>("User_Id")
                        .HasColumnType("int");

                    b.Property<DateTime>("ValidUntil")
                        .HasColumnType("datetime");

                    b.HasKey("Token")
                        .HasName("pk_sesions");

                    b.HasIndex("User_Id")
                        .HasDatabaseName("ind_sesions_user_id");

                    b.ToTable("sesions");
                });

            modelBuilder.Entity("login_data_access.Contexts.SecurityContext.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Application_Id")
                        .IsRequired()
                        .HasColumnType("char(32)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(64)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(16)");

                    b.Property<byte[]>("Password")
                        .IsRequired()
                        .HasColumnType("varbinary(128)");

                    b.Property<byte[]>("Salt")
                        .IsRequired()
                        .HasColumnType("varbinary(32)");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("Application_Id")
                        .HasDatabaseName("ind_users_application_id");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("unq_users_email");

                    b.ToTable("users");
                });

            modelBuilder.Entity("login_data_access.Contexts.SecurityContext.Models.Sesion", b =>
                {
                    b.HasOne("login_data_access.Contexts.SecurityContext.Models.User", null)
                        .WithMany()
                        .HasForeignKey("User_Id")
                        .HasConstraintName("fk_sesions_users")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("login_data_access.Contexts.SecurityContext.Models.User", b =>
                {
                    b.HasOne("login_data_access.Contexts.SecurityContext.Models.Application", null)
                        .WithMany()
                        .HasForeignKey("Application_Id")
                        .HasConstraintName("fk_users_applications")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
