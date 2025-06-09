using Microsoft.EntityFrameworkCore;
using Minimart_Api.Models;

namespace Minimart_Api.Data
{
    public class MinimartDBContext : DbContext
    {
        public MinimartDBContext(DbContextOptions<MinimartDBContext> options) : base(options) { }

        public virtual DbSet<Addresses> Addresses { get; set; }
        public virtual DbSet<Cart> Cart { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<SavedItems> SavedItems { get; set; }
        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<Counties> Counties { get; set; }
        public virtual DbSet<DeliveryStations> DeliveryStations { get; set; }
        public virtual DbSet<Features> Features { get; set; }
        public virtual DbSet<Modules> Modules { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<OrderProducts> OrderProducts { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<OrderStatus> OrderStatuses { get; set; }
        public virtual DbSet<OrderTracking>  OrderTrackings { get; set; }
        public virtual DbSet<PaymentDetails> PaymentDetails { get; set; }
        public virtual DbSet<PaymentMethods> PaymentMethods { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<Reviews> Reviews { get; set; }
        public virtual DbSet<RolePermissions> RolePermissions { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<SubModuleCategories> SubModuleCategories { get; set; }
        public virtual DbSet<SubModules> SubModules { get; set; }
        public virtual DbSet<SystemMerchants> SystemMerchants { get; set; }
        public virtual DbSet<Towns> Towns { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure indexes
            modelBuilder.Entity<Orders>().HasIndex(o => o.UserID);
            modelBuilder.Entity<OrderProducts>().HasIndex(op => op.ProductID);
            modelBuilder.Entity<Products>().HasIndex(p => p.CategoryId);

            // Addresses configuration
            modelBuilder.Entity<Addresses>(entity =>
            {
                entity.ToTable("Addresses");
                entity.Property(e => e.Name).HasColumnType("varchar(100)");
                entity.Property(e => e.Phonenumber).HasColumnType("varchar(15)");
                entity.Property(e => e.PostalAddress).HasColumnType("varchar(200)");
                entity.Property(e => e.County).HasColumnType("varchar(50)");
                entity.Property(e => e.Town).HasColumnType("varchar(50)");
                entity.Property(e => e.PostalCode).HasColumnType("varchar(10)");
                entity.Property(e => e.ExtraInformation).HasColumnType("varchar(500)");
                entity.Property(e => e.CreatedOn).HasColumnType("timestamp");
                entity.Property(e => e.LastUpdatedOn).HasColumnType("timestamp");
            });

            // Products configuration
            modelBuilder.Entity<Products>(entity =>
            {
                entity.HasOne(e => e.Categories)
                    .WithMany(c => c.Products)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Categories)
                    .WithMany(c => c.Products)
                    .HasForeignKey(e => e.SubCategoryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Categories)
                    .WithMany(c => c.Products)
                    .HasForeignKey(e => e.SubSubCategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // OrderItems configuration
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItems");
                entity.Property(e => e.Price).HasColumnType("numeric(18,2)");
            });

            // Reviews configuration
            modelBuilder.Entity<Reviews>(entity =>
            {
                entity.ToTable("Reviews");
                entity.Property(e => e.ReviewDate).HasColumnType("timestamp");
            });

            // Categories configuration
            modelBuilder.Entity<Categories>(entity =>
            {
                entity.ToTable("Categories");
                entity.Property(e => e.CreatedOn).HasColumnType("timestamp");
                entity.Property(e => e.UpdatedOn).HasColumnType("timestamp");
            });

            // Features configuration
            modelBuilder.Entity<Features>(entity =>
            {
                entity.ToTable("Features");

                // Relationship with Category
                entity.HasOne(f => f.Category)
                    .WithMany(c => c.CategoryFeatures)
                    .HasForeignKey(f => f.CategoryID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relationship with SubCategory
                entity.HasOne(f => f.SubCategory)
                    .WithMany(c => c.SubCategoryFeatures)
                    .HasForeignKey(f => f.SubCategoryID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relationship with SubSubCategory (if needed)
                entity.HasOne(f => f.SubSubCategory)
                    .WithMany(c => c.SubSubCategoryFeatures)
                    .HasForeignKey(f => f.SubSubCategoryID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Authorization configuration
            modelBuilder.Entity<SubModuleCategories>().HasKey(sc => sc.SubModuleID);
            modelBuilder.Entity<Modules>().HasKey(m => m.ModuleID);
            modelBuilder.Entity<SubModules>().HasKey(sm => sm.SubModuleID);
            modelBuilder.Entity<Roles>().HasKey(r => r.RoleID);
            modelBuilder.Entity<RolePermissions>().HasKey(rp => rp.RolePermissionID);

            // Users configuration
            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("timestamp");
                //entity.Property(e => e.).HasColumnType("timestamp");
            });

            // Orders configuration
            modelBuilder.Entity<Orders>(entity =>
            {
                entity.ToTable("Orders");
                entity.Property(e => e.OrderDate).HasColumnType("timestamp with time zone");
                entity.Property(e => e.DeliveryScheduleDate).HasColumnType("timestamp with time zone");
                entity.Property(e => e.TotalOrderAmount).HasColumnType("money");
                entity.Property(e => e.TotalPaymentAmount).HasColumnType("money");
                entity.Property(e => e.TotalDeliveryFees).HasColumnType("money");
                entity.Property(e => e.TotalTax).HasColumnType("money");
            });

            // OrderTracking configuration
            modelBuilder.Entity<OrderTracking>(entity =>
            {
                entity.ToTable("OrderTracking");
                entity.Property(e => e.TrackingDate).HasColumnType("timestamp");
            });

            // PaymentDetails configuration
            modelBuilder.Entity<PaymentDetails>(entity =>
            {
                entity.Property(e => e.PaymentDate).HasColumnType("timestamp with time zone");
                entity.Property(e => e.Amount).HasColumnType("money");
            });

            // Cart configuration
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("Cart");
                entity.Property(e => e.CreatedAt).HasColumnType("timestamp");
            });

            // CartItem configuration
            modelBuilder.Entity<CartItem>()
               .HasOne(ci => ci.Products)
               .WithMany()
               .HasForeignKey(ci => ci.ProductId);
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.ToTable("CartItem");
                entity.Property(e => e.ProductId).HasColumnType("varchar(50)");
                entity.Property(e => e.CreatedOn).HasColumnType("timestamp");
                entity.Property(e => e.UpdatedOn).HasColumnType("timestamp");
            });

            // BuyAgain configuration
            modelBuilder.Entity<BuyAgain>(entity =>
            {
                entity.ToTable("BuyAgain");
                entity.Property(e => e.ProductId).HasColumnType("varchar(50)");
                entity.Property(e => e.PurchasedOn).HasColumnType("timestamp");
            });

            // SavedItems configuration
            modelBuilder.Entity<SavedItems>(entity =>
            {
                entity.ToTable("SavedItems");
                entity.Property(e => e.ProductId).HasColumnType("varchar(50)");
                entity.Property(e => e.SavedOn).HasColumnType("timestamp");
            });

            // SystemMerchants configuration
            modelBuilder.Entity<SystemMerchants>(entity =>
            {
                entity.Property(e => e.BusinessType).HasColumnType("varchar(50)");
                entity.Property(e => e.BusinessRegistrationNo).HasColumnType("varchar(50)");
                entity.Property(e => e.KRAPIN).HasColumnType("varchar(20)");
                entity.Property(e => e.BusinessNature).HasColumnType("varchar(50)");
                entity.Property(e => e.BusinessCategory).HasColumnType("varchar(50)");
                entity.Property(e => e.Email).HasColumnType("varchar(255)");
                entity.Property(e => e.Phone).HasColumnType("varchar(20)");
                entity.Property(e => e.Address).HasColumnType("varchar(500)");
                entity.Property(e => e.BankAccountNo).HasColumnType("varchar(20)");
                entity.Property(e => e.MpesaPaybill).HasColumnType("varchar(20)");
                entity.Property(e => e.MpesaTillNumber).HasColumnType("varchar(20)");
                entity.Property(e => e.Status).HasColumnType("varchar(20)");
            });
        }
    }
}