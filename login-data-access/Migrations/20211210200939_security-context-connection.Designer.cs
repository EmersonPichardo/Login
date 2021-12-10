﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using login_data_access.Contexts.SecurityContext;

namespace login_data_access.Migrations
{
    [DbContext(typeof(SecurityContext))]
    [Migration("20211210200939_security-context-connection")]
    partial class securitycontextconnection
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.12");

            modelBuilder.Entity("login_data_access.Contexts.SecurityContext.Models.Sesion", b =>
                {
                    b.Property<byte[]>("Token")
                        .HasColumnType("varbinary(3072)");

                    b.Property<int>("User_Id")
                        .HasColumnType("int")
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("ValidSince")
                        .HasColumnType("datetime");

                    b.HasKey("Token")
                        .HasName("pk_sesions");

                    b.HasIndex("User_Id");

                    b.ToTable("sesions");
                });

            modelBuilder.Entity("login_data_access.Contexts.SecurityContext.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(75)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<byte[]>("Password")
                        .IsRequired()
                        .HasColumnType("varbinary(20)");

                    b.Property<byte[]>("Salt")
                        .IsRequired()
                        .HasColumnType("varbinary(8)");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("Name")
                        .HasDatabaseName("ind_users_name");

                    b.ToTable("users");
                });

            modelBuilder.Entity("login_data_access.Contexts.SecurityContext.Models.Sesion", b =>
                {
                    b.HasOne("login_data_access.Contexts.SecurityContext.Models.User", null)
                        .WithMany()
                        .HasForeignKey("User_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
