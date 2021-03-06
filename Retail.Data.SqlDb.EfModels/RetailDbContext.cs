﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Retail.Data.SqlDb.EfModels.Models;
using Microsoft.EntityFrameworkCore;

namespace Retail.Data.SqlDb.EfModels
{
    public partial class RetailDbContext : DbContext
    {
        public RetailDbContext()
        {
        }

        public RetailDbContext(DbContextOptions<RetailDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderLineItem> OrderLineItems { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Store> Stores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.MembershipNumber)
                    .HasName("UQ_Customers_MembershipNumber")
                    .IsUnique();

                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.MembershipNumber).HasDefaultValueSql("(newid())");

                entity.Property(e => e.PhoneNumber).IsUnicode(false);

                entity.Property(e => e.PostalCode).IsUnicode(false);
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasKey(e => new { e.StoreId, e.ProductId });

                entity.HasIndex(e => e.ProductId);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.CustomerPhoneNumber).IsUnicode(false);

                entity.Property(e => e.ShippingPostalCode).IsUnicode(false);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Orders_Customer");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orders_Store");
            });

            modelBuilder.Entity<OrderLineItem>(entity =>
            {
                entity.Property(e => e.LineItemId).ValueGeneratedNever();

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderLineItems)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderLineItems_Order");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderLineItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderLineItems_Product");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.PhoneNumber).IsUnicode(false);

                entity.Property(e => e.PostalCode).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}