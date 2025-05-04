using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MEGATECH.App_Start;
using MEGATECH.Models;
using MEGATECH.Models.EF;
using PagedList;

namespace MEGATECH.Areas.Admin.Controllers
{
    public class ProductCategoryController : Controller
    {
        private MEGATECHDBContext db = new MEGATECHDBContext();

        private List<ProductCategory> getList(string viewName, List<ProductCategory> filteredList)
        {
            if (filteredList != null && filteredList.Any())
            {
                // Sử dụng danh sách đã lọc nếu có
                return filteredList;
            }

            // Nếu không có danh sách đã lọc, trả về danh sách mặc định
            return db.productCategories.OrderBy(x => x.Title).ToList();
        }
        //Hiển thị danh sách toàn bộ: SELCT * FROM
        public List<ProductCategory> getList(string status = "All")
        {
            List<ProductCategory> list = null;
            switch (status)
            {
                case "Index":
                    {
                        list = db.productCategories
                            .Where(m => m.Status != 0)
                            .ToList();
                        break;
                    }
                case "Trash":
                    {
                        list = db.productCategories
                            .Where(m => m.Status == 0)
                            .ToList();
                        break;
                    }
                default:
                    {
                        list = db.productCategories.OrderBy(x => x.Title).ToList();
                        break;
                    }
            }
            return list;
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_XemDanhSach", chucVu = "Quản lý")]
        public ActionResult Index(string Searchtext)
        {
            // Khởi tạo danh sách nhân viên và sắp xếp theo ID giảm dần
            var items = db.productCategories.AsQueryable();

            // Kiểm tra nếu có chuỗi tìm kiếm
            if (!string.IsNullOrEmpty(Searchtext))
            {
                // Lọc danh sách theo các tiêu chí tìm kiếm
                items = items.Where(x =>
                    x.Title.Contains(Searchtext)
                );
            }
            // Chỉ lấy những nhân viên có status != 0
            items = items.Where(x => x.Status != 0);
            // Chuyển danh sách đã lọc sang getList để trả về kết quả
            var filteredList = items.OrderByDescending(x => x.MaLoaiSanPham).ToList(); // Chuyển đổi IQueryable thành List
            var resultList = getList("Index", filteredList);

            return View(resultList);
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_ThemMoi", chucVu = "Quản lý")]
        public ActionResult Create()
        {
            return View();
        }
        [AdminAuthorize(idChucNang = "MEGATECH_ThemMoi", chucVu = "Quản lý")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductCategory model)
        {
            if (ModelState.IsValid)
            {
                var exist = db.productCategories.FirstOrDefault(x => x.Title == model.Title);
                if (exist != null)
                {
                    ModelState.AddModelError("Title", "Tên loại sản phẩm đã tồn tại.");
                    // Trả về View với model để hiển thị lỗi
                    return View(model);
                }
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();
                var maloaisp = new string(Enumerable.Repeat(chars, 8)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                model.MaLoaiSanPham = maloaisp;
                model.CreatedBy = "MEGATECH Administrator";
                model.CreatedDate = DateTime.Now;
                model.Alias = MEGATECH.Models.Common.Filter.FilterChar(model.Title);
                model.Status = 1;
                db.productCategories.Add(model);
                db.SaveChanges();
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Thêm mới loại sản phẩm " + model.Title + " thành công");
                return RedirectToAction("Index");
            }
            return View();
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_ChinhSua", chucVu = "Quản lý")]
        public ActionResult Edit(string id)
        {
            var item = db.productCategories.Find(id);
            return View(item);
        }
        [AdminAuthorize(idChucNang = "MEGATECH_ChinhSua", chucVu = "Quản lý")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductCategory model)
        {
            if (ModelState.IsValid)
            {
                // Cập nhật thông tin của tài khoản
                model.Modifiedby = "MEGATECH Administrator";
                model.ModifiedDate = DateTime.Now;
                model.Alias = MEGATECH.Models.Common.Filter.FilterChar(model.Title);
                db.productCategories.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Chỉnh sửa loại sản phẩm " + model.Title + " thành công");
                return RedirectToAction("Index");
            }
            return View();
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_XemChiTiet", chucVu = "Quản lý")]
        public ActionResult Detail(string id)
        {
            var item = db.productCategories.Find(id);
            return View(item);
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
                TempData["message"] = new XMessage("danger", "Đưa loại sản phẩm có id " + id + " vào thùng rác thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }

            var item = db.productCategories.Find(id);
            if (item == null)
            {
                //Thông báo thất bại
                TempData["message"] = new XMessage("danger", "Đưa loại sản phẩm " + item.Title + " vào thùng rác thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }
            item.IsActive = false;
            item.Status = 0;
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            //Hiển thị thông báo thành công
            TempData["message"] = new XMessage("success", "Đưa thành công loại sản phẩm " + item.Title + " vào thùng rác");
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
                        var obj = db.productCategories.Find(item);
                        if (obj != null)
                        {
                            obj.IsActive = false;
                            obj.Status = 0;
                            db.Entry(obj).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                }
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Đưa thành công các loại sản phẩm đã lựa chọn vào thùng rác");
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_Xoa", chucVu = "Quản lý")]
        public ActionResult Delete(string id)
        {
            var item = db.productCategories.Find(id);
            if (item == null)
            {
                //Hiển thị thông báo
                TempData["message"] = new XMessage("danger", "Không tìm thấy loại sản phẩm có id " + item.MaLoaiSanPham);
                // Nếu không tìm thấy Menu, trả về lỗi
                return RedirectToAction("Index");
            }
            db.productCategories.Remove(item);
            db.SaveChanges();
            //Hiển thị thông báo
            TempData["message"] = new XMessage("success", "Xoá thành công loại sản phẩm " + item.Title);
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
                        var obj = db.productCategories.Find(item);
                        db.productCategories.Remove(obj);
                    }
                    db.SaveChanges();
                }
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Xoá thành công các loại sản phẩm đã lựa chọn");
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
                TempData["message"] = new XMessage("danger", "Phục hồi loại sản phẩm thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }
            var item = db.productCategories.Find(id);
            if (item == null)
            {
                //Thông báo thất bại
                TempData["message"] = new XMessage("danger", "Phục hồi loại sản phẩm thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }
            item.IsActive = true;
            item.Status = 1;
            item.Modifiedby = "MEGATECH Administrator";
            item.ModifiedDate = DateTime.Now;
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            //Hiển thị thông báo thành công
            TempData["message"] = new XMessage("success", "Phục hồi thành công loại sản phẩm " + item.Title);
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
                        var obj = db.productCategories.Find(item);
                        if (obj != null)
                        {
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
                TempData["message"] = new XMessage("success", "Phục hồi thành công các loại sản phẩm đã lựa chọn");
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_Xoa", chucVu = "Quản lý")]
        [HttpPost]
        public ActionResult IsActive(string id)
        {
            var item = db.productCategories.Find(id);
            if (item != null)
            {
                if (item.IsActive == true)
                {
                    item.IsActive = false;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    //Hiển thị thông báo thành công
                    TempData["message"] = new XMessage("success", "Hiển thị loại sản phẩm " + item.Title + " : KHÔNG HIỂN THỊ");
                }
                else
                {
                    item.IsActive = true;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    //Hiển thị thông báo thành công
                    TempData["message"] = new XMessage("success", "Hiển thị loại sản phẩm " + item.Title + " : ĐANG HIỂN THỊ");
                }
                return Json(new { success = true, isActive = item.IsActive });
            }
            return Json(new { success = false });
        }
    }
}