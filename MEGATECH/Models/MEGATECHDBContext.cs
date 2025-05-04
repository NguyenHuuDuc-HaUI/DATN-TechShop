using MEGATECH.Models.EF;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace MEGATECH.Models
{
    public partial class MEGATECHDBContext : DbContext
    {
        public MEGATECHDBContext()
            : base("name=MEGATECHDBContext")
        {
            ((IObjectContextAdapter)this).ObjectContext.SavingChanges += MEGATECHDBContext_SavingChanges;
        }

        public DbSet<Category> categories { get; set; }
        public DbSet<ChucVu> chucVus { get; set; }
        public DbSet<NhanVien> nhanViens { get; set; }
        public DbSet<ChucNangQuyen> chucNangQuyens { get; set; }
        public DbSet<PhanQuyen> phanQuyens { get; set; }
        public DbSet<NhaCungCap> nhaCungCaps { get; set; }
        public DbSet<ProductCategory> productCategories { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<HoaDon> hoaDons { get; set; } 
        public DbSet<ChiTietHoaDon> chiTietHoaDons { get; set; }
        public DbSet<ThongKe> thongKes { get; set; }




        // Sự kiện xử lý trước khi lưu thay đổi vào cơ sở dữ liệu
        private void MEGATECHDBContext_SavingChanges(object sender, EventArgs e)
        {
            var dbContext = sender as MEGATECHDBContext;
            if (dbContext != null)
            {
                // Lấy tất cả các thay đổi trong DbContext
                var entries = dbContext.ChangeTracker.Entries().ToList();

                foreach (var entry in entries)
                {
                    // Kiểm tra xem thay đổi có liên quan đến hoá đơn hay không
                    if (entry.Entity is HoaDon)
                    {
                        var hoaDon = entry.Entity as HoaDon;
                        // Kiểm tra nếu trạng thái là đã thanh toán (2)
                        if (hoaDon.TrangThai == 2 && entry.State == EntityState.Modified)
                        {
                            // Lấy tất cả các chi tiết hoá đơn của hoá đơn này
                            var orderDetails = dbContext.chiTietHoaDons.Where(od => od.OrderID == hoaDon.MaHoaDon).ToList();
                            foreach (var orderDetail in orderDetails)
                            {
                                // Tìm sản phẩm tương ứng trong kho
                                var product = dbContext.products.Find(orderDetail.ProductID);
                                if (product != null)
                                {
                                    // Giảm số lượng sản phẩm trong kho
                                    product.SoLuong -= orderDetail.SoLuong;
                                    dbContext.Entry(product).State = EntityState.Modified;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
