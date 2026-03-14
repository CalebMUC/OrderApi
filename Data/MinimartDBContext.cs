using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Minimart_Api.DTOS.Products;
using Minimart_Api.Models;

namespace Minimart_Api.Data
{
    public class MinimartDBContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public MinimartDBContext(DbContextOptions<MinimartDBContext> options) : base(options) { }
        
        // Only modern Identity system models
        public virtual DbSet<Addresses> Addresses { get; set; }
        public virtual DbSet<Cart> Cart { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<SavedItems> SavedItems { get; set; }

        // Merchant system models
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
        public virtual DbSet<SubSubCategory> SubSubCategories { get; set; }
        public virtual DbSet<Merchants> Merchants { get; set; }

        public virtual DbSet<Counties> Counties { get; set; }
        public virtual DbSet<DeliveryStations> DeliveryStations { get; set; }
        public virtual DbSet<Features> Features { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<OrderProduct> OrderProducts { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderStatus> OrderStatuses { get; set; }
        public virtual DbSet<OrderTracking> OrderTracking{ get; set; }
        public virtual DbSet<PaymentDetails> PaymentDetails { get; set; }
        public virtual DbSet<PaymentMethods> PaymentMethods { get; set; }
        public virtual DbSet<MerchantPaymentMethod> MerchantPaymentMethods { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Reviews> Reviews { get; set; }
        public virtual DbSet<Towns> Towns { get; set; }

        // Authentication related models
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<UserLoginAttempt> UserLoginAttempts { get; set; }

        public DbSet<MpesaTransaction> MpesaTransactions { get; set; }

        // Payout system models
        public virtual DbSet<Payout> Payouts { get; set; }
        public virtual DbSet<PayoutTransaction> PayoutTransactions { get; set; }

        public virtual DbSet<SlugRedirect> SlugRedirects { get; set; } // ADD THIS LINE
        
        // Guest checkout models
        public virtual DbSet<GuestCart> GuestCarts { get; set; }
        public virtual DbSet<GuestCheckout> GuestCheckouts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call base method first for Identity tables
            base.OnModelCreating(modelBuilder);

            // Configure authentication models
            ConfigureAuthenticationEntities(modelBuilder);

            // Products configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(p => p.ProductId);

                // Configure category relationships with Guid IDs
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.SubCategory)
                    .WithMany(sc => sc.Products)
                    .HasForeignKey(p => p.SubCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.SubSubCategory)
                    .WithMany(ssc => ssc.Products)
                    .HasForeignKey(p => p.SubSubCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Merchant)
                    .WithMany(m => m.Products)
                    .HasForeignKey(p => p.MerchantID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configure audit fields
                entity.Property(p => p.CreatedOn).HasColumnType("timestamp with time zone");
                entity.Property(p => p.UpdatedOn).HasColumnType("timestamp with time zone");
                entity.Property(p => p.DeletedOn).HasColumnType("timestamp with time zone");

                // Configure ImageUrls as text array
                entity.Property(p => p.ImageUrls).HasColumnType("text[]");
            });

            // Features configuration
            modelBuilder.Entity<Features>(entity =>
            {
                entity.ToTable("Features");
                entity.HasKey(f => f.FeatureID);

                // Configure category system relationships
                entity.HasOne(f => f.Category)
                    .WithMany()
                    .HasForeignKey(f => f.CategoryID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(f => f.SubCategory)
                    .WithMany()
                    .HasForeignKey(f => f.SubCategoryID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(f => f.SubSubCategory)
                    .WithMany()
                    .HasForeignKey(f => f.SubSubCategoryID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Category system configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(c => c.CategoryId);

                entity.HasOne(c => c.Merchant)
                    .WithMany(m => m.Categories)
                    .HasForeignKey(c => c.MerchantID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Parent)
                    .WithMany(c => c.Children)
                    .HasForeignKey(c => c.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SubCategory>(entity =>
            {
                entity.ToTable("SubCategories");
                entity.HasKey(sc => sc.SubCategoryId);

                entity.HasOne(sc => sc.Category)
                    .WithMany(c => c.SubCategories)
                    .HasForeignKey(sc => sc.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SubSubCategory>(entity =>
            {
                entity.ToTable("SubSubCategories");
                entity.HasKey(ssc => ssc.SubSubCategoryId);

                entity.HasOne(ssc => ssc.SubCategory)
                    .WithMany(sc => sc.SubSubCategories)
                    .HasForeignKey(ssc => ssc.SubCategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure other important entities
            ConfigureOrderEntities(modelBuilder);
            ConfigureUserEntities(modelBuilder);
            ConfigureAdditionalEntities(modelBuilder);
        }

        private void ConfigureAuthenticationEntities(ModelBuilder modelBuilder)
        {
            // Configure RefreshToken entity
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshTokens");
                entity.HasKey(rt => rt.Id);

                entity.Property(rt => rt.Token).IsRequired().HasMaxLength(500);
                entity.Property(rt => rt.JwtId).IsRequired().HasMaxLength(200);
                entity.Property(rt => rt.ApplicationUserId).IsRequired().HasMaxLength(450);

                entity.Property(rt => rt.CreationDate).HasColumnType("timestamp with time zone");
                entity.Property(rt => rt.ExpiryDate).HasColumnType("timestamp with time zone");

                // Configure relationship with ApplicationUser
                entity.HasOne(rt => rt.User)
                    .WithMany()
                    .HasForeignKey(rt => rt.ApplicationUserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Add indexes for performance
                entity.HasIndex(rt => rt.Token).IsUnique();
                entity.HasIndex(rt => rt.JwtId);
                entity.HasIndex(rt => rt.ApplicationUserId);
                entity.HasIndex(rt => rt.ExpiryDate);
            });

            // Configure UserLoginAttempt entity
            modelBuilder.Entity<UserLoginAttempt>(entity =>
            {
                entity.ToTable("UserLoginAttempts");
                entity.HasKey(ula => ula.Id);

                entity.Property(ula => ula.Email).IsRequired().HasMaxLength(255);
                entity.Property(ula => ula.IpAddress).IsRequired().HasMaxLength(45);
                entity.Property(ula => ula.UserAgent).HasMaxLength(500);
                entity.Property(ula => ula.FailureReason).HasMaxLength(255);

                entity.Property(ula => ula.AttemptDate).HasColumnType("timestamp with time zone");

                // Add indexes for performance and security queries
                entity.HasIndex(ula => ula.Email);
                entity.HasIndex(ula => ula.IpAddress);
                entity.HasIndex(ula => ula.AttemptDate);
                entity.HasIndex(ula => new { ula.Email, ula.AttemptDate });
                entity.HasIndex(ula => new { ula.IpAddress, ula.AttemptDate });
            });
        }

        private void ConfigureAdditionalEntities(ModelBuilder modelBuilder)
        {
            // Configure SlugRedirect entity (SEO)
            modelBuilder.Entity<SlugRedirect>(entity =>
            {
                entity.ToTable("SlugRedirects");
                entity.HasKey(sr => sr.RedirectId); // FIXED: Changed from Id to RedirectId
                
                entity.Property(sr => sr.CreatedAt)
                    .HasColumnType("timestamp with time zone");
                
                entity.Property(sr => sr.IsActive)
                    .HasDefaultValue(true);
                
                entity.HasOne(sr => sr.Product)
                    .WithMany()
                    .HasForeignKey(sr => sr.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasIndex(sr => sr.OldSlug);
                entity.HasIndex(sr => sr.NewSlug);
                entity.HasIndex(sr => sr.ProductId);
                entity.HasIndex(sr => sr.IsActive);
            });

            // Configure CartItem entity
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.ToTable("CartItems");
                entity.HasKey(ci => ci.CartItemId);
                
                // Configure relationship with Product
                entity.HasOne(ci => ci.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(ci => ci.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                // Add indexes
                entity.HasIndex(ci => ci.CartId);
                entity.HasIndex(ci => ci.ProductId);
            });

            // Configure Counties entity
            modelBuilder.Entity<Counties>(entity =>
            {
                entity.ToTable("Counties");
                entity.HasKey(c => c.CountyId);
                
                entity.Property(c => c.CreatedOn).HasColumnType("timestamp with time zone");
                
                // Configure relationship with Towns
                entity.HasMany(c => c.Towns)
                    .WithOne(t => t.County)
                    .HasForeignKey(t => t.CountyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Towns entity
            modelBuilder.Entity<Towns>(entity =>
            {
                entity.ToTable("Towns");
                entity.HasKey(t => t.TownId);
                
                entity.Property(t => t.CreatedOn).HasColumnType("timestamp with time zone");
                
                // Configure relationship with DeliveryStations
                entity.HasMany(t => t.DeliveryStations)
                    .WithOne(ds => ds.Town)
                    .HasForeignKey(ds => ds.TownId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                // Add index
                entity.HasIndex(t => t.CountyId);
            });

            // Configure DeliveryStations entity
            modelBuilder.Entity<DeliveryStations>(entity =>
            {
                entity.ToTable("DeliveryStations");
                entity.HasKey(ds => ds.DeliveryStationId);
                
                entity.Property(ds => ds.CreatedOn).HasColumnType("timestamp with time zone");
                
                // Add indexes
                entity.HasIndex(ds => ds.TownId);
            });

            // Configure PaymentMethods entity
            modelBuilder.Entity<PaymentMethods>(entity =>
            {
                entity.ToTable("PaymentMethods");
                entity.HasKey(pm => pm.PaymentMethodID);
                
                // Configure relationship with PaymentDetails
                entity.HasMany(pm => pm.PaymentDetails)
                    .WithOne(pd => pd.PaymentMethod)
                    .HasForeignKey(pd => pd.PaymentMethodID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure MerchantPaymentMethod entity
            modelBuilder.Entity<MerchantPaymentMethod>(entity =>
            {
                entity.ToTable("MerchantPaymentMethods");
                entity.HasKey(mpm => mpm.Id);
                
                // Configure relationship with Merchant
                entity.HasOne(mpm => mpm.Merchant)
                    .WithMany(m => m.MerchantPaymentMethods)
                    .HasForeignKey(mpm => mpm.MerchantId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Configure relationship with PaymentMethod
                entity.HasOne(mpm => mpm.PaymentMethod)
                    .WithMany(pm => pm.MerchantPaymentMethods)
                    .HasForeignKey(mpm => mpm.PaymentMethodId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                // Add unique constraint - one payment method per merchant
                entity.HasIndex(mpm => new { mpm.MerchantId, mpm.PaymentMethodId })
                    .IsUnique()
                    .HasDatabaseName("UQ_MerchantPaymentMethods_MerchantId_PaymentMethodId");
                
                // Add indexes for performance
                entity.HasIndex(mpm => mpm.MerchantId)
                    .HasDatabaseName("IX_MerchantPaymentMethods_MerchantId");
                entity.HasIndex(mpm => mpm.PaymentMethodId)
                    .HasDatabaseName("IX_MerchantPaymentMethods_PaymentMethodId");
                entity.HasIndex(mpm => mpm.IsEnabled)
                    .HasDatabaseName("IX_MerchantPaymentMethods_IsEnabled");
                
                // Configure timestamp fields
                entity.Property(mpm => mpm.CreatedAt)
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("NOW()");
                    
                entity.Property(mpm => mpm.UpdatedAt)
                    .HasColumnType("timestamp with time zone");
                    
                // Configure Configuration field
                entity.Property(mpm => mpm.Configuration)
                    .HasMaxLength(500);
            });

            // Configure PaymentDetails entity
            modelBuilder.Entity<PaymentDetails>(entity =>
            {
                entity.ToTable("PaymentDetails");
                entity.HasKey(pd => pd.PaymentID);
                
                // Add indexes for performance
                entity.HasIndex(pd => pd.TrxReference).IsUnique();
                entity.HasIndex(pd => pd.PaymentReference);
                entity.HasIndex(pd => pd.PaymentMethodID);
            });

            // Configure MpesaTransaction entity
            modelBuilder.Entity<MpesaTransaction>(entity =>
            {
                entity.ToTable("MpesaTransactions");
                entity.HasKey(mt => mt.Id);
                
                entity.Property(mt => mt.CreatedAt).HasColumnType("timestamp with time zone");
                
                // Add useful indexes for Mpesa transactions
                entity.HasIndex(mt => mt.TransID);
                entity.HasIndex(mt => mt.BillRefNumber);
                entity.HasIndex(mt => mt.MSISDN);
                entity.HasIndex(mt => mt.BusinessShortCode);
            });

            // Configure OrderItem entity (if still used)
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItems");
                entity.HasKey(oi => oi.OrderItemId);
                
                // Configure relationship with Product
                entity.HasOne(oi => oi.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(oi => oi.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                // Add indexes
                entity.HasIndex(oi => oi.ProductId);
            });

            // Configure Payout entities
            ConfigurePayoutEntities(modelBuilder);
            
            // Configure Guest Checkout entities
            ConfigureGuestCheckoutEntities(modelBuilder);
        }

        private void ConfigureGuestCheckoutEntities(ModelBuilder modelBuilder)
        {
            // Configure GuestCart entity
            modelBuilder.Entity<GuestCart>(entity =>
            {
                entity.ToTable("GuestCarts");
                entity.HasKey(gc => gc.GuestCartId);
                
                // Configure relationship with Product
                entity.HasOne(gc => gc.Product)
                    .WithMany()
                    .HasForeignKey(gc => gc.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Configure timestamp fields
                entity.Property(gc => gc.CreatedAt).HasColumnType("timestamp with time zone");
                entity.Property(gc => gc.UpdatedAt).HasColumnType("timestamp with time zone");
                entity.Property(gc => gc.ExpiresAt).HasColumnType("timestamp with time zone");
                
                // Add indexes for performance
                entity.HasIndex(gc => gc.GuestId);
                entity.HasIndex(gc => gc.ProductId);
                entity.HasIndex(gc => gc.ExpiresAt);
                entity.HasIndex(gc => new { gc.GuestId, gc.ProductId });
            });
            
            // Configure GuestCheckout entity
            modelBuilder.Entity<GuestCheckout>(entity =>
            {
                entity.ToTable("GuestCheckouts");
                entity.HasKey(gco => gco.GuestCheckoutId);
                
                // Configure relationships
                entity.HasOne(gco => gco.County)
                    .WithMany()
                    .HasForeignKey(gco => gco.CountyId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                entity.HasOne(gco => gco.Town)
                    .WithMany()
                    .HasForeignKey(gco => gco.TownId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                entity.HasOne(gco => gco.DeliveryStation)
                    .WithMany()
                    .HasForeignKey(gco => gco.DeliveryStationId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                entity.HasOne(gco => gco.Order)
                    .WithOne(o => o.GuestCheckout)
                    .HasForeignKey<GuestCheckout>(gco => gco.OrderId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                // Configure decimal precision
                entity.Property(gco => gco.DeliveryFee).HasColumnType("decimal(18,2)");
                entity.Property(gco => gco.SubtotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(gco => gco.TotalAmount).HasColumnType("decimal(18,2)");
                
                // Configure timestamp fields
                entity.Property(gco => gco.CreatedAt).HasColumnType("timestamp with time zone");
                entity.Property(gco => gco.CompletedAt).HasColumnType("timestamp with time zone");
                
                // Add indexes for performance
                entity.HasIndex(gco => gco.GuestId);
                entity.HasIndex(gco => gco.PhoneNumber);
                entity.HasIndex(gco => gco.Status);
                entity.HasIndex(gco => gco.CheckoutRequestId);
                entity.HasIndex(gco => gco.MerchantRequestId);
                entity.HasIndex(gco => gco.OrderId);
                entity.HasIndex(gco => gco.CreatedAt);
            });
        }

        private void ConfigureOrderEntities(ModelBuilder modelBuilder)
        {
            // Configure Order entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(o => o.OrderID);

                entity.Property(o => o.OrderDate).HasColumnType("timestamp with time zone");
                entity.Property(o => o.DeliveryScheduleDate).HasColumnType("timestamp with time zone");

                entity.Property(o => o.TotalOrderAmount).HasColumnType("decimal(18,2)");
                entity.Property(o => o.TotalPaymentAmount).HasColumnType("decimal(18,2)");
                entity.Property(o => o.TotalDeliveryFees).HasColumnType("decimal(18,2)");
                entity.Property(o => o.TotalTax).HasColumnType("decimal(18,2)");

                // Relationship with ApplicationUser
                entity.HasOne(o => o.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.ApplicationUserId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Relationship with OrderStatus
                entity.HasOne(o => o.OrderStatus)
                    .WithMany()
                    .HasForeignKey(o => o.StatusID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relationship with PaymentDetails
                entity.HasOne(o => o.PaymentDetails)
                    .WithMany()
                    .HasForeignKey(o => o.PaymentID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Add indexes
                entity.HasIndex(o => o.ApplicationUserId);
                entity.HasIndex(o => o.StatusID);
                entity.HasIndex(o => o.OrderDate);
            });

            // Configure OrderStatus entity
            modelBuilder.Entity<OrderStatus>(entity =>
            {
                entity.ToTable("OrderStatuses");
                entity.HasKey(os => os.StatusID);
            });

            // Configure OrderTracking entity
            modelBuilder.Entity<OrderTracking>(entity =>
            {
                entity.ToTable("OrderTracking");
                entity.HasKey(ot => ot.TrackingID);

                entity.HasOne(ot => ot.Order)
                    .WithMany(o => o.OrderTrackings)
                    .HasForeignKey(ot => ot.OrderID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(ot => ot.OrderID);
            });

            // Configure OrderProduct entity
            modelBuilder.Entity<OrderProduct>(entity =>
            {
                entity.ToTable("OrderProducts");
                entity.HasKey(op => op.OrderProductID);

                entity.HasOne(op => op.Order)
                    .WithMany(o => o.OrderProducts)
                    .HasForeignKey(op => op.OrderID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(op => op.OrderID);
            });
        }

        private void ConfigureUserEntities(ModelBuilder modelBuilder)
        {
            // Configure ApplicationUser entity
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.LastLogin).HasColumnType("timestamp with time zone");
                entity.Property(u => u.CreatedAt).HasColumnType("timestamp with time zone");
                entity.Property(u => u.PasswordChangesOn).HasColumnType("timestamp with time zone");
                entity.Property(u => u.LastPasswordReset).HasColumnType("timestamp with time zone");
                entity.Property(u => u.LastLoginDate).HasColumnType("timestamp with time zone");
                entity.Property(u => u.TemporaryPasswordExpiry).HasColumnType("timestamp with time zone");
            });

            // Configure Cart entity
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("Cart");
                entity.HasKey(c => c.CartId);

                entity.HasOne(c => c.User)
                    .WithMany(u => u.Carts)
                    .HasForeignKey(c => c.ApplicationUserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(c => c.UpdatedAt).HasColumnType("timestamp with time zone");

                entity.HasIndex(c => c.ApplicationUserId);
            });

            // Configure SavedItems entity
            modelBuilder.Entity<SavedItems>(entity =>
            {
                entity.ToTable("SavedItems");
                entity.HasKey(si => si.Id);
                
                entity.HasIndex(si => si.ApplicationUserId);
                entity.HasIndex(si => si.ProductId);
            });

            // Configure Addresses entity
            modelBuilder.Entity<Addresses>(entity =>
            {
                entity.ToTable("Addresses");
                entity.HasKey(a => a.AddressID);

                entity.HasOne(a => a.User)
                    .WithMany(u => u.Addresses)
                    .HasForeignKey(a => a.ApplicationUserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(a => a.ApplicationUserId);
            });

            // Configure Reviews entity
            modelBuilder.Entity<Reviews>(entity =>
            {
                entity.ToTable("Reviews");
                entity.HasKey(r => r.ReviewId);

                entity.HasOne(r => r.User)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(r => r.ApplicationUserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Product)
                    .WithMany()
                    .HasForeignKey(r => r.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(r => r.ApplicationUserId);
                entity.HasIndex(r => r.ProductId);
            });

            // Configure Merchants entity
            modelBuilder.Entity<Merchants>(entity =>
            {
                entity.ToTable("Merchants");
                entity.HasKey(m => m.MerchantID);

                entity.HasOne(m => m.User)
                    .WithOne(u => u.Merchant)
                    .HasForeignKey<Merchants>(m => m.ApplicationUserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(m => m.ApplicationUserId);
            });
        }

        private void ConfigurePayoutEntities(ModelBuilder modelBuilder)
        {
            // Configure Payout entity
            modelBuilder.Entity<Payout>(entity =>
            {
                entity.ToTable("Payouts");
                entity.HasKey(p => p.PayoutId);

                entity.Property(p => p.GrossAmount).HasColumnType("decimal(18,2)");
                entity.Property(p => p.CommissionAmount).HasColumnType("decimal(18,2)");
                entity.Property(p => p.CommissionRate).HasColumnType("decimal(5,4)");
                entity.Property(p => p.NetAmount).HasColumnType("decimal(18,2)");

                entity.Property(p => p.PeriodStartDate).HasColumnType("timestamp with time zone");
                entity.Property(p => p.PeriodEndDate).HasColumnType("timestamp with time zone");
                entity.Property(p => p.CreatedDate).HasColumnType("timestamp with time zone");
                entity.Property(p => p.ScheduledDate).HasColumnType("timestamp with time zone");
                entity.Property(p => p.CompletedDate).HasColumnType("timestamp with time zone");
                entity.Property(p => p.UpdatedDate).HasColumnType("timestamp with time zone");

                // Relationship with Merchant
                entity.HasOne(p => p.Merchant)
                    .WithMany()
                    .HasForeignKey(p => p.MerchantId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relationship with PaymentMethod
                entity.HasOne(p => p.PaymentMethod)
                    .WithMany()
                    .HasForeignKey(p => p.PaymentMethodId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Add indexes
                entity.HasIndex(p => p.MerchantId);
                entity.HasIndex(p => p.Status);
                entity.HasIndex(p => p.CreatedDate);
                entity.HasIndex(p => p.PeriodStartDate);
                entity.HasIndex(p => p.PeriodEndDate);
            });

            // Configure PayoutTransaction entity
            modelBuilder.Entity<PayoutTransaction>(entity =>
            {
                entity.ToTable("PayoutTransactions");
                entity.HasKey(pt => pt.PayoutTransactionId);

                entity.Property(pt => pt.OrderAmount).HasColumnType("decimal(18,2)");
                entity.Property(pt => pt.CommissionAmount).HasColumnType("decimal(18,2)");
                entity.Property(pt => pt.NetAmount).HasColumnType("decimal(18,2)");
                entity.Property(pt => pt.CommissionRate).HasColumnType("decimal(5,4)");

                entity.Property(pt => pt.OrderCompletedDate).HasColumnType("timestamp with time zone");
                entity.Property(pt => pt.CreatedDate).HasColumnType("timestamp with time zone");

                // Relationship with Payout
                entity.HasOne(pt => pt.Payout)
                    .WithMany(p => p.PayoutTransactions)
                    .HasForeignKey(pt => pt.PayoutId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relationship with Order
                entity.HasOne(pt => pt.Order)
                    .WithMany()
                    .HasForeignKey(pt => pt.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Add indexes
                entity.HasIndex(pt => pt.PayoutId);
                entity.HasIndex(pt => pt.OrderId);
            });
        }
    }
}