using System;
using System.Collections.Generic;
using Authentication_and_Authorization_Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Minimart_Api.Models
{
    public partial class MinimartDBContext : DbContext
    {
        public MinimartDBContext()
        {
        }

        public MinimartDBContext(DbContextOptions<MinimartDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CartItem> CartItems { get; set; } = null!;
        public virtual DbSet<TCart> TCarts { get; set; } = null!;
        public virtual DbSet<TCategory> TCategories { get; set; } = null!;
        public virtual DbSet<TDashboarditem> TDashboarditems { get; set; } = null!;
        public virtual DbSet<TFeature> TFeatures { get; set; } = null!;
        public virtual DbSet<THelpsetting> THelpsettings { get; set; } = null!;
        public virtual DbSet<TImage> TImages { get; set; } = null!;
        public virtual DbSet<TOrder> TOrders { get; set; } = null!;
        public virtual DbSet<TOrderItem> TOrderItems { get; set; } = null!;
        public virtual DbSet<TProduct> TProducts { get; set; } = null!;
        public virtual DbSet<TProduct1> TProducts1 { get; set; } = null!;
        public virtual DbSet<TRefreshToken> TRefreshTokens { get; set; } = null!;
        public virtual DbSet<TReview> TReviews { get; set; } = null!;
        public virtual DbSet<TSubcategoryid> TSubcategoryids { get; set; } = null!;
        public virtual DbSet<TUser> TUsers { get; set; } = null!;

        public virtual DbSet<Status> Statuses { get; set; } = null!;

        public virtual DbSet<UserRegStatus> UsrRegStatuses { get; set; } = null!;

        public virtual DbSet<UserInfo> Users { get; set; } = null!;
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-K2BT32U;Database = MinimartDB;User Id=Caleb; Password=Caleb@2543");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Status>().HasNoKey();

            modelBuilder.Entity<UserRegStatus>().HasNoKey();

            modelBuilder.Entity<UserInfo>()
                  .HasOne(u => u.Status)
                  .WithMany(r => r.Users)
                  .HasForeignKey(u => u.StatusId);


            ;
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.Property(e => e.CartItemId).HasColumnName("cartItemID");

                entity.Property(e => e.CartId).HasColumnName("cartID");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(50)
                    .HasColumnName("productID");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

                entity.HasOne(d => d.Cart)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.CartId)
                    .HasConstraintName("FK__CartItems__cartI__2A164134");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__CartItems__prod");
            });

            modelBuilder.Entity<TCart>(entity =>
            {
                entity.HasKey(e => e.CartId)
                    .HasName("PK__t_Carts__415B03D8762A75C5");

                entity.ToTable("t_Carts");

                entity.Property(e => e.CartId).HasColumnName("cartID");

                entity.Property(e => e.CartName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("cartName");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("userID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TCarts)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__t_Carts__userID__2739D489");
            });

            modelBuilder.Entity<TCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("PK__t_Catego__23CAF1F8AFEA0E02");

                entity.ToTable("t_Categories");

                entity.Property(e => e.CategoryId).HasColumnName("categoryID");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("categoryName");

                entity.Property(e => e.Description)
                    .HasColumnType("text")
                    .HasColumnName("description");
            });

            modelBuilder.Entity<TDashboarditem>(entity =>
            {
                entity.HasKey(e => e.DashBoardId)
                    .HasName("PK_t_dashboarditems_DashBoardID");

                entity.ToTable("t_dashboarditems");

                entity.Property(e => e.DashBoardId).HasColumnName("DashBoardID");

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<TFeature>(entity =>
            {
                entity.HasKey(e => e.ProductId)
                    .HasName("PK_t_features_productID");

                entity.ToTable("t_features");

                entity.Property(e => e.ProductId).HasColumnName("productID");

                entity.Property(e => e.AdaptiveSync).HasMaxLength(100);

                entity.Property(e => e.Brand).HasMaxLength(100);

                entity.Property(e => e.Color)
                    .HasMaxLength(100)
                    .HasColumnName("color");

                entity.Property(e => e.ConnectiveTechnology)
                    .HasMaxLength(100)
                    .HasColumnName("connectiveTechnology");

                entity.Property(e => e.DisplayResolution).HasMaxLength(100);

                entity.Property(e => e.DisplayType).HasMaxLength(100);

                entity.Property(e => e.ImageBrightness).HasMaxLength(100);

                entity.Property(e => e.ItemWeight).HasMaxLength(100);

                entity.Property(e => e.MainCategory).HasMaxLength(100);

                entity.Property(e => e.MountingType).HasMaxLength(100);

                entity.Property(e => e.ProductName)
                    .HasMaxLength(100)
                    .HasColumnName("productName");

                entity.Property(e => e.Resolution).HasMaxLength(100);

                entity.Property(e => e.ResponseTime).HasMaxLength(100);

                entity.Property(e => e.ScreenSize).HasMaxLength(100);

                entity.Property(e => e.ScreenSurface).HasMaxLength(100);

                entity.Property(e => e.SpecialFeatures).HasMaxLength(100);

                entity.Property(e => e.WarrantyType).HasMaxLength(100);
            });

            modelBuilder.Entity<THelpsetting>(entity =>
            {
                entity.HasKey(e => e.Rowid)
                    .HasName("PK_t_helpsettings_ROWID");

                entity.ToTable("t_helpsettings");

                entity.Property(e => e.Rowid).HasColumnName("ROWID");

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<TImage>(entity =>
            {
                entity.HasKey(e => e.ImageId)
                    .HasName("PK_t_image_ImageID");

                entity.ToTable("t_image");

                entity.Property(e => e.ImageId).HasColumnName("ImageID");

                entity.Property(e => e.ImageType).HasMaxLength(150);
            });

            modelBuilder.Entity<TOrder>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("PK__t_Orders__0809337DBD766844");

                entity.ToTable("t_Orders");

                entity.Property(e => e.OrderId).HasColumnName("orderID");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime")
                    .HasColumnName("orderDate");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("totalAmount");

                entity.Property(e => e.UserId).HasColumnName("userID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TOrders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__t_Orders__userID__1BC821DD");
            });

            modelBuilder.Entity<TOrderItem>(entity =>
            {
                entity.HasKey(e => e.OrderItemId)
                    .HasName("PK__t_OrderI__3724BD72056978F8");

                entity.ToTable("t_OrderItems");

                entity.Property(e => e.OrderItemId).HasColumnName("orderItemID");

                entity.Property(e => e.OrderId).HasColumnName("orderID");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(50)
                    .HasColumnName("productID");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.TOrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__t_OrderIt__order__1EA48E88");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.TOrderItems)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__t_OrderItems__prod");
            });

            modelBuilder.Entity<TProduct>(entity =>
            {
                entity.HasKey(e => e.ProductId)
                    .HasName("PK_t_product_ProductID");

                entity.ToTable("t_product");

                entity.HasIndex(e => e.ProductName, "idxProduct");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.Box).HasMaxLength(500);

                entity.Property(e => e.Category).HasMaxLength(50);

                entity.Property(e => e.ImageType).HasMaxLength(150);

                entity.Property(e => e.ImageUrl).HasMaxLength(500);

                entity.Property(e => e.KeyFeatures).HasMaxLength(500);

                entity.Property(e => e.Price).HasColumnType("decimal(10, 0)");

                entity.Property(e => e.ProductName).HasMaxLength(150);

                entity.Property(e => e.ProductType)
                    .HasMaxLength(10)
                    .HasDefaultValueSql("(N'P')");

                entity.Property(e => e.Specification).HasMaxLength(500);

                entity.Property(e => e.SubCategoryId)
                    .HasMaxLength(100)
                    .HasColumnName("SubCategoryID");
            });

            modelBuilder.Entity<TProduct1>(entity =>
            {
                entity.HasKey(e => e.ProductId)
                    .HasName("PK__t_Produc__B40CC6ED6CED5B85");

                entity.ToTable("t_Products");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(50)
                    .HasColumnName("ProductID");

                entity.Property(e => e.Box).HasMaxLength(500);

                entity.Property(e => e.Category).HasMaxLength(50);

                entity.Property(e => e.CategoryId).HasColumnName("categoryID");

                entity.Property(e => e.Description)
                    .HasColumnType("text")
                    .HasColumnName("description");

                entity.Property(e => e.ImageType).HasMaxLength(150);

                entity.Property(e => e.ImageUrl).HasMaxLength(500);

                entity.Property(e => e.KeyFeatures).HasMaxLength(500);

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 0)")
                    .HasColumnName("price");

                entity.Property(e => e.ProductName)
                    .IsUnicode(false)
                    .HasColumnName("productName");

                entity.Property(e => e.ProductType).HasMaxLength(10);

                entity.Property(e => e.RowId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("RowID");

                entity.Property(e => e.Specification).HasMaxLength(500);

                entity.Property(e => e.StockQuantity).HasColumnName("stockQuantity");

                entity.Property(e => e.SubCategoryId)
                    .HasMaxLength(100)
                    .HasColumnName("SubCategoryID");

                entity.HasOne(d => d.CategoryNavigation)
                    .WithMany(p => p.TProduct1s)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__t_Product__categ__18EBB532");
            });

            modelBuilder.Entity<TRefreshToken>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("t_RefreshToken");

                entity.Property(e => e.ExpiryDate).HasColumnType("datetime");

                entity.Property(e => e.RefreshToken)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("UserID");
            });

            modelBuilder.Entity<TReview>(entity =>
            {
                entity.HasKey(e => e.ReviewId)
                    .HasName("PK__t_Review__2ECD6E2451F94E4F");

                entity.ToTable("t_Reviews");

                entity.Property(e => e.ReviewId).HasColumnName("reviewID");

                entity.Property(e => e.Comment)
                    .HasColumnType("text")
                    .HasColumnName("comment");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(50)
                    .HasColumnName("productID");

                entity.Property(e => e.Rating).HasColumnName("rating");

                entity.Property(e => e.ReviewDate)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .HasColumnName("review_date");

                entity.Property(e => e.UserId).HasColumnName("userID");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.TReviews)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__t_Reviews__prod");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TReviews)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__t_Reviews__userI__245D67DE");
            });

            modelBuilder.Entity<TSubcategoryid>(entity =>
            {
                entity.HasKey(e => e.SubCategoryId)
                    .HasName("PK_t_subcategoryid_CategoryID");

                entity.ToTable("t_subcategoryid");

                entity.Property(e => e.SubCategoryId).HasColumnName("SubCategoryID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CategoryName).HasMaxLength(100);

                entity.Property(e => e.ProductName).HasMaxLength(100);

                entity.Property(e => e.ProductType)
                    .HasMaxLength(10)
                    .HasDefaultValueSql("(N'C')");

                entity.Property(e => e.SubCategory).HasMaxLength(100);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.TSubcategoryids)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_t_subcategoryid_CategoryID");
            });

            modelBuilder.Entity<TUser>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__t_Users__1788CCAC83A2EDF9");

                entity.ToTable("t_Users");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.IsAdmin).HasColumnName("isAdmin");

                entity.Property(e => e.IsLoggedIn).HasColumnName("isLoggedIn");

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PasswordChangesOn).HasColumnType("datetime");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("RoleID");

                entity.Property(e => e.UserName)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
