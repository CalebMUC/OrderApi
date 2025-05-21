 using Microsoft.EntityFrameworkCore;
using Minimart_Api.Models;
using StackExchange.Redis;

namespace Minimart_Api.Data
{
    public class MinimartDBContext : DbContext
    {
        public MinimartDBContext(DbContextOptions<MinimartDBContext> options) : base(options)
        {
            
        }

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
        public virtual DbSet<OrderTracking> OrderTrackings { get; set; }
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


        //public override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        //    if (!optionsBuilder.IsConfigured) { 

        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            // In your DbContext's OnModelCreating
            modelBuilder.Entity<Orders>()
                .HasIndex(o => o.UserID);

            modelBuilder.Entity<OrderProducts>()
                .HasIndex(op => op.ProductID);

            modelBuilder.Entity<Products>()
                .HasIndex(p => p.CategoryId);

            // ─── Addresses ───────────────────────────────────────────────────


            modelBuilder.Entity<Addresses>(entity =>
            {
                entity.ToTable("Addresses");

                entity.HasKey(e => e.AddressID);

                entity.Property(e => e.AddressID)
                    .IsRequired();

                entity.Property(e => e.UserID)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("nvarchar(100)");

                entity.Property(e => e.Phonenumber)
                    .IsRequired()
                    .HasColumnType("varchar(15)");

                entity.Property(e => e.PostalAddress)
                    .HasColumnType("nvarchar(200)");

                entity.Property(e => e.County)
                    .HasColumnType("nvarchar(50)");

                entity.Property(e => e.Town)
                    .HasColumnType("nvarchar(50)");

                entity.Property(e => e.PostalCode)
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.ExtraInformation)
                    .HasColumnType("nvarchar(500)");

                entity.Property(e => e.isDefault)
                    .HasColumnType("bit");


                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime");


                entity.Property(e => e.LastUpdatedOn)
                    .HasColumnType("datetime");

                // ─── Relationships ───────────────────────────────────────────────────


                // Foreign key relationship to Users
                entity.HasOne(e => e.users)
                    .WithMany() // or .WithMany(u => u.Addresses) if you have a collection in Users
                    .HasForeignKey(e => e.UserID)
                    .OnDelete(DeleteBehavior.Cascade); // Adjust based on your business rule
            });

            // ─── Products ───────────────────────────────────────────────────

