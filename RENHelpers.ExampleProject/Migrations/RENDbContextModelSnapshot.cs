﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RENHelpers.ExampleProject.Database;

#nullable disable

namespace RENHelpers.ExampleProject.Migrations
{
    [DbContext(typeof(RENDbContext))]
    partial class RENDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RENHelpers.APITest.Data.POCO.Side", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Sides");
                });

            modelBuilder.Entity("RENHelpers.APITest.Data.POCO.Test", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("SideId")
                        .HasColumnType("int");

                    b.Property<string>("TestName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SideId")
                        .IsUnique();

                    b.ToTable("Tests");
                });

            modelBuilder.Entity("RENHelpers.APITest.Data.POCO.TestDescription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TestId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TestId");

                    b.ToTable("TestDescriptions");
                });

            modelBuilder.Entity("RENHelpers.APITest.Data.POCO.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SideId")
                        .HasColumnType("int");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SideId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("RENHelpers.APITest.Data.POCO.Test", b =>
                {
                    b.HasOne("RENHelpers.APITest.Data.POCO.Side", "Side")
                        .WithOne("Test")
                        .HasForeignKey("RENHelpers.APITest.Data.POCO.Test", "SideId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Side");
                });

            modelBuilder.Entity("RENHelpers.APITest.Data.POCO.TestDescription", b =>
                {
                    b.HasOne("RENHelpers.APITest.Data.POCO.Test", "Test")
                        .WithMany("TestDescriptions")
                        .HasForeignKey("TestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Test");
                });

            modelBuilder.Entity("RENHelpers.APITest.Data.POCO.User", b =>
                {
                    b.HasOne("RENHelpers.APITest.Data.POCO.Side", "Side")
                        .WithMany("Users")
                        .HasForeignKey("SideId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Side");
                });

            modelBuilder.Entity("RENHelpers.APITest.Data.POCO.Side", b =>
                {
                    b.Navigation("Test")
                        .IsRequired();

                    b.Navigation("Users");
                });

            modelBuilder.Entity("RENHelpers.APITest.Data.POCO.Test", b =>
                {
                    b.Navigation("TestDescriptions");
                });
#pragma warning restore 612, 618
        }
    }
}
