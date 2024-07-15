using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Pet_Shop2.Models;

public partial class PetShopContext : DbContext
{
    public PetShopContext()
    {
    }

    public PetShopContext(DbContextOptions<PetShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Banner> Banners { get; set; }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Page> Pages { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<ProductRate> ProductRates { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Shiper> Shipers { get; set; }

    public virtual DbSet<Slider> Sliders { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<Ward> Wards { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
        .Build();

        optionsBuilder.UseSqlServer(configuration.GetConnectionString("dbPet_Shop"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Account");

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Birthdate).HasColumnType("date");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.District).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.Salt).HasMaxLength(150);
            entity.Property(e => e.UserName).HasMaxLength(100);
            entity.Property(e => e.Ward).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Account_Role");
        });

        modelBuilder.Entity<Banner>(entity =>
        {
            entity.ToTable("Banner");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Image).HasMaxLength(250);
            entity.Property(e => e.PageId).HasColumnName("PageID");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Page).WithMany(p => p.Banners)
                .HasForeignKey(d => d.PageId)
                .HasConstraintName("FK_Banner_Page");
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.ToTable("Blog");

            entity.Property(e => e.Alias).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.MetaDesc).HasMaxLength(150);
            entity.Property(e => e.MetaKey).HasMaxLength(150);
            entity.Property(e => e.Scontents).HasMaxLength(150);
            entity.Property(e => e.Tags).HasMaxLength(150);
            entity.Property(e => e.Thumb).HasMaxLength(150);
            entity.Property(e => e.Title).HasMaxLength(150);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.Alias).HasMaxLength(150);
            entity.Property(e => e.Description).HasMaxLength(150);
            entity.Property(e => e.Image).HasMaxLength(150);
            entity.Property(e => e.MetaDesc).HasMaxLength(150);
            entity.Property(e => e.MetaKey).HasMaxLength(150);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Title).HasMaxLength(150);
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.ToTable("District");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.LocationId).HasColumnName("LocationID");
            entity.Property(e => e.Name).HasMaxLength(250);

            entity.HasOne(d => d.Location).WithMany(p => p.Districts)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("FK_District_Location");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("Location");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Type).HasMaxLength(30);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.Property(e => e.Note).HasMaxLength(150);
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.ShipDate).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.Orders)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Order_Account");

            entity.HasOne(d => d.TransctStatus).WithMany(p => p.Orders)
                .HasForeignKey(d => d.TransctStatusId)
                .HasConstraintName("FK_Order_Transaction");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.ToTable("OrderDetail");

            entity.Property(e => e.Total).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderDetail_Order");
        });

        modelBuilder.Entity<Page>(entity =>
        {
            entity.ToTable("Page");

            entity.Property(e => e.Alias).HasMaxLength(150);
            entity.Property(e => e.Contents).HasMaxLength(150);
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.MetaDesc).HasMaxLength(150);
            entity.Property(e => e.MetaKey).HasMaxLength(150);
            entity.Property(e => e.PageName).HasMaxLength(150);
            entity.Property(e => e.Thumb).HasMaxLength(150);
            entity.Property(e => e.Title).HasMaxLength(150);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.Alias).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(150);
            entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");
            entity.Property(e => e.Liked).HasDefaultValueSql("((0))");
            entity.Property(e => e.MetaDesc).HasMaxLength(150);
            entity.Property(e => e.MetaKey).HasMaxLength(150);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ProductName).HasMaxLength(150);
            entity.Property(e => e.ShortDesc).HasMaxLength(150);
            entity.Property(e => e.Thumb).HasMaxLength(150);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Product_Category");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.ToTable("Product_Image");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Idproduct).HasColumnName("IDproduct");

            entity.HasOne(d => d.IdproductNavigation).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.Idproduct)
                .HasConstraintName("FK_Product_Image_Product");
        });

        modelBuilder.Entity<ProductRate>(entity =>
        {
            entity.ToTable("Product_Rate");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Comment).HasMaxLength(250);
            entity.Property(e => e.CusName).HasMaxLength(20);
            entity.Property(e => e.IdCus).HasColumnName("idCus");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Description).HasMaxLength(150);
            entity.Property(e => e.RoleName).HasMaxLength(150);
        });

        modelBuilder.Entity<Shiper>(entity =>
        {
            entity.ToTable("Shiper");

            entity.Property(e => e.Company).HasMaxLength(150);
            entity.Property(e => e.Phone)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.ShipperName).HasMaxLength(150);
        });

        modelBuilder.Entity<Slider>(entity =>
        {
            entity.ToTable("Slider");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bottom).HasColumnName("bottom");
            entity.Property(e => e.Center).HasColumnName("center");
            entity.Property(e => e.Content)
                .HasMaxLength(250)
                .HasColumnName("content");
            entity.Property(e => e.Image)
                .HasMaxLength(250)
                .HasColumnName("image");
            entity.Property(e => e.Index).HasColumnName("index");
            entity.Property(e => e.Left).HasColumnName("left");
            entity.Property(e => e.Link)
                .HasMaxLength(250)
                .HasColumnName("link");
            entity.Property(e => e.PageId).HasColumnName("PageID");
            entity.Property(e => e.Right).HasColumnName("right");
            entity.Property(e => e.Top).HasColumnName("top");

            entity.HasOne(d => d.Page).WithMany(p => p.Sliders)
                .HasForeignKey(d => d.PageId)
                .HasConstraintName("FK_Slider_Page");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transaction");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(150);
        });

        modelBuilder.Entity<Ward>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Ward");

            entity.Property(e => e.DistrictId).HasColumnName("DistrictID");
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.WardId).HasColumnName("WardID");

            entity.HasOne(d => d.District).WithMany()
                .HasForeignKey(d => d.DistrictId)
                .HasConstraintName("FK_Ward_District");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
