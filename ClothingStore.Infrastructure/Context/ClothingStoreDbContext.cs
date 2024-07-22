using ClothingStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingStore.Infrastructure.Context
{
    public class ClothingStoreDbContext : DbContext
    {
        public ClothingStoreDbContext(DbContextOptions<ClothingStoreDbContext> options) : base(options) { }

        public DbSet<User> Users{ get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Product> Products { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder) //esto permite utilizar la herencia y poder trabajar en el repositorio con "User" (clase abstracta que no puede instanciarse directamente)
            //es decir poder instanciar "client" o "seller" segun el discriminador "role" de la tabla. 
        {
            modelBuilder.Entity<User>()
            .HasDiscriminator<string>("Role")
            .HasValue<Seller>("seller")
            .HasValue<Client>("client");

            // Configuración de la relación entre Producto y Vendedor
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Seller)
                .WithMany(v => v.PublishedProducts)
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            //configuración de la relación entre Producto y Cliente
            modelBuilder.Entity<Product>()
           .HasOne(p => p.Client)
           .WithMany(c => c.PurchasedProducts)
           .HasForeignKey(p => p.ClientId)
           .OnDelete(DeleteBehavior.Restrict);
        }

    }
}