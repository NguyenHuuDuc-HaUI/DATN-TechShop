using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MEGATECH.Models;
using MEGATECH.Models.EF;

namespace MEGATECH.Controllers
{
    public class ProductsController : Controller
    {
        private MEGATECHDBContext db = new MEGATECHDBContext();
        //GET: Products
        public ActionResult Index()
        {
            var items = db.products.Where(p => p.IsActive).ToList();

            return View(items);
        }

        public ActionResult Detail(string alias, string id)
        {
            var item = db.products.Find(id);
            if (item != null)
            {
                db.products.Attach(item);
                item.ViewCount = item.ViewCount + 1;
                db.Entry(item).Property(x => x.ViewCount).IsModified = true;
                db.SaveChanges();
            }
            //var countReview = db.tb_Review.Where(x => x.ProductId == id).Count();
            //ViewBag.CountReview = countReview;
            return View(item);
        }
        public ActionResult ProductCategory(string alias, string id)
        {
            //var items = db.products.ToList();
            //items = items.Where(x => x.ProductCategoryID == id).ToList();
            var items = db.products.Where(x => x.ProductCategoryID == id && x.IsActive).ToList();
            var cate = db.productCategories.Find(id);
            if (cate != null)
            {
                ViewBag.CateName = cate.Title;
            }

            ViewBag.CateId = id;
            return View(items);
        }

        public ActionResult Supplier(string alias, string id)
        {
            //var items = db.products.ToList();
            //items = items.Where(x => x.SupplierID == id).ToList();
            var items = db.products.Where(x => x.SupplierID == id && x.IsActive).ToList();
            var supp = db.nhaCungCaps.Find(id);
            if (supp != null)
            {
                ViewBag.SupplierName = supp.Title;
            }

            ViewBag.SupplierID = id;
            return View(items);
        }

        public ActionResult Partial_ItemsByCateId()
        {
            var items = db.products.Where(x => x.IsHome && x.IsActive).Take(15).ToList();
            return PartialView(items);
        }

        public ActionResult Partial_ProductSales()
        {
            var items = db.products.Where(x => x.IsSale && x.IsActive).Take(12).ToList();
            return PartialView(items);
        }

        public ActionResult Partial_ProductNew()
        {
            var items = db.products.Where(x => x.IsNew && x.IsActive).Take(12).ToList();
            return PartialView(items);
        }

        public ActionResult Partial_ProductsHot()
        {
            // Lấy tất cả các chi tiết hóa đơn
            var orderDetails = db.chiTietHoaDons.ToList();

            // Tạo một Dictionary để lưu tổng số lượng bán được của mỗi sản phẩm
            Dictionary<string, int> productSales = new Dictionary<string, int>();

            // Duyệt qua từng chi tiết hóa đơn để tính tổng số lượng bán được của mỗi sản phẩm
            foreach (var orderDetail in orderDetails)
            {
                // Kiểm tra xem sản phẩm đã tồn tại trong Dictionary chưa
                if (productSales.ContainsKey(orderDetail.ProductID))
                {
                    // Nếu đã tồn tại, cập nhật tổng số lượng bán được của sản phẩm
                    productSales[orderDetail.ProductID] += orderDetail.SoLuong;
                }
                else
                {
                    // Nếu chưa tồn tại, thêm sản phẩm vào Dictionary với số lượng bán được ban đầu là số lượng trong chi tiết hóa đơn
                    productSales.Add(orderDetail.ProductID, orderDetail.SoLuong);
                }
            }

            // Sắp xếp các sản phẩm theo tổng số lượng bán được (giảm dần)
            var sortedProducts = productSales.OrderByDescending(x => x.Value);

            // Lấy ra 5 sản phẩm bán chạy nhất (có tổng số lượng bán được nhiều nhất)
            var hotProducts = sortedProducts.Take(5).ToList();

            // Lấy ra các ProductID của hotProducts
            var hotProductIDs = hotProducts.Select(hp => hp.Key).ToList();

            // Lấy ra các sản phẩm bán chạy nhất từ CSDL
            var hotProductsDetails = db.products.Where(p => hotProductIDs.Contains(p.MaSanPham)).ToList();


            return PartialView(hotProductsDetails);
        }
    }
}