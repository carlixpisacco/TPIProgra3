﻿// <auto-generated />
using System;
using ClothingStore.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ClothingStore.Infrastructure.Migrations
{
    [DbContext(typeof(ClothingStoreDbContext))]
    [Migration("20240722090031_ModificacionesEnProduct")]
    partial class ModificacionesEnProduct
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("ClothingStore.Domain.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ClientId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<float>("Price")
                        .HasColumnType("REAL");

                    b.Property<int>("SellerId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("isActive")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("isAvailable")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("SellerId");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("ClothingStore.Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasDiscriminator<string>("Role").HasValue("User");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("ClothingStore.Domain.Entities.Client", b =>
                {
                    b.HasBaseType("ClothingStore.Domain.Entities.User");

                    b.HasDiscriminator().HasValue("client");
                });

            modelBuilder.Entity("ClothingStore.Domain.Entities.Seller", b =>
                {
                    b.HasBaseType("ClothingStore.Domain.Entities.User");

                    b.HasDiscriminator().HasValue("seller");
                });

            modelBuilder.Entity("ClothingStore.Domain.Entities.Product", b =>
                {
                    b.HasOne("ClothingStore.Domain.Entities.Client", "Client")
                        .WithMany("PurchasedProducts")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("ClothingStore.Domain.Entities.Seller", "Seller")
                        .WithMany("PublishedProducts")
                        .HasForeignKey("SellerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("Seller");
                });

            modelBuilder.Entity("ClothingStore.Domain.Entities.Client", b =>
                {
                    b.Navigation("PurchasedProducts");
                });

            modelBuilder.Entity("ClothingStore.Domain.Entities.Seller", b =>
                {
                    b.Navigation("PublishedProducts");
                });
#pragma warning restore 612, 618
        }
    }
}
