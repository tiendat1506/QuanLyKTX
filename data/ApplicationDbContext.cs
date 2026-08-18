using Microsoft.EntityFrameworkCore;
using QuanLyKTX.Models;

namespace QuanLyKTX.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Phong> Phongs { get; set; } = null!;
        public DbSet<NguoiDung> NguoiDungs { get; set; } = null!;
        public DbSet<NguoiThue> NguoiThues { get; set; } = null!;
        public DbSet<HopDong> HopDongs { get; set; } = null!;
        public DbSet<ChiSoDienNuoc> ChiSoDienNuocs { get; set; } = null!;
        public DbSet<BacGia> BacGias { get; set; } = null!;
        public DbSet<HoaDon> HoaDons { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<NguoiDung>()
                .HasIndex(n => n.TenDangNhap)
                .IsUnique();

            modelBuilder.Entity<NguoiThue>()
                .HasIndex(n => n.CCCD)
                .IsUnique();

            modelBuilder.Entity<HopDong>()
                .HasOne(h => h.Phong)
                .WithMany()
                .HasForeignKey(h => h.MaPhong)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HopDong>()
                .HasOne(h => h.NguoiThue)
                .WithMany()
                .HasForeignKey(h => h.MaNguoiThue)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChiSoDienNuoc>()
                .HasIndex(c => new { c.MaPhong, c.Thang, c.Nam })
                .IsUnique();

            modelBuilder.Entity<ChiSoDienNuoc>()
                .HasOne(c => c.Phong)
                .WithMany()
                .HasForeignKey(c => c.MaPhong)
                .OnDelete(DeleteBehavior.Restrict);

            // Mỗi hợp đồng chỉ có 1 hóa đơn / tháng - tránh lập trùng
            modelBuilder.Entity<HoaDon>()
                .HasIndex(h => new { h.MaHopDong, h.Thang, h.Nam })
                .IsUnique();

            modelBuilder.Entity<HoaDon>()
                .HasOne(h => h.HopDong)
                .WithMany()
                .HasForeignKey(h => h.MaHopDong)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}