            modelBuilder.Entity<Products>(entity => {

                entity.HasIndex(e => new { e.ProductId });


                entity.ToTable("Products");

                entity.HasOne(e=> e.Categories)
                .WithMany(c=> c.Products)
                .HasForeignKey(e=>e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Categories)
              .WithMany(c => c.Products)
              .HasForeignKey(e => e.SubCategoryId)
              .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Categories)
             .WithMany(c => c.Products)
             .HasForeignKey(e => e.SubSubCategoryId)
             .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e=> e.Reviews)
                .WithOne(r=> r.Product)
                .HasForeignKey(r=>r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);


            });

            //_________________________________OrderItems_______________________________________

            modelBuilder.Entity<OrderItem>(entity => {

                entity.ToTable("OrderItems");

                entity.HasOne(e=>e.Product)
                .WithMany()
                .HasForeignKey(e=>e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Order)
                .WithMany()
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            });

            //---------------------------------------Reviews-----------------------------

            modelBuilder.Entity<Reviews>(entity => {
                entity.ToTable("Reviews");

                entity.HasOne(e=>e.Product)
                .WithMany()//the product entity can be refrenced by many reviewed records
                .HasForeignKey(e=>e.ProductId) 
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            // ------------------------------------------------------------------------------------------------------------------------------------
            //-----------------------------Categories---------------------------------------------------
            //-------------------------------------------------------------------------------------------------------------------------------------

            modelBuilder.Entity<Categories>(entity =>
            {
                entity.ToTable("Categories");

                entity.HasMany(e=> e.SubCategories)
                .WithOne(e=> e.ParentCategory)//one category can have many subcategories
                .HasForeignKey(e=> e.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);


                entity.HasMany(e => e.Products)
                .WithOne(p => p.Categories)//one category can have many products
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
            });


            // ------------------------------------------------------------------------------------------------------------------------------------
            //-----------------------------Features---------------------------------------------------
            //-------------------------------------------------------------------------------------------------------------------------------------

            modelBuilder.Entity<Features>(entity =>
            {
                entity.ToTable("Features");

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.CategoryFeatures)
                    .HasForeignKey(e => e.CategoryID)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

                entity.HasOne(e => e.SubCategory)
                    .WithMany(c => c.SubCategoryFeatures)
                    .HasForeignKey(e => e.SubCategoryID)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
            });



            // ------------------------------------------------------------------------------------------------------------------------------------
            //-----------------------------Authorization---------------------------------------------------
            //-------------------------------------------------------------------------------------------------------------------------------------

            modelBuilder.Entity<SubModuleCategories>()
         .HasKey(sc => sc.SubModuleID);

            modelBuilder.Entity<SubModuleCategories>()
                .HasOne(sc => sc.Submodule)
                .WithMany(sm => sm.SubModuleCategories)
                .HasForeignKey(sc => sc.SubModuleID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Modules>()
                .HasKey(m => m.ModuleID); // Primary Key

            // Configure Submodule Entity
            modelBuilder.Entity<SubModules>()
                .HasKey(sm => sm.SubModuleID); // Primary Key

            modelBuilder.Entity<SubModules>()
                .HasOne(sm => sm.Module)
                .WithMany(m => m.Submodules)
                .HasForeignKey(sm => sm.ModuleID)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete if Module is deleted

            // Configure Role Entity
            modelBuilder.Entity<Roles>()
                .HasKey(r => r.RoleID); // Primary Key

            modelBuilder.Entity<Roles>()
               .HasKey(r => r.RoleID); // Primary Key


            // Configure RolePermission Entity
            modelBuilder.Entity<RolePermissions>()
                .HasKey(rp => rp.RolePermissionID);

            // Cascade on delete from Role
            modelBuilder.Entity<RolePermissions>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleID)
                .OnDelete(DeleteBehavior.Cascade); // Keep cascade here

            // Restrict on delete from Module
            modelBuilder.Entity<RolePermissions>()
                .HasOne(rp => rp.Module)
                .WithMany(m => m.RolePermissions)
                .HasForeignKey(rp => rp.ModuleID)
                .OnDelete(DeleteBehavior.Restrict); // Change from Cascade to Restrict

            // Restrict on delete from Submodule (already okay)
            modelBuilder.Entity<RolePermissions>()
                .HasOne(rp => rp.Submodule)
                .WithMany(sm => sm.RolePermissions)
                .HasForeignKey(rp => rp.SubModuleID)
                .OnDelete(DeleteBehavior.Restrict);


            //------------------------------------------------------------------------------------------------------------------------------------
            //----------------------------Users-------------------------------------
            //------------------------------------------------------------------------------------------------------------------------------------

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasIndex(e => new { e.UserId });


                entity.HasOne(u => u.Role)
                        .WithMany(r => r.Users)
                        .HasForeignKey(u => u.RoleId)
                        .HasPrincipalKey(r => r.RoleID) // Because RoleID is a string and the PK in Roles
                        .OnDelete(DeleteBehavior.Restrict); // Optional: prevents deleting a Role if users exist

            });
           



            //---------------------------------------------------------------
            //-------------Orders--------------------------------------
            //----------------------------------------------------------------

            //------------------------------Orders--------------------------------------

            modelBuilder.Entity<Orders>(entity => {

                entity.ToTable("Orders");
                //one User can have many orders in the same Table
                entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserID);

                entity.HasOne(e => e.PaymentDetails)
                .WithMany(p => p.Orders)
                .HasForeignKey(e => e.PaymentID);

                entity.HasMany(e => e.OrderProducts)
                .WithOne(op => op.order)
                .HasForeignKey(op => op.OrderID);

             


            });


   


            modelBuilder.Entity<OrderTracking>(entity => {
                entity.ToTable("OrderTracking");
                //many OrderTracking per Order 
                entity.HasOne(e=> e.Order)
                .WithMany(o=> o.OrderTrackings)
                .HasForeignKey(e=> e.OrderID)
                .OnDelete(DeleteBehavior.Restrict);

                //many OrderTracking per Product 
                entity.HasOne(e => e.product)
                .WithMany(o => o.OrderTrackings)
                .HasForeignKey(e => e.ProductID)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e=>e.PreviousStatusNavigation)
                .WithMany() //the OrderStaus Entity can be refrenced by many Order Tracking Records
                .HasForeignKey(e=> e.PreviousStatus)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CurrentStatusNavigation)
                .WithMany() //the OrderStaus Entity can be refrenced by many Order Tracking Records
                .HasForeignKey(e => e.CurrentStatus)
                .OnDelete(DeleteBehavior.Restrict);

            });

            // ------------------------------------------------------------------------------------------------------------------------------------
            //-----------------------------Payments---------------------------------------------------
            //-------------------------------------------------------------------------------------------------------------------------------------

            modelBuilder.Entity<PaymentDetails>(entity => {
                entity.HasOne(e => e.Payments)
                .WithMany()//Paymenthod can be refrenced in many PaymentDetails Records
                .HasForeignKey(e=> e.PaymentMethodID)
                .OnDelete(DeleteBehavior.Restrict);

            });


            //________Cart__________________________________________

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("Cart");

                entity.HasKey(e => e.CartId);

                entity.Property(e => e.CartId)
                    .ValueGeneratedOnAdd(); // Same as [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.Property(e => e.CartName)
                    .HasMaxLength(100)
                    .HasColumnType("nvarchar(100)");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime");

                // ─── Relationships ───────────────────────────────────────────────────


                // Foreign key to Users
                entity.HasOne(e => e.User)
                    .WithMany() // or WithMany(u => u.Carts) if User class has a collection
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade); // Optional based on your business rules

                // One-to-many relationship with CartItems
                entity.HasMany(e => e.CartItems)
                    .WithOne(ci => ci.Cart)
                    .HasForeignKey(ci => ci.CartId)
                    .OnDelete(DeleteBehavior.Cascade); // Optional, can be Restrict, SetNull, etc.
            });

            // ─── CartItems ───────────────────────────────────────────────────

            modelBuilder.Entity<CartItem>(entity => {

                entity.ToTable("CartItem");

                entity.HasKey(e => e.CartItemId);

                entity.Property(e => e.ProductId)
                .HasColumnType("nvarchar(50)")
                ;

                entity.Property(e => e.Quantity)
                .HasColumnType("int")
                .IsRequired();


                entity.Property(e => e.CartId)
                .HasColumnType("int")
                .IsRequired();

                entity.Property(e => e.CreatedOn)
                .HasColumnType("Datetime");


                entity.Property(e => e.UpdatedOn)
                .HasColumnType("Datetime");

                // ─── Relationships ───────────────────────────────────────────────────

                entity.HasOne(e => e.Cart)
                .WithMany(c=>c.CartItems)
                .HasForeignKey("CartId")
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Products)
                .WithMany()
                .HasForeignKey("ProductId")
                .OnDelete(DeleteBehavior.Cascade);


            });


            // ─── BuyAgain ───────────────────────────────────────────────────


            modelBuilder.Entity<BuyAgain>(entity =>
            {
                entity.ToTable("BuyAgain");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnType("int");

                entity.Property(e => e.ProductId)
                    .IsRequired()
                    .HasColumnType("nvarchar(50)");

                entity.Property(e => e.PurchasedOn)
                    .IsRequired()
                    .HasColumnType("datetime");

                entity.Property(e => e.Quantity)
                    .IsRequired()
                    .HasColumnType("int");

                // ─── Relationships ───────────────────────────────────────────────────


                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnType("bit")
                    .HasDefaultValue(true);
                entity.HasOne(e => e.User)
                .WithMany()//a user can have many buy again products
                .HasForeignKey(e=>e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Products)
                .WithMany()
                .HasForeignKey(e=>e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            // ─── SavedItems ───────────────────────────────────────────────────


            modelBuilder.Entity<SavedItems>(entity =>
            {
                entity.ToTable("SavedItems");

                entity.HasKey(e => e.Id); 

                entity.Property(e => e.UserId) 
                .IsRequired()
                .HasColumnType("int");

                entity.Property(e => e.ProductId)
                .IsRequired().
                HasColumnType("nVarchar(50)");

                entity.Property(e => e.SavedOn)
                .IsRequired()
                .HasColumnType("datetime");

            entity.Property(e => e.Quantity)
                .IsRequired()
                .HasColumnType("int");


            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasColumnType("bit")
                .HasDefaultValue(true);


                entity.HasOne(e =>e.User)
                .WithMany()
                .HasForeignKey(e=>e.UserId) 
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Products)
                .WithMany()
                .HasForeignKey(e=>e.ProductId) 
                .OnDelete(DeleteBehavior.Cascade);
            });


            }
    }
}
