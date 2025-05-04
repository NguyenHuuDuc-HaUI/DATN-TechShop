using MEGATECH.App_Start;
using MEGATECH.Models;
using MEGATECH.Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MEGATECH.Areas.Admin.Controllers
{
    public class SupplierController : Controller
    {
        private MEGATECHDBContext db = new MEGATECHDBContext();

        private List<NhaCungCap> getList(string viewName, List<NhaCungCap> filteredList)
        {
            if (filteredList != null && filteredList.Any())
            {
                // Sử dụng danh sách đã lọc nếu có
                return filteredList;
            }

            // Nếu không có danh sách đã lọc, trả về danh sách mặc định
            return db.nhaCungCaps.OrderBy(x => x.Title).ToList();
        }
        //Hiển thị danh sách toàn bộ: SELCT * FROM
        public List<NhaCungCap> getList(string status = "All")
        {
            List<NhaCungCap> list = null;
            switch (status)
            {
                case "Index":
                    {
                        list = db.nhaCungCaps
                            .Where(m => m.Status != 0)
                            .ToList();
                        break;
                    }
                case "Trash":
                    {
                        list = db.nhaCungCaps
                            .Where(m => m.Status == 0)
                            .ToList();
                        break;
                    }
                default:
                    {
                        list = db.nhaCungCaps.OrderBy(x => x.Title).ToList();
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
            var items = db.nhaCungCaps.AsQueryable();

            // Kiểm tra nếu có chuỗi tìm kiếm
            if (!string.IsNullOrEmpty(Searchtext))
            {
                // Lọc danh sách theo các tiêu chí tìm kiếm
                items = items.Where(x =>
                    x.MaNhaCungCap.Contains(Searchtext) ||
                    x.Title.Contains(Searchtext) ||
                    x.SoDienThoai.Contains(Searchtext) ||
                    x.Email.Contains(Searchtext)
                );
            }
            // Chỉ lấy những nhân viên có status != 0
            items = items.Where(x => x.Status != 0);
            // Chuyển danh sách đã lọc sang getList để trả về kết quả
            var filteredList = items.OrderByDescending(x => x.MaNhaCungCap).ToList(); // Chuyển đổi IQueryable thành List
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
        public ActionResult Create(NhaCungCap model)
        {
            if (ModelState.IsValid)
            {
                var exist = db.nhaCungCaps.FirstOrDefault(x => x.Title == model.Title);
                if (exist != null)
                {
                    ModelState.AddModelError("Title", "Tên nhà cung cấp đã tồn tại.");
                    // Trả về View với model để hiển thị lỗi
                    return View(model);
                }
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();
                var mancc = new string(Enumerable.Repeat(chars, 8)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                model.MaNhaCungCap = mancc;
                model.CreatedBy = "MEGATECH Administrator";
                model.CreatedDate = DateTime.Now;
                model.Alias = MEGATECH.Models.Common.Filter.FilterChar(model.Title);
                model.Status = 1;
                db.nhaCungCaps.Add(model);
                db.SaveChanges();
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Thêm mới nhà cung cấp  " + model.Title + " thành công");
                return RedirectToAction("Index");
            }
            return View();
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_ChinhSua", chucVu = "Quản lý")]
        public ActionResult Edit(string id)
        {
            var item = db.nhaCungCaps.Find(id);
            return View(item);
        }
        [AdminAuthorize(idChucNang = "MEGATECH_ChinhSua", chucVu = "Quản lý")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NhaCungCap model)
        {
            if (ModelState.IsValid)
            {
                // Cập nhật thông tin của tài khoản
                model.Modifiedby = "MEGATECH Administrator";
                model.ModifiedDate = DateTime.Now;
                model.Alias = MEGATECH.Models.Common.Filter.FilterChar(model.Title);
                db.nhaCungCaps.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Chỉnh sửa thông tin nhà cung cấp " + model.Title + " thành công");
                return RedirectToAction("Index");
            }
            return View();
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_XemChiTiet", chucVu = "Quản lý")]
        public ActionResult Detail(string id)
        {
            var item = db.nhaCungCaps.Find(id);
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
                TempData["message"] = new XMessage("danger", "Đưa nhà cung cấp có id " + id + " vào thùng rác thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }

            var item = db.nhaCungCaps.Find(id);
            if (item == null)
            {
                //Thông báo thất bại
                TempData["message"] = new XMessage("danger", "Đưa nhà cung cấp " + item.Title + " vào thùng rác thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }
            item.Status = 0;
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            //Hiển thị thông báo thành công
            TempData["message"] = new XMessage("success", "Đưa thành công nhà cung cấp " + item.Title + " vào thùng rác");
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
                        var obj = db.nhaCungCaps.Find(item);
                        if (obj != null)
                        {
                            obj.Status = 0;
                            db.Entry(obj).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                }
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Đưa thành công các nhà cung cấp đã lựa chọn vào thùng rác");
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_Xoa", chucVu = "Quản lý")]
        public ActionResult Delete(string id)
        {
            var item = db.nhaCungCaps.Find(id);
            if (item == null)
            {
                //Hiển thị thông báo
                TempData["message"] = new XMessage("danger", "Không tìm thấy nhà cung cấp có id " + item.MaNhaCungCap);
                // Nếu không tìm thấy Menu, trả về lỗi
                return RedirectToAction("Index");
            }
            db.nhaCungCaps.Remove(item);
            db.SaveChanges();
            //Hiển thị thông báo
            TempData["message"] = new XMessage("success", "Xoá thành công nhà cung cấp " + item.Title);
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
                        var obj = db.nhaCungCaps.Find(item);
                        db.nhaCungCaps.Remove(obj);
                    }
                    db.SaveChanges();
                }
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Xoá thành công các nhà cung cấp đã lựa chọn");
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
                TempData["message"] = new XMessage("danger", "Phục hồi nhà cung cấp thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }
            var item = db.nhaCungCaps.Find(id);
            if (item == null)
            {
                //Thông báo thất bại
                TempData["message"] = new XMessage("danger", "Phục hồi nhà cung cấp thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }
            item.Status = 1;
            item.Modifiedby = "MEGATECH Administrator";
            item.ModifiedDate = DateTime.Now;
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            //Hiển thị thông báo thành công
            TempData["message"] = new XMessage("success", "Phục hồi thành công nhà cung cấp " + item.Title);
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
                        var obj = db.nhaCungCaps.Find(item);
                        if (obj != null)
                        {
                            obj.Status = 1;
                            obj.Modifiedby = "MEGATECH Administrator";
                            obj.ModifiedDate = DateTime.Now;
                            db.Entry(obj).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                }
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Phục hồi thành công các nhà cung cấp đã lựa chọn");
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}