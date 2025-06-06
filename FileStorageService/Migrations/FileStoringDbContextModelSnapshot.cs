﻿// <auto-generated />
using System;
using FileStorageService.Infrastructure.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FileStorageService.Migrations
{
    [DbContext(typeof(FileStoringDbContext))]
    partial class FileStoringDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FileStorageService.Domain.Entities.FileHolder", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("FileDirectory")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("fileDirectory");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("fileName");

                    b.Property<int>("Hash")
                        .HasColumnType("integer")
                        .HasColumnName("hash");

                    b.HasKey("Id");

                    b.ToTable("FileHolders", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
