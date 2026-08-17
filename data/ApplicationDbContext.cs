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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<NguoiDung>()
                .HasIndex(n => n.TenDangNhap)
                .IsUnique();

            modelBuilder.Entity<NguoiThue>()
                .HasIndex(n => n.CCCD)
                .IsUnique();

            // Không cho xóa Phòng nếu đang có Hợp đồng liên kết (tránh mất dữ liệu lịch sử)
            modelBuilder.Entity<HopDong>()
                .HasOne(h => h.Phong)
                .WithMany()
                .HasForeignKey(h => h.MaPhong)
                .OnDelete(DeleteBehavior.Restrict);

            // Tương tự với Người thuê
            modelBuilder.Entity<HopDong>()
                .HasOne(h => h.NguoiThue)
                .WithMany()
                .HasForeignKey(h => h.MaNguoiThue)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}