using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Repository.Models
{
    public partial class PokemonDbContext : DbContext
    {
        public PokemonDbContext()
        {
        }

        public PokemonDbContext(DbContextOptions<PokemonDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CartItem> CartItems { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<ChatRequest> ChatRequests { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(local);Database=PokemonStoreDB;Uid=sa;Pwd=12345;TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.ItemId)
                    .HasName("PK__CartItem__727E838BF693B9B4");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerId_CartItems");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductId_CartItems");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<ChatRequest>(entity =>
            {
                entity.HasKey(e => e.MessageId)
                    .HasName("PK__ChatRequ__C87C0C9C2634263E");

                entity.ToTable("ChatRequest");

                entity.Property(e => e.SendTime).HasColumnType("datetime");

                entity.Property(e => e.Type).HasMaxLength(256);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.DoB).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.HashedPassword).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Phone).HasMaxLength(255);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.ExpiredDate).HasColumnType("datetime");

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.TotalPrice).HasColumnType("decimal(13, 2)");

                entity.Property(e => e.TransactionCode).HasMaxLength(256);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerId_Orders");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.PricePerUnit).HasColumnType("decimal(13, 2)");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderId_OrderDetails");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductId_OrderDetails");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.PayDate).HasColumnType("datetime");

                entity.Property(e => e.PaymentAmount).HasColumnType("decimal(13, 2)");

                entity.Property(e => e.PaymentMethod).HasMaxLength(100);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderId_Payments");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Price).HasColumnType("decimal(13, 2)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CategoryId");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.ImageId)
                    .HasName("PK__ProductI__7516F70CCBEAE9E3");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductId_ProductImages");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
