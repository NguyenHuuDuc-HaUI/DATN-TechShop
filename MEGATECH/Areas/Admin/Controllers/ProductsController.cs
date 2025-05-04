using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MEGATECH.Models;
using MEGATECH.Models.EF;
using MEGATECH.App_Start;
using System.Data.Entity;
using System.Web.WebPages;

namespace MEGATECH.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private MEGATECHDBContext db = new MEGATECHDBContext();

        // GET: Admin/Products
        private List<Product> getList(string viewName, List<Product> filteredList)
        {
            if (filteredList != null && filteredList.Any())
            {
                // Sắp xếp danh sách đã lọc theo tên loại sản phẩm
                return filteredList.OrderBy(x => x.NhaCungCap.Title).ToList();
            }

            // Nếu không có danh sách đã lọc, trả về danh sách mặc định sắp xếp theo tên loại sản phẩm
            return db.products.OrderBy(x => x.NhaCungCap.Title).ToList();
        }


        //Hiển thị danh sách toàn bộ: SELCT * FROM
        public List<Product> getList(string status = "All")
        {
            List<Product> list = null;
            switch (status)
            {
                case "Index":
                    {
                        list = db.products
                            .Where(m => m.Status != 0)
                            .ToList();
                        break;
                    }
                case "Trash":
                    {
                        list = db.products
                            .Where(m => m.Status == 0)
                            .ToList();
                        break;
                    }
                default:
                    {
                        list = db.products.OrderBy(x => x.ProductCategory.Title).ToList();
                        break;
                    }
            }
            return list;
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_XemDanhSach")]
        public ActionResult Index(string Searchtext)
        {
            // Khởi tạo danh sách sản phẩm và sắp xếp theo ID giảm dần
            var items = db.products.Include(p => p.ProductCategory).Include(p => p.NhaCungCap).AsQueryable();

            // Kiểm tra nếu có chuỗi tìm kiếm
            if (!string.IsNullOrEmpty(Searchtext))
            {
                // Lọc danh sách theo các tiêu chí tìm kiếm
                items = items.Where(x =>
                    x.MaSanPham.Contains(Searchtext) ||
                    x.Title.Contains(Searchtext) ||
                    x.ProductCategory.Title.Contains(Searchtext) ||
                    x.NhaCungCap.Title.Contains(Searchtext)
                );
            }
            // Chỉ lấy những nhân viên có status != 0
            items = items.Where(x => x.Status != 0);
            // Chuyển danh sách đã lọc sang getList để trả về kết quả
            var filteredList = items.OrderBy(x => x.ProductCategory.Title).ToList(); // Chuyển đổi IQueryable thành List và sắp xếp theo tên loại sản phẩm
            var resultList = getList("Index", filteredList);

            return View(resultList);
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_ThemMoi", chucVu = "Quản lý")]
        public ActionResult Create()
        {
            ViewBag.IdProductCategory = new SelectList(db.productCategories.ToList(), "MaLoaiSanPham", "Title");
            ViewBag.IdSupplier = new SelectList(db.nhaCungCaps.ToList(), "MaNhaCungCap", "Title");
            return View();
        }
        [AdminAuthorize(idChucNang = "MEGATECH_ThemMoi", chucVu = "Quản lý")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product model)
        {
            if (ModelState.IsValid)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();
                var masp = new string(Enumerable.Repeat(chars, 8)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                model.MaSanPham = masp;
                model.CreatedBy = "MEGATECH Administrator";
                model.CreatedDate = DateTime.Now;
                model.Alias = MEGATECH.Models.Common.Filter.FilterChar(model.Title);
                model.IsActive = true;
                model.Status = 1;

                // Tính tỉ lệ giảm giá tối đa mà không bị lỗ và rút gọn còn 2 số thập phân
                decimal maxGiamGia = Math.Round((model.GiaNiemYet - model.GiaNhap) / model.GiaNiemYet * 100, 2);
                ViewBag.MaxGiamGia = maxGiamGia;

                if (string.IsNullOrEmpty(model.ProductCategoryID))
                {
                    ModelState.AddModelError("ProductCategoryID", "Loại sản phẩm cần phải lựa chọn");
                    // Trả về View với model để hiển thị lỗi
                    ViewBag.IdSupplier = new SelectList(db.nhaCungCaps.ToList(), "MaNhaCungCap", "Title");
                    ViewBag.IdProductCategory = new SelectList(db.productCategories.ToList(), "MaLoaiSanPham", "Title");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.SupplierID))
                {
                    ModelState.AddModelError("SupplierID", "Nhà cung cấp cần phải lựa chọn");
                    // Trả về View với model để hiển thị lỗi
                    ViewBag.IdSupplier = new SelectList(db.nhaCungCaps.ToList(), "MaNhaCungCap", "Title");
                    ViewBag.IdProductCategory = new SelectList(db.productCategories.ToList(), "MaLoaiSanPham", "Title");
                    return View(model);
                }
                var existSP = db.products.FirstOrDefault(x => x.Title == model.Title);
                if (existSP != null)
                {
                    ModelState.AddModelError("Title", "Tên sản phẩm đã tồn tại.");
                    // Trả về View với model để hiển thị lỗi
                    ViewBag.IdSupplier = new SelectList(db.nhaCungCaps.ToList(), "MaNhaCungCap", "Title");
                    ViewBag.IdProductCategory = new SelectList(db.productCategories.ToList(), "MaLoaiSanPham", "Title");
                    return View(model);
                }
                if (model.GiaNiemYet < model.GiaNhap)
                {
                    ModelState.AddModelError("GiaNiemYet", "Giá niêm yết sản phẩm phải lớn hơn giá nhập về.");
                    // Trả về View với model để hiển thị lỗi
                    ViewBag.IdSupplier = new SelectList(db.nhaCungCaps.ToList(), "MaNhaCungCap", "Title");
                    ViewBag.IdProductCategory = new SelectList(db.productCategories.ToList(), "MaLoaiSanPham", "Title");
                    return View(model);
                }

                if (model.GiamGia < 0 || model.GiamGia > 100)
                {
                    ModelState.AddModelError("GiamGia", "Tỉ lệ giá giảm sản phẩm phải từ 0 đến 100%");
                    // Trả về View với model để hiển thị lỗi
                    ViewBag.IdSupplier = new SelectList(db.nhaCungCaps.ToList(), "MaNhaCungCap", "Title");
                    ViewBag.IdProductCategory = new SelectList(db.productCategories.ToList(), "MaLoaiSanPham", "Title");
                    return View(model);
                }

                if(model.GiamGia == null || model.GiamGia < 0)
                {
                    model.IsSale = false;
                    model.GiaBan = (decimal)model.GiaNiemYet;
                }
                else if (model.GiamGia > 0)
                {
                    model.IsSale = true;
                    model.GiaBan = (decimal)(model.GiaNiemYet - (model.GiaNiemYet * (model.GiamGia / 100)));
                }

                if (model.GiaBan < model.GiaNhap)
                {
                    ModelState.AddModelError("GiaBan", "Giá bán sản phẩm phải lớn hơn giá nhập về. Giá bán sản phẩm vừa được tính là: " + @MEGATECH.Common.Common.FormatNumber(model.GiaBan, 0) + " VND");
                    // Trả về View với model để hiển thị lỗi
                    ViewBag.IdSupplier = new SelectList(db.nhaCungCaps.ToList(), "MaNhaCungCap", "Title");
                    ViewBag.IdProductCategory = new SelectList(db.productCategories.ToList(), "MaLoaiSanPham", "Title");
                    return View(model);
                }
                db.products.Add(model);
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Thêm mới sản phẩm " + model.Title + " thành công");
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdSupplier = new SelectList(db.nhaCungCaps.ToList(), "MaNhaCungCap", "Title");
            ViewBag.IdProductCategory = new SelectList(db.productCategories.ToList(), "MaLoaiSanPham", "Title");
            return View(model);
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_XemChiTiet")]
        public ActionResult Detail(string id)
        {
            var item = db.products.Find(id);
            return View(item);
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_ChinhSua", chucVu = "Quản lý")]
        public ActionResult Edit(string id)
        {
            ViewBag.IdSupplier = new SelectList(db.nhaCungCaps.ToList(), "MaNhaCungCap", "Title");
            ViewBag.IdProductCategory = new SelectList(db.productCategories.ToList(), "MaLoaiSanPham", "Title");
            var item = db.products.Find(id);
            return View(item);
        }
        [AdminAuthorize(idChucNang = "MEGATECH_ChinhSua", chucVu = "Quản lý")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product model)
        {
            if (ModelState.IsValid)
            {
                // Cập nhật thông tin của tài khoản
                model.Modifiedby = "MEGATECH Administrator";
                model.ModifiedDate = DateTime.Now;
                model.Alias = MEGATECH.Models.Common.Filter.FilterChar(model.Title);
                // Tính tỉ lệ giảm giá tối đa mà không bị lỗ và rút gọn còn 2 số thập phân
                decimal maxGiamGia = Math.Round((model.GiaNiemYet - model.GiaNhap) / model.GiaNiemYet * 100, 2);
                ViewBag.MaxGiamGia = maxGiamGia;
                if (string.IsNullOrEmpty(model.ProductCategoryID))
                {
                    ModelState.AddModelError("ProductCategoryID", "Loại sản phẩm cần phải lựa chọn");
                    // Trả về View với model để hiển thị lỗi
                    ViewBag.IdSupplier = new SelectList(db.nhaCungCaps.ToList(), "MaNhaCungCap", "Title");
                    ViewBag.IdProductCategory = new SelectList(db.productCategories.ToList(), "MaLoaiSanPham", "Title");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.SupplierID))
                {
                    ModelState.AddModelError("SupplierID", "Nhà cung cấp cần phải lựa chọn");
                    // Trả về View với model để hiển thị lỗi
                    ViewBag.IdSupplier = new SelectList(db.nhaCungCaps.ToList(), "MaNhaCungCap", "Title");
                    ViewBag.IdProductCategory = new SelectList(db.productCategories.ToList(), "MaLoaiSanPham", "Title");
                    return View(model);
                }
                if (model.GiaNiemYet < model.GiaNhap)
                {
                    ModelState.AddModelError("GiaNiemYet", "Giá bán sản phẩm phải lớn hơn giá nhập về.");
                    // Trả về View với model để hiển thị lỗi
                    ViewBag.IdSupplier = new SelectList(db.nhaCungCaps.ToList(), "MaNhaCungCap", "Title");
                    ViewBag.IdProductCategory = new SelectList(db.productCategories.ToList(), "MaLoaiSanPham", "Title");
                    return View(model);
                }

                if (model.GiamGia < 0 || model.GiamGia > 100)
                {
                    ModelState.AddModelError("GiamGia", "Tỉ lệ giá giảm sản phẩm phải từ 0 đến 100%");
                    // Trả về View với model để hiển thị lỗi
                    ViewBag.IdSupplier = new SelectList(db.nhaCungCaps.ToList(), "MaNhaCungCap", "Title");
                    ViewBag.IdProductCategory = new SelectList(db.productCategories.ToList(), "MaLoaiSanPham", "Title");
                    return View(model);
                }
                if (model.GiamGia == null)
                {
                    model.GiamGia = 0;
                }

                if (model.GiamGia > 0)
                {
                    model.IsSale = true;
                    model.GiaBan = (decimal)(model.GiaNiemYet - (model.GiaNiemYet * (model.GiamGia / 100)));
                }
                else
                {
                    model.IsSale = false;
                }

                if (model.GiaBan < model.GiaNhap)
                {
                    ModelState.AddModelError("GiaBan", "Giá bán sản phẩm phải lớn hơn giá nhập về. Giá bán sản phẩm vừa được tính là: " + @MEGATECH.Common.Common.FormatNumber(model.GiaBan, 0) + " VND");
                    // Trả về View với model để hiển thị lỗi
                    ViewBag.IdSupplier = new SelectList(db.nhaCungCaps.ToList(), "MaNhaCungCap", "Title");
                    ViewBag.IdProductCategory = new SelectList(db.productCategories.ToList(), "MaLoaiSanPham", "Title");
                    return View(model);
                }
                db.products.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;

                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Chỉnh sửa thông tin sản phẩm " + model.Title + " thành công");
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdSupplier = new SelectList(db.nhaCungCaps.ToList(), "MaNhaCungCap", "Title");
            ViewBag.IdProductCategory = new SelectList(db.productCategories.ToList(), "MaLoaiSanPham", "Title");
            return View(model);
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_Xoa", chucVu = "Quản lý")]
        public ActionResult Trash()
        {
            return View(getList("Trash"));
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_Xoa", chucVu = "Quản lý")]
        public ActionResult GoToTrash(string id)
        {
            if (id == null)
            {
                //Thông báo thất bại
                TempData["message"] = new XMessage("danger", "Đưa sản phẩm có id " + id + " vào thùng rác thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }

            var item = db.products.Find(id);
            if (item == null)
            {
                //Thông báo thất bại
                TempData["message"] = new XMessage("danger", "Đưa sản phẩm " + item.Title + " vào thùng rác thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }
            item.IsActive = false;
            item.IsHome = false;
            item.IsHot = false;
            item.IsNew = false;
            item.IsSale = false;
            item.Status = 0;
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            //Hiển thị thông báo thành công
            TempData["message"] = new XMessage("success", "Đưa thành công sản phẩm " + item.Title + " vào thùng rác");
            //Trở về trang Index
            return RedirectToAction("Index");
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_Xoa", chucVu = "Quản lý")]
        [HttpPost]
        public ActionResult GoToTrashAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = db.products.Find(item);
                        if (obj != null)
                        {
                            obj.IsActive = false;
                            obj.IsHome = false;
                            obj.IsHot = false;
                            obj.IsNew = false;
                            obj.IsSale = false;
                            obj.Status = 0;
                            db.Entry(obj).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                }
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Đưa thành công các sản phẩm đã lựa chọn vào thùng rác");
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_Xoa", chucVu = "Quản lý")]
        public ActionResult Delete(string id)
        {
            var item = db.products.Find(id);
            if (item == null)
            {
                //Hiển thị thông báo
                TempData["message"] = new XMessage("danger", "Không tìm thấy sản phẩm có id " + item.MaSanPham);
                // Nếu không tìm thấy Menu, trả về lỗi
                return RedirectToAction("Index");
            }
            db.products.Remove(item);
            db.SaveChanges();
            //Hiển thị thông báo
            TempData["message"] = new XMessage("success", "Xoá thành công sản phẩm " + item.Title);
            return Json(new { success = true });
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_Xoa", chucVu = "Quản lý")]
        [HttpPost]
        public ActionResult DeleteAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = db.products.Find(item);
                        db.products.Remove(obj);
                    }
                    db.SaveChanges();
                }
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Xoá thành công các sản phẩm đã lựa chọn");
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        /////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_Xoa", chucVu = "Quản lý")]
        public ActionResult Undo(string id)
        {
            if (id == null)
            {
                //Thông báo thất bại
                TempData["message"] = new XMessage("danger", "Phục hồi sản phẩm thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }
            var item = db.products.Find(id);
            if (item == null)
            {
                //Thông báo thất bại
                TempData["message"] = new XMessage("danger", "Phục hồi sản phẩm thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }
            if(item.GiamGia > 0)
            {
                item.IsSale = true;
            }
            item.IsActive = true;
            item.Status = 1;
            item.Modifiedby = "MEGATECH Administrator";
            item.ModifiedDate = DateTime.Now;
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            //Hiển thị thông báo thành công
            TempData["message"] = new XMessage("success", "Phục hồi thành công sản phẩm " + item.Title);
            //Trở về trang Index
            return RedirectToAction("Trash");
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_Xoa", chucVu = "Quản lý")]
        [HttpPost]
        public ActionResult UndoAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = db.products.Find(item);
                        if (obj != null)
                        {
                            if (obj.GiamGia > 0)
                            {
                                obj.IsSale = true;
                            }
                            obj.IsActive = true;
                            obj.Status = 1;
                            obj.Modifiedby = "MEGATECH Administrator";
                            obj.ModifiedDate = DateTime.Now;
                            db.Entry(obj).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                }
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Phục hồi thành công các sản phẩm đã lựa chọn");
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_Xoa", chucVu = "Quản lý")]
        [HttpPost]
        public ActionResult IsActive(string id)
        {
            var item = db.products.Find(id);
            if (item != null)
            {
                if (item.IsActive == true)
                {
                    item.IsActive = false;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    //Hiển thị thông báo thành công
                    TempData["message"] = new XMessage("success", "Hiển thị sản phẩm " + item.Title + ": KHÔNG HIỂN THỊ");
                }
                else
                {
                    item.IsActive = true;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    //Hiển thị thông báo thành công
                    TempData["message"] = new XMessage("success", "Hiển thị sản phẩm " + item.Title + ": ĐANG HIỂN THỊ");
                }
                return Json(new { success = true, isActive = item.IsActive });
            }
            return Json(new { success = false, isActive = item.IsActive });
        }
    }
}