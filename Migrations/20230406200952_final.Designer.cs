﻿// <auto-generated />
using System;
using Lab4.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Lab4.Migrations
{
    [DbContext(typeof(NewsDbContext))]
    [Migration("20230406200952_final")]
    partial class final
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Lab4.Models.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Client", (string)null);
                });

            modelBuilder.Entity("Lab4.Models.News", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NewsBoardId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("NewsBoardId");

                    b.ToTable("News", (string)null);
                });

            modelBuilder.Entity("Lab4.Models.NewsBoard", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("Fee")
                        .HasColumnType("money");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("NewsBoard", (string)null);
                });

            modelBuilder.Entity("Lab4.Models.Subscription", b =>
                {
                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<string>("NewsBoardId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ClientId", "NewsBoardId");

                    b.HasIndex("NewsBoardId");

                    b.ToTable("Subscription", (string)null);
                });

            modelBuilder.Entity("Lab4.Models.News", b =>
                {
                    b.HasOne("Lab4.Models.NewsBoard", "newsboard")
                        .WithMany("News")
                        .HasForeignKey("NewsBoardId");

                    b.Navigation("newsboard");
                });

            modelBuilder.Entity("Lab4.Models.Subscription", b =>
                {
                    b.HasOne("Lab4.Models.Client", "Client")
                        .WithMany("Subscriptions")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Lab4.Models.NewsBoard", "NewsBoard")
                        .WithMany("Subscriptions")
                        .HasForeignKey("NewsBoardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("NewsBoard");
                });

            modelBuilder.Entity("Lab4.Models.Client", b =>
                {
                    b.Navigation("Subscriptions");
                });

            modelBuilder.Entity("Lab4.Models.NewsBoard", b =>
                {
                    b.Navigation("News");

                    b.Navigation("Subscriptions");
                });
#pragma warning restore 612, 618
        }
    }
}
