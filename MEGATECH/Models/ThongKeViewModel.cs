using MEGATECH.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MEGATECH.Models
{
    public class ThongKeViewModel
    {
        public int HomNay { get; set; }
        public int HomQua { get; set; }
        public int TuanNay { get; set; }
        public int TuanTruoc { get; set; }
        public int ThangNay { get; set; }
        public int ThangTruoc { get; set; }
        public int TatCa { get; set; }
    }
    public class ThongKeModel
    {
        public string HomNay { get; set; }
        public string HomQua { get; set; }
        public string TuanNay { get; set; }
        public string TuanTruoc { get; set; }
        public string ThangNay { get; set; }
        public string ThangTruoc { get; set; }
        public string TatCa { get; set; }
    }

    public class SanPhamBanChay
    {
        public string MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public int TongSoLuong { get; set; }
        public string TenNhaCungCap { get; set; }
    }
    public class ThongKeSanPham
    {
        public List<SanPhamBanChay> sanPhamBanChays { get; set; }
        public List<Product> sanPhamHetHang { get; set; }
    }

    public class ProductSalesViewModel
    {
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal GiaNhap { get; set; }
    }

    public class InvoiceViewModel
    {
        public string MaHoaDon { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ID_KhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public int PhuongThucThanhToan { get; set; }
        public int TrangThai {  get; set; }
        public decimal TongHoaDon { get; set; }
    }
}