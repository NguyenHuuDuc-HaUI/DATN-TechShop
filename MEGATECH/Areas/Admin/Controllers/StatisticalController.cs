using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MEGATECH.App_Start;
using MEGATECH.Models;
using MEGATECH.Models.EF;
using PagedList;

namespace MEGATECH.Areas.Admin.Controllers
{
    public class StatisticalController : Controller
    {
        private MEGATECHDBContext db = new MEGATECHDBContext();
        // GET: Admin/Statistical
        [AdminAuthorize(idChucNang = "MEGATECH_ThongKe")]
        public ActionResult Index()
        {
            return View();
        }
        /////////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_ThongKe")]
        [HttpGet]
        public ActionResult GetStatistical(string fromDay, string toDay)
        {
            var query = from o in db.hoaDons
                        join od in db.chiTietHoaDons
                        on o.MaHoaDon equals od.OrderID
                        join p in db.products
                        on od.ProductID equals p.MaSanPham
                        where o.TrangThai == 2
                        select new
                        {
                            CreatedDate = o.CreatedDate,
                            Quantity = od.SoLuong,
                            Price = od.GiaBan,
                            OriginalPrice = p.GiaNhap,
                        };
            if (!string.IsNullOrEmpty(fromDay))
            {
                DateTime startDate = DateTime.ParseExact(fromDay, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate >= startDate);
            }
            if (!string.IsNullOrEmpty(toDay))
            {
                DateTime endDate = DateTime.ParseExact(toDay, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate < endDate);
            }

            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate)).Select(x => new
            {
                Date = x.Key.Value,
                TotalBuy = x.Sum(y => y.Quantity * y.OriginalPrice),
                TotalSell = x.Sum(y => y.Quantity * y.Price),
            }).Select(x => new
            {
                Date = x.Date,
                DoanhThu = x.TotalSell,
                LoiNhuan = x.TotalSell - x.TotalBuy
            });
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }
        /////////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_ThongKe")]
        [HttpGet]
        public ActionResult GetProductSales(string fromDay, string toDay)
        {
            var query = from o in db.hoaDons
                        join od in db.chiTietHoaDons
                        on o.MaHoaDon equals od.OrderID
                        where o.TrangThai == 2
                        select new
                        {
                            CreatedDate = o.CreatedDate,
                            Quantity = od.SoLuong,
                            ProductName = od.Product.Title
                        };
            if (!string.IsNullOrEmpty(fromDay))
            {
                DateTime startDate = DateTime.ParseExact(fromDay, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate >= startDate);
            }
            if (!string.IsNullOrEmpty(toDay))
            {
                DateTime endDate = DateTime.ParseExact(toDay, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate < endDate);
            }

            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate)).Select(x => new
            {
                Date = x.Key.Value,
                TotalProducts = x.Sum(y => y.Quantity),
                Products = x.GroupBy(y => y.ProductName).Select(y => new
                {
                    ProductName = y.Key,
                    Quantity = y.Sum(z => z.Quantity)
                })
                .OrderByDescending(y => y.Quantity) // Sắp xếp theo số lượng giảm dần
                .Take(5),
                BestSellingProduct = x.GroupBy(y => y.ProductName)
                              .OrderByDescending(y => y.Sum(z => z.Quantity))
                              .ThenBy(y => y.Key) // Sắp xếp thứ tự sản phẩm để kiểm tra số lượng bằng nhau
                              .Select(y => new
                              {
                                  ProductName = y.Key,
                                  Quantity = y.Sum(z => z.Quantity)
                              })
                              .ToList() // Chuyển đổi thành danh sách để kiểm tra số lượng
            }).ToList() // Chuyển đổi thành danh sách để kiểm tra số lượng bên ngoài
            .Select(x => new
            {
                x.Date,
                x.TotalProducts,
                x.Products,
                BestSellingProduct = (x.BestSellingProduct.Count > 1 && x.BestSellingProduct[0].Quantity == x.BestSellingProduct[1].Quantity)
                                      ? "Chưa xác định"
                                      : x.BestSellingProduct[0].ProductName
            });

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }
        /////////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_ThongKe")]
        [HttpGet]
        public ActionResult ProductSalesDetail(string date)
        {
            DateTime selectedDate = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var productSales = db.chiTietHoaDons
                .Where(x => DbFunctions.TruncateTime(x.HoaDon.CreatedDate) == selectedDate.Date && x.HoaDon.TrangThai == 2)
                .GroupBy(x => x.Product.Title) // GroupBy theo tên sản phẩm
                .Select(group => new ProductSalesViewModel
                {
                    ProductID = group.FirstOrDefault().ProductID,
                    ProductName = group.Key, // Key ở đây là tên sản phẩm
                    Quantity = group.Sum(x => x.SoLuong), // Tổng số lượng bán của sản phẩm
                    UnitPrice = group.FirstOrDefault().GiaBan, // Giá bán của sản phẩm (lấy giá bán của bất kỳ sản phẩm nào trong nhóm)
                    GiaNhap = group.FirstOrDefault().Product.GiaNhap
                }).ToList();

            ViewBag.Date = date;
            return View(productSales);
        }
        /////////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_ThongKe")]
        [HttpGet]
        public ActionResult GetPaymentMethodsStatistical(string fromDay, string toDay)
        {
            var query = from o in db.hoaDons
                        select new
                        {
                            CreatedDate = o.CreatedDate,
                            PaymentMethod = o.PhuongThucThanhToan
                        };

            if (!string.IsNullOrEmpty(fromDay))
            {
                DateTime startDate = DateTime.ParseExact(fromDay, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate >= startDate);
            }
            if (!string.IsNullOrEmpty(toDay))
            {
                DateTime endDate = DateTime.ParseExact(toDay, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate < endDate);
            }

            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate))
                              .Select(g => new
                              {
                                  Date = g.Key.Value,
                                  OnlineCount = g.Count(x => x.PaymentMethod == 1 || x.PaymentMethod == 2),
                                  OfflineCount = g.Count(x => x.PaymentMethod == 3)
                              })
                              .ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }
        /////////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_ThongKe")]
        public ActionResult PaymentMethodDetail(string date)
        {
            DateTime selectedDate = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var invoices = db.hoaDons
                .Where(x => DbFunctions.TruncateTime(x.CreatedDate) == selectedDate.Date && x.TrangThai == 2)
                .Select(x => new InvoiceViewModel
                {
                    MaHoaDon = x.MaHoaDon,
                    CreatedDate = x.CreatedDate,
                    ID_KhachHang = x.ID_KhachHang,
                    TenKhachHang = x.TenKhachHang,
                    PhuongThucThanhToan = x.PhuongThucThanhToan,
                    TrangThai = x.TrangThai,
                    TongHoaDon = x.ChiTietHoaDons.Sum(ct => (decimal?)ct.SoLuong * ct.GiaBan) ?? 0m
                })
                .ToList();
            var onlineInvoices = invoices.Where(x => x.PhuongThucThanhToan == 1 || x.PhuongThucThanhToan == 2).ToList();
            var offlineInvoices = invoices.Where(x => x.PhuongThucThanhToan == 3).ToList();
            ViewBag.Date = date;
            ViewBag.OnlineInvoices = onlineInvoices;
            ViewBag.OfflineInvoices = offlineInvoices;
            return View();
        }

    }
}