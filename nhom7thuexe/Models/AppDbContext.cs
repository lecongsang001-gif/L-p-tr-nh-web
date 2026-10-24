using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace nhom7thuexe.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BangGium> BangGia { get; set; }

    public virtual DbSet<ChiTietDonThue> ChiTietDonThues { get; set; }

    public virtual DbSet<DonThueXe> DonThueXes { get; set; }

    public virtual DbSet<HopDong> HopDongs { get; set; }

    public virtual DbSet<HuyDonXe> HuyDonXes { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<PhuThu> PhuThus { get; set; }

    public virtual DbSet<SuCoDichVu> SuCoDichVus { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<ThanhToan> ThanhToans { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    public virtual DbSet<Xe> Xes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=CHOCOOPIE\\SQLEXPRESS;Database=Motorbike;User Id=nhom7;Password=123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BangGium>(entity =>
        {
            entity.HasKey(e => e.MaBangGia).HasName("PK__BangGia__5DAC5A68539B7910");

            entity.Property(e => e.GiaTheoNgay).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiaTheoThang).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiaTheoTuan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaXe)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TienCoc).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaXeNavigation).WithMany(p => p.BangGia)
                .HasForeignKey(d => d.MaXe)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BangGia_Xe");
        });

        modelBuilder.Entity<ChiTietDonThue>(entity =>
        {
            entity.HasKey(e => new { e.MaDon, e.MaXe }).HasName("PK__ChiTietD__FFFBA7647FF64E4B");

            entity.ToTable("ChiTietDonThue");

            entity.Property(e => e.MaDon)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaXe)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.DonGiaThue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LyDoPhuThu).HasMaxLength(255);
            entity.Property(e => e.TienPhuThu)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.MaDonNavigation).WithMany(p => p.ChiTietDonThues)
                .HasForeignKey(d => d.MaDon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTiet_DonThue");

            entity.HasOne(d => d.MaXeNavigation).WithMany(p => p.ChiTietDonThues)
                .HasForeignKey(d => d.MaXe)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTiet_Xe");
        });

        modelBuilder.Entity<DonThueXe>(entity =>
        {
            entity.HasKey(e => e.MaDon).HasName("PK__DonThueX__3D89F568709BD3DF");

            entity.ToTable("DonThueXe");

            entity.Property(e => e.MaDon)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CocXe)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiaDiemGiaoXe).HasMaxLength(255);
            entity.Property(e => e.DiaDiemTraXe).HasMaxLength(255);
            entity.Property(e => e.MaKhachHang)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaNhanVien)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NgayDat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TongTien)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("Chờ xác nhận");

            entity.HasOne(d => d.MaDonNavigation).WithOne(p => p.DonThueXe)
                .HasPrincipalKey<HopDong>(p => p.MaDon)
                .HasForeignKey<DonThueXe>(d => d.MaDon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonThueXe_HopDong");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.DonThueXes)
                .HasForeignKey(d => d.MaKhachHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonThue_KhachHang");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.DonThueXes)
                .HasForeignKey(d => d.MaNhanVien)
                .HasConstraintName("FK_DonThue_NhanVien");
        });

        modelBuilder.Entity<HopDong>(entity =>
        {
            entity.HasKey(e => e.MaHopDong);

            entity.ToTable("HopDong");

            entity.HasIndex(e => e.MaDon, "UQ_MaDon").IsUnique();

            entity.Property(e => e.MaHopDong)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaDon)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TrangThai).HasMaxLength(20);
        });

        modelBuilder.Entity<HuyDonXe>(entity =>
        {
            entity.HasKey(e => e.MaHuyDon).HasName("PK__HuyDonXe__BFA1AA1A55036462");

            entity.ToTable("HuyDonXe");

            entity.Property(e => e.LyDoHuy).HasMaxLength(255);
            entity.Property(e => e.MaDon)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaNhanVien)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NgayHuy)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TienHoanTra)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TienPhatHuy)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThaiHuy)
                .HasMaxLength(50)
                .HasDefaultValue("Đã xử lý");

            entity.HasOne(d => d.MaDonNavigation).WithMany(p => p.HuyDonXes)
                .HasForeignKey(d => d.MaDon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HuyDon_DonThueXe");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.HuyDonXes)
                .HasForeignKey(d => d.MaNhanVien)
                .HasConstraintName("FK_HuyDon_NhanVien");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKhachHang).HasName("PK__KhachHan__88D2F0E5EF01F107");

            entity.ToTable("KhachHang");

            entity.HasIndex(e => e.Cccd, "UQ__KhachHan__A955A0AAF5C74B3F").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__KhachHan__A9D10534BD3DF665").IsUnique();

            entity.HasIndex(e => e.Sdt, "UQ__KhachHan__CA1930A50AC4FD3A").IsUnique();

            entity.Property(e => e.MaKhachHang)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CapDoVip)
                .HasMaxLength(30)
                .HasDefaultValue("Thường")
                .HasColumnName("CapDoVIP");
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CCCD");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Gplx)
                .HasDefaultValue(false)
                .HasColumnName("GPLX");
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("SDT");

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithMany(p => p.KhachHangs)
                .HasForeignKey(d => d.MaTaiKhoan)
                .HasConstraintName("FK__KhachHang__MaTai__22401542");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNhanVien).HasName("PK__NhanVien__77B2CA4770521206");

            entity.ToTable("NhanVien");

            entity.HasIndex(e => e.Email, "UQ__NhanVien__A9D105348A2576C7").IsUnique();

            entity.HasIndex(e => e.Sdt, "UQ__NhanVien__CA1930A5B939D9E8").IsUnique();

            entity.Property(e => e.MaNhanVien)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ChucVu).HasMaxLength(50);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("SDT");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("Đang làm");

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.MaTaiKhoan)
                .HasConstraintName("FK__NhanVien__MaTaiK__214BF109");
        });

        modelBuilder.Entity<PhuThu>(entity =>
        {
            entity.HasKey(e => e.MaPhuThu).HasName("PK__PhuThu__FD0E3BFA92298097");

            entity.ToTable("PhuThu");

            entity.Property(e => e.MaPhuThu)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LoaiPhuThu).HasMaxLength(100);
            entity.Property(e => e.LyDo).HasMaxLength(500);
            entity.Property(e => e.MaDon)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaNhanVien)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaSuCo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NgayGhiNhan)
                .HasDefaultValueSql("(getdate())", "DF__PhuThu__NgayGhiN__625A9A57")
                .HasColumnType("datetime");
            entity.Property(e => e.SoTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Chờ xác nhận", "DF__PhuThu__TrangTha__634EBE90");

            entity.HasOne(d => d.MaDonNavigation).WithMany(p => p.PhuThus)
                .HasForeignKey(d => d.MaDon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhuThu_DonThueXe");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.PhuThus)
                .HasForeignKey(d => d.MaNhanVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhuThu_NhanVien");

            entity.HasOne(d => d.MaSuCoNavigation).WithMany(p => p.PhuThus)
                .HasForeignKey(d => d.MaSuCo)
                .HasConstraintName("FK_PhuThu_SuCoDichVu");
        });

        modelBuilder.Entity<SuCoDichVu>(entity =>
        {
            entity.HasKey(e => e.MaSuCo).HasName("PK__SuCoDich__A69DF79FE26FD927");

            entity.ToTable("SuCoDichVu");

            entity.Property(e => e.MaSuCo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ChiPhiPhatSinh)
                .HasDefaultValue(0m, "DF__SuCoDichV__ChiPh__4D5F7D71")
                .HasColumnType("decimal(18, 0)");
            entity.Property(e => e.LoaiSuCo).HasMaxLength(100);
            entity.Property(e => e.MaDon)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaXe)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NgayGhiNhan)
                .HasDefaultValueSql("(getdate())", "DF__SuCoDichV__NgayG__4E53A1AA")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaDonNavigation).WithMany(p => p.SuCoDichVus)
                .HasForeignKey(d => d.MaDon)
                .HasConstraintName("FK_SuCo_DonThue");

            entity.HasOne(d => d.MaXeNavigation).WithMany(p => p.SuCoDichVus)
                .HasForeignKey(d => d.MaXe)
                .HasConstraintName("FK_SuCo_Xe");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan).HasName("PK__TaiKhoan__AD7C6529A4B46B1B");

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.TenDangNhap, "UQ__TaiKhoan__55F68FC0B45C613B").IsUnique();

            entity.Property(e => e.MaVaiTro)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.MaVaiTro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TaiKhoan__MaVaiT__2057CCD0");
        });

        modelBuilder.Entity<ThanhToan>(entity =>
        {
            entity.HasKey(e => e.MaThanhToan).HasName("PK__ThanhToa__D4B258447DDBB98D");

            entity.ToTable("ThanhToan");

            entity.Property(e => e.MaThanhToan)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaDon)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NgayThanhToan).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PhuongThuc).HasMaxLength(50);
            entity.Property(e => e.SoTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TienCoc)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("Chờ thanh toán");

            entity.HasOne(d => d.MaDonNavigation).WithMany(p => p.ThanhToans)
                .HasForeignKey(d => d.MaDon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ThanhToan_DonThue");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro).HasName("PK__VaiTro__C24C41CFA44406A8");

            entity.ToTable("VaiTro");

            entity.Property(e => e.MaVaiTro)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenVaiTro).HasMaxLength(50);
        });

        modelBuilder.Entity<Xe>(entity =>
        {
            entity.HasKey(e => e.MaXe).HasName("PK__Xe__272520CD79878C58");

            entity.ToTable("Xe");

            entity.Property(e => e.MaXe)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.HinhAnh).HasMaxLength(255);
            entity.Property(e => e.LoaiXeMay).HasMaxLength(50);
            entity.Property(e => e.MauSac).HasMaxLength(50);
            entity.Property(e => e.TenHang).HasMaxLength(50);
            entity.Property(e => e.TenXe).HasMaxLength(100);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Sẵn sàng");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
