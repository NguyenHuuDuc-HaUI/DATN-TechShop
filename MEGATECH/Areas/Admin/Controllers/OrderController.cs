using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MEGATECH.Models;
using PagedList;
using System.Globalization;
using System.Data.Entity;
using MEGATECH.Models.ViewModels;
using MEGATECH.Models.EF;
using MEGATECH.App_Start;

namespace MEGATECH.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {

        private MEGATECHDBContext db = new MEGATECHDBContext();
        // GET: Admin/Order
        [AdminAuthorize(idChucNang = "MEGATECH_XemDanhSach")]
        public ActionResult Index(string Searchtext)
        {
            var items = db.hoaDons.OrderByDescending(x => x.CreatedDate).ToList();
            if (!string.IsNullOrEmpty(Searchtext))
            {
                // Chuyển đổi Searchtext thành DateTime
                DateTime searchDate;
                bool isDate = DateTime.TryParseExact(Searchtext, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out searchDate);

                items = items
                    .Where(c => c.MaHoaDon.Contains(Searchtext) ||
                                c.TenKhachHang.Contains(Searchtext) ||
                                (isDate && c.CreatedDate.Date == searchDate.Date) || // So sánh ngày
                                c.CCCD.Contains(Searchtext))
                    .ToList();
            }
            return View(items);
        }
        [AdminAuthorize(idChucNang = "MEGATECH_XemChiTiet")]
        public ActionResult Detail(string id)
        {
            var item = db.hoaDons.Find(id);
            return View(item);
        }
        [AdminAuthorize(idChucNang = "MEGATECH_XemChiTiet")]
        public ActionResult Partial_SanPham(string id)
        {
            var items = db.chiTietHoaDons.Where(x => x.OrderID == id).ToList();
            return PartialView(items);
        }

        [AdminAuthorize(idChucNang = "MEGATECH_ChinhSua", chucVu = "Quản lý, Thu ngân")]
        [HttpPost]
        public ActionResult UpdateTT(string id, int trangthai)
        {
            var item = db.hoaDons.Find(id);
            if (item != null)
            {
                db.hoaDons.Attach(item);
                item.TrangThai = trangthai;
                db.Entry(item).Property(x => x.TrangThai).IsModified = true;
                if(trangthai == 2)
                {
                    var orderDetails = db.chiTietHoaDons.Where(od => od.OrderID == item.MaHoaDon).ToList();
                    foreach (var orderDetail in orderDetails)
                    {
                        var product = db.products.Find(orderDetail.ProductID);
                        if (product != null)
                        {
                            product.SoLuong -= orderDetail.SoLuong; // Giảm số lượng sản phẩm trong kho
                            db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
                return Json(new { message = "Success", Success = true });
            }
            return Json(new { message = "Unsuccess", Success = false });
        }
        [AdminAuthorize(idChucNang = "MEGATECH_XemDanhSach")]
        public ActionResult KhachHang(string Searchtext)
        {
            var customers = db.hoaDons
                .GroupBy(h => new { h.ID_KhachHang, h.TenKhachHang, h.CCCD })
                .Select(g => new CustomerViewModel
                {
                    MaKhachHang = g.Key.ID_KhachHang,
                    TenKhachHang = g.Key.TenKhachHang,
                    CCCD = g.Key.CCCD,
                    Email = g.OrderByDescending(h => h.CreatedDate).FirstOrDefault().Email,
                    SoDienThoai = g.OrderByDescending(h => h.CreatedDate).FirstOrDefault().SoDienThoai
                })
                .ToList();

            if (!string.IsNullOrEmpty(Searchtext))
            {
                customers = customers
                    .Where(c => c.MaKhachHang.Contains(Searchtext) ||
                                c.TenKhachHang.Contains(Searchtext) ||
                                c.CCCD.Contains(Searchtext))
                    .ToList();
            }

            return View(customers);
        }
        [AdminAuthorize(idChucNang = "MEGATECH_XemChiTiet")]
        public ActionResult ChiTietKhachHang(string id)
        {
            var customer = db.hoaDons
                .Where(h => h.ID_KhachHang == id)
                .Select(h => new CustomerDetailsViewModel
                {
                    MaKhachHang = h.ID_KhachHang,
                    TenKhachHang = h.TenKhachHang,
                    CCCD = h.CCCD,
                    HoaDons = db.hoaDons
                    .Where(d => d.ID_KhachHang == id)
                    .Select(d => new HoaDonViewModel
                    {
                        MaHoaDon = d.MaHoaDon,
                        ThoiGianLap = d.CreatedDate,
                        SoDienThoai = d.SoDienThoai,
                        Email = d.Email,
                        DiaChi = d.DiaChi,
                        Status = d.TrangThai
                    }).ToList()
                }).FirstOrDefault();

            return View(customer);
        }
    }
}