using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using MEGATECH.App_Start;
using MEGATECH.Models;
using MEGATECH.Models.EF;
using PagedList;

namespace MEGATECH.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        private MEGATECHDBContext db = new MEGATECHDBContext();

        public List<Category> getList()
        {
            return db.categories.OrderBy(c => c.Position).ToList();
        }

        //Hiển thị danh sách toàn bộ: SELCT * FROM
        public List<Category> getList(string status = "All")
        {
            List<Category> list = null;
            switch (status)
            {
                case "Index":
                    {
                        list = db.categories
                            .Where(m => m.Status != 0)
                            .ToList();
                        break;
                    }
                case "Trash":
                    {
                        list = db.categories
                            .Where(m => m.Status == 0)
                            .ToList();
                        break;
                    }
                default:
                    {
                        list = db.categories.OrderBy(c => c.Position).ToList();
                        break;
                    }
            }
            return list;
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_XemDanhSach", chucVu = "Quản lý")]
        public ActionResult Index()
        {
            return View(getList("Index"));
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_ThemMoi", chucVu = "Quản lý")]
        public ActionResult Create()
        {
            int categoryCount = db.categories.Count();

            // Gán giá trị categoryCount vào ViewBag
            ViewBag.CategoryCount = categoryCount;
            return View();
        }
        [AdminAuthorize(idChucNang = "MEGATECH_ThemMoi", chucVu = "Quản lý")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                var existMenu = db.categories.FirstOrDefault(x => x.Title == category.Title);
                if (existMenu != null)
                {
                    ModelState.AddModelError("Title", "Tên Menu đã tồn tại.");
                    // Trả về View với model để hiển thị lỗi
                    return View(category);
                }
                category.CreatedBy = "MEGATECH Administrator";
                category.CreatedDate = DateTime.Now;
                category.Status = 1;
                category.Alias = MEGATECH.Models.Common.Filter.FilterChar(category.Title);
                var categoriesToUpdate = db.categories.Where(c => c.Position >= category.Position).ToList();

                if (category.Position == 0)
                {
                    category.Position = 1;
                    foreach (var cat in categoriesToUpdate)
                    {
                        cat.Position++;
                    }
                    db.categories.Add(category);
                    db.SaveChanges();
                    //Hiển thị thông báo thành công
                    TempData["message"] = new XMessage("success", "Thêm mới Menu " + category.Title + " thành công");
                    return RedirectToAction("Index");
                }

                if (category?.Position == null)
                {
                    int maxPosition = db.categories.Max(c => (int?)c.Position) ?? 0;
                    category.Position = maxPosition + 1;
                    db.categories.Add(category);
                    db.SaveChanges();
                    //Hiển thị thông báo thành công
                    TempData["message"] = new XMessage("success", "Thêm mới Menu " + category.Title + " thành công");
                    return RedirectToAction("Index");
                }
                else
                {
                    if (category.Position > 0 && category.Position <= db.categories.Count())
                    {
                        foreach (var cat in categoriesToUpdate)
                        {
                            cat.Position++;
                        }
                        db.categories.Add(category);
                        db.SaveChanges();
                        //Hiển thị thông báo thành công
                        TempData["message"] = new XMessage("success", "Thêm mới Menu " + category.Title + " thành công");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = new XMessage("success", "Thêm mới Menu " + category.Title + " thất bại vì vị trí " + category.Position + " không hợp lệ!");
                        return RedirectToAction("Index");
                    }
                }
            }

            int categoryCount = db.categories.Count();

            // Gán giá trị categoryCount vào ViewBag
            ViewBag.CategoryCount = categoryCount;

            return View(category);
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_XemChiTiet", chucVu = "Quản lý")]
        public ActionResult Detail(int id)
        {
            var item = db.categories.Find(id);
            return View(item);
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_ChinhSua", chucVu = "Quản lý")]
        public ActionResult Edit(int? id)
        {
            var item = db.categories.Find(id);
            if (id == null)
            {
                //Hiển thị thông báo
                TempData["message"] = new XMessage("success", "Chỉnh sửa thông tin Menu " + item.Title + " thất bại");
                return RedirectToAction("Index");
            }
            if (item == null)
            {
                TempData["message"] = new XMessage("success", "Chỉnh sửa thông tin Menu " + item.Title + " thất bại");
                return RedirectToAction("Index");
            }
            int categoryCount = db.categories.Count();

            // Gán giá trị categoryCount vào ViewBag
            ViewBag.CategoryCount = categoryCount;
            return View(item);
        }
        [AdminAuthorize(idChucNang = "MEGATECH_ChinhSua", chucVu = "Quản lý")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                var items = db.categories.Find(category.ID);

                if (items != null)
                {
                    // Lưu trữ vị trí ban đầu của danh mục
                    int originalPosition = items.Position ?? 0;
                    // Lưu trữ vị trí mới của danh mục
                    int newPosition = category.Position ?? 0;

                    // Kiểm tra nếu vị trí mới là 0, thì tự động cập nhật vị trí lên 1
                    if (newPosition == 0)
                    {
                        newPosition = 1;
                    }

                    // Nếu vị trí mới khác vị trí ban đầu
                    if (newPosition != originalPosition)
                    {
                        // Cập nhật vị trí của danh mục hiện tại
                        items.Position = newPosition;
                        db.Entry(items).State = EntityState.Modified;

                        // Nếu vị trí mới nhỏ hơn vị trí ban đầu
                        if (newPosition < originalPosition)
                        {
                            // Cập nhật vị trí của các danh mục khác mà có vị trí nằm giữa vị trí mới và vị trí ban đầu
                            var categoriesToUpdate = db.categories.Where(c => c.Position >= newPosition && c.Position < originalPosition && c.ID != category.ID).ToList();
                            foreach (var cat in categoriesToUpdate)
                            {
                                cat.Position++;
                                db.Entry(cat).State = EntityState.Modified;
                            }
                        }
                        else // Nếu vị trí mới lớn hơn vị trí ban đầu
                        {
                            // Cập nhật vị trí của các danh mục khác mà có vị trí nằm giữa vị trí ban đầu và vị trí mới
                            var categoriesToUpdate = db.categories.Where(c => c.Position > originalPosition && c.Position <= newPosition && c.ID != category.ID).ToList();
                            foreach (var cat in categoriesToUpdate)
                            {
                                cat.Position--;
                                db.Entry(cat).State = EntityState.Modified;
                            }
                        }
                    }

                    // Cập nhật các thông tin khác của danh mục
                    items.ModifiedDate = DateTime.Now;
                    items.Alias = MEGATECH.Models.Common.Filter.FilterChar(category.Title);
                    items.Title = category.Title;
                    items.SeoDescription = category.SeoDescription;
                    items.SeoKeywords = category.SeoKeywords;
                    items.SeoTitle = category.SeoTitle;
                    items.IsActive = category.IsActive;
                    items.Modifiedby = category.Modifiedby;
                    // Lưu các thay đổi vào cơ sở dữ liệu
                    db.SaveChanges();

                    //Hiển thị thông báo thành công
                    TempData["message"] = new XMessage("success", "Chỉnh sửa thông tin danh mục " + category.Title + " thành công");
                    return RedirectToAction("Index");
                }
            }

            int categoryCount = db.categories.Count();

            // Gán giá trị categoryCount vào ViewBag
            ViewBag.CategoryCount = categoryCount;

            return View(category);
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_Xoa", chucVu = "Quản lý")]
        public ActionResult Trash()
        {
            return View(getList("Trash"));
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_Xoa", chucVu = "Quản lý")]
        public ActionResult GoToTrash(int? id)
        {
            if (id == null)
            {
                //Thông báo thất bại
                TempData["message"] = new XMessage("danger", "Đưa Menu " + id + " vào thùng rác thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }

            var item = db.categories.Find(id);
            if (item == null)
            {
                //Thông báo thất bại
                TempData["message"] = new XMessage("danger", "Đưa Menu " + item.Title + " vào thùng rác thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }
            item.IsActive = false;
            item.Status = 0;
            item.Modifiedby = "MEGATECH Administrator";
            item.ModifiedDate = DateTime.Now;
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            // Lấy danh sách các Menu còn lại để cập nhật vị trí
            var remainingMenus = db.categories.OrderBy(m => m.Position).Where(x => x.Status == 1).ToList();

            if (remainingMenus.Count > 0)
            {
                // Nếu có Menu còn lại, cập nhật vị trí
                for (int i = 0; i < remainingMenus.Count; i++)
                {
                    remainingMenus[i].Position = i + 1;
                }

                db.SaveChanges();
            }
            //Hiển thị thông báo thành công
            TempData["message"] = new XMessage("success", "Đưa thành công Menu " + item.Title + " vào thùng rác");
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
                        var obj = db.categories.Find(Convert.ToInt32(item));
                        if (obj != null)
                        {
                            obj.IsActive = false;
                            obj.Status = 0;
                            obj.Modifiedby = "MEGATECH Administrator";
                            obj.ModifiedDate = DateTime.Now;
                            db.Entry(obj).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                }
                // Lấy danh sách các Menu còn lại để cập nhật vị trí
                var remainingMenus = db.categories.OrderBy(m => m.Position).Where(x => x.Status == 1).ToList();

                if (remainingMenus.Count > 0)
                {
                    // Nếu có Menu còn lại, cập nhật vị trí
                    for (int i = 0; i < remainingMenus.Count; i++)
                    {
                        remainingMenus[i].Position = i + 1;
                    }

                    db.SaveChanges();
                }
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Đưa thành công các Menu đã lựa chọn vào thùng rác");
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_Xoa", chucVu = "Quản lý")]
        [HttpPost]
        public ActionResult Delete(int? id)
        {
            var item = db.categories.Find(id);
            if (item == null)
            {
                //Hiển thị thông báo
                TempData["message"] = new XMessage("danger", "Không tìm thấy Menu có id " + item.ID);
                // Nếu không tìm thấy Menu, trả về lỗi
                return RedirectToAction("Index");
            }
            // Xoá Menu khỏi cơ sở dữ liệu
            db.categories.Remove(item);
            db.SaveChanges();

            // Lấy danh sách các Menu còn lại để cập nhật vị trí
            var remainingMenus = db.categories.OrderBy(m => m.Position).ToList();

            if (remainingMenus.Count > 0)
            {
                // Nếu có Menu còn lại, cập nhật vị trí
                for (int i = 0; i < remainingMenus.Count; i++)
                {
                    remainingMenus[i].Position = i + 1;
                }

                db.SaveChanges();
            }
            //Hiển thị thông báo
            TempData["message"] = new XMessage("success", "Xoá thành công danh mục Menu " + item.Title);
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
                        var obj = db.categories.Find(Convert.ToInt32(item));
                        db.categories.Remove(obj);
                    }
                    db.SaveChanges();
                    // Cập nhật lại vị trí cho các danh mục còn lại
                    var categories = db.categories.OrderBy(c => c.Position).ToList();
                    int position = 1;
                    foreach (var category in categories)
                    {
                        category.Position = position;
                        position++;
                    }
                    db.SaveChanges();
                }
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Xoá thành công các Menu đã lựa chọn");
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_Xoa", chucVu = "Quản lý")]
        public ActionResult Undo(int? id)
        {
            if (id == null)
            {
                //Thông báo thất bại
                TempData["message"] = new XMessage("danger", "Phục hồi Menu thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }
            var item = db.categories.Find(id);
            if (item == null)
            {
                //Thông báo thất bại
                TempData["message"] = new XMessage("danger", "Phục hồi Menu thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }
            item.IsActive = true;
            item.Status = 1;
            item.Modifiedby = "MEGATECH Administrator";
            item.ModifiedDate = DateTime.Now;
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            // Lấy danh sách các Menu còn lại để cập nhật vị trí
            var remainingMenus = db.categories.OrderBy(m => m.Position).Where(x => x.Status == 1).ToList();

            if (remainingMenus.Count > 0)
            {
                // Nếu có Menu còn lại, cập nhật vị trí
                for (int i = 0; i < remainingMenus.Count; i++)
                {
                    remainingMenus[i].Position = i + 1;
                }

                db.SaveChanges();
            }
            //Hiển thị thông báo thành công
            TempData["message"] = new XMessage("success", "Phục hồi thành công Menu " + item.Title);
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
                        var obj = db.categories.Find(Convert.ToInt32(item));
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
                // Lấy danh sách các Menu còn lại để cập nhật vị trí
                var remainingMenus = db.categories.OrderBy(m => m.Position).Where(x => x.Status == 1).ToList();

                if (remainingMenus.Count > 0)
                {
                    // Nếu có Menu còn lại, cập nhật vị trí
                    for (int i = 0; i < remainingMenus.Count; i++)
                    {
                        remainingMenus[i].Position = i + 1;
                    }

                    db.SaveChanges();
                }
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Phục hồi thành công các nhân viên đã lựa chọn");
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_Xoa", chucVu = "Quản lý")]
        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.categories.Find(id);
            if (item != null)
            {
                if (item.IsActive == true)
                {
                    item.IsActive = false;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    //Hiển thị thông báo thành công
                    TempData["message"] = new XMessage("success", "Hiển thị danh mục Menu " + item.Title + " : KHÔNG HIỂN THỊ");
                }
                else
                {
                    item.IsActive = true;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    //Hiển thị thông báo thành công
                    TempData["message"] = new XMessage("success", "Hiển thị danh mục Menu " + item.Title + " : ĐANG HIỂN THỊ");
                }
                return Json(new { success = true, isActive = item.IsActive });
            }
            return Json(new { success = false });
        }
    }
}