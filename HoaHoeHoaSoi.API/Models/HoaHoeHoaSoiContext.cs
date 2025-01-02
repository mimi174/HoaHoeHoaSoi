using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HoaHoeHoaSoi.API.Models;

public partial class HoaHoeHoaSoiContext : DbContext
{
    public HoaHoeHoaSoiContext()
    {
    }

    public HoaHoeHoaSoiContext(DbContextOptions<HoaHoeHoaSoiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<OrderLine> OrderLines { get; set; }

    public virtual DbSet<Ordered> Ordereds { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductWishlist> ProductWishlists { get; set; }

    public virtual DbSet<UserInfo> UserInfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(local);Initial Catalog=HoaHoeHoaSoi;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.ToTable("Admin");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer");

            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.ToTable("Feedback");

            entity.Property(e => e.Content).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<OrderLine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order_Li__3214EC07DD91D014");

            entity.ToTable("Order_Line");

            entity.Property(e => e.OrderedId).HasColumnName("Ordered_Id");
            entity.Property(e => e.ProductsId).HasColumnName("Products_Id");

            entity.HasOne(d => d.Ordered).WithMany(p => p.OrderLines)
                .HasForeignKey(d => d.OrderedId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Line_Ordered");

            entity.HasOne(d => d.Products).WithMany(p => p.OrderLines)
                .HasForeignKey(d => d.ProductsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Line_Products");
        });

        modelBuilder.Entity<Ordered>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ordered__3214EC07A617FD9C");

            entity.ToTable("Ordered");

            entity.Property(e => e.CustomerId).HasColumnName("Customer_Id");
            entity.Property(e => e.PaymentMethod).HasDefaultValue(1);
            entity.Property(e => e.PaymentOrderId)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ReceiverAddress).HasMaxLength(200);
            entity.Property(e => e.ReceiverName).HasMaxLength(200);
            entity.Property(e => e.ReceiverPhone)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ResultCode)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Customer).WithMany(p => p.Ordereds)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Ordered_Customer");

            entity.HasOne(d => d.User).WithMany(p => p.Ordereds)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Ordered__UserId__49C3F6B7");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC0798DEAFE8");

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ProductWishlist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductW__3214EC0778AC9EBE");

            entity.ToTable("ProductWishlist");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductWishlists)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductWi__Produ__5CD6CB2B");

            entity.HasOne(d => d.User).WithMany(p => p.ProductWishlists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductWi__UserI__5DCAEF64");
        });

        modelBuilder.Entity<UserInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserInfo__3214EC073944D147");

            entity.ToTable("UserInfo");

            entity.HasIndex(e => e.Username, "UQ__UserInfo__536C85E4CF044EE2").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.Mail).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
