using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Minimart_Api.Models
{
    public partial class muchiriDBContext : DbContext
    {
        public muchiriDBContext()
        {
        }

        public muchiriDBContext(DbContextOptions<muchiriDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TGlobalQoute> TGlobalQoutes { get; set; } = null!;
        public virtual DbSet<TStockDetail> TStockDetails { get; set; } = null!;
        public virtual DbSet<TUser> TUsers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=172.16.2.14;Database = muchiriDB;User Id = Realm; Password = friend;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TGlobalQoute>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("t_GlobalQoute");

                entity.Property(e => e.Change)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("change");

                entity.Property(e => e.ChangePercent)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.High)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Low)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Opening)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.PreviousClose)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Price)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Symbol)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("symbol");

                entity.Property(e => e.TradingDay)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Volume)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TStockDetail>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("t_StockDetails");

                entity.Property(e => e.AssetType)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Exchange)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Ipodate)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(110)
                    .IsUnicode(false);

                entity.Property(e => e.Symbol)
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TUser>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("t_user");

                //entity.Property(e => e.EmailAddress)
                //    .HasMaxLength(50)
                //    .IsUnicode(false);

                //entity.Property(e => e.Name)
                //    .HasMaxLength(50)
                //    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                //entity.Property(e => e.RoleID)
                //    .HasMaxLength(20)
                //    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
