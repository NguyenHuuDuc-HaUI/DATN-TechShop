using MEGATECH.App_Start;
using MEGATECH.Models;
using MEGATECH.Models.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MEGATECH.Areas.Admin.Controllers
{
    public class ChucVuController : Controller
    {
        private MEGATECHDBContext db = new MEGATECHDBContext();
        private List<ChucVu> getList(string viewName, List<ChucVu> filteredList)
        {
            if (filteredList != null && filteredList.Any())
            {
                // Sử dụng danh sách đã lọc nếu có
                return filteredList;
            }

            // Nếu không có danh sách đã lọc, trả về danh sách mặc định
            return db.chucVus.OrderBy(x => x.TenChucVu).ToList();
        }
        //Hiển thị danh sách toàn bộ: SELCT * FROM
        public List<ChucVu> getList(string status = "All")
        {
            List<ChucVu> list = null;
            switch (status)
            {
                case "Index":
                    {
                        list = db.chucVus
                            .Where(m => m.Status != 0)
                            .ToList();
                        break;
                    }
                case "Trash":
                    {
                        list = db.chucVus
                            .Where(m => m.Status == 0)
                            .ToList();
                        break;
                    }
                default:
                    {
                        list = db.chucVus.OrderBy(x => x.TenChucVu).ToList();
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
            var items = db.chucVus.AsQueryable();

            // Kiểm tra nếu có chuỗi tìm kiếm
            if (!string.IsNullOrEmpty(Searchtext))
            {
                // Lọc danh sách theo các tiêu chí tìm kiếm
                items = items.Where(x =>
                    x.TenChucVu.Contains(Searchtext)
                );
            }

            // Chỉ lấy những nhân viên có status != 0
            items = items.Where(x => x.Status != 0);

            // Chuyển danh sách đã lọc sang getList để trả về kết quả
            var filteredList = items.OrderByDescending(x => x.ID).ToList(); // Chuyển đổi IQueryable thành List
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
        public ActionResult Create(ChucVu model)
        {
            if (ModelState.IsValid)
            {
                var existCV = db.chucVus.FirstOrDefault(x => x.TenChucVu == model.TenChucVu);
                if (existCV != null)
                {
                    ModelState.AddModelError("TenChucVu", "Tên chức vụ đã tồn tại.");
                    // Trả về View với model để hiển thị lỗi
                    return View(model);
                }
                model.CreatedBy = "MEGATECH Administrator";
                model.CreatedDate = DateTime.Now;
                model.Status = 1;
                db.chucVus.Add(model);
                db.SaveChanges();
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Thêm mới chức vụ " + model.TenChucVu + " thành công");
                return RedirectToAction("Index");
            }
            return View();
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_ChinhSua", chucVu = "Quản lý")]
        public ActionResult Edit(int? id)
        {
            var item = db.chucVus.Find(id);
            return View(item);
        }
        [AdminAuthorize(idChucNang = "MEGATECH_ChinhSua", chucVu = "Quản lý")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ChucVu model)
        {
            if (ModelState.IsValid)
            {
                // Cập nhật thông tin của tài khoản
                model.Modifiedby = "MEGATECH Administrator";
                model.ModifiedDate = DateTime.Now;
                db.chucVus.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Chỉnh sửa thông tin chức vụ " + model.TenChucVu + " thành công");
                return RedirectToAction("Index");
            }
            return View();
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_XemChiTiet", chucVu = "Quản lý")]
        public ActionResult Detail(int id)
        {
            var item = db.chucVus.Find(id);
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
        public ActionResult GoToTrash(int? id)
        {
            if (id == null)
            {
                //Thông báo thất bại
                TempData["message"] = new XMessage("danger", "Đưa chức vụ " + id + " vào thùng rác thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }

            var item = db.chucVus.Find(id);
            if (item == null)
            {
                //Thông báo thất bại
                TempData["message"] = new XMessage("danger", "Đưa chức vụ " + item.TenChucVu + " vào thùng rác thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }
            item.Status = 0;
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            //Hiển thị thông báo thành công
            TempData["message"] = new XMessage("success", "Đưa thành công chức vụ " + item.TenChucVu + " vào thùng rác");
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
                        var obj = db.chucVus.Find(Convert.ToInt32(item));
                        if (obj != null)
                        {
                            obj.Status = 0;
                            db.Entry(obj).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                }
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Đưa thành công các chức vụ đã lựa chọn vào thùng rác");
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_Xoa", chucVu = "Quản lý")]
        public ActionResult Delete(int? id)
        {
            var item = db.chucVus.Find(id);
            if (item == null)
            {
                //Hiển thị thông báo
                TempData["message"] = new XMessage("danger", "Không tìm thấy chức vụ có id " + item.ID);
                // Nếu không tìm thấy Menu, trả về lỗi
                return RedirectToAction("Index");
            }
            db.chucVus.Remove(item);
            db.SaveChanges();
            //Hiển thị thông báo
            TempData["message"] = new XMessage("success", "Xoá thành công chức vụ " + item.TenChucVu);
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
                        var obj = db.chucVus.Find(Convert.ToInt32(item));
                        db.chucVus.Remove(obj);
                    }
                    db.SaveChanges();
                }
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Xoá thành công các chức vụ đã lựa chọn");
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
                TempData["message"] = new XMessage("danger", "Phục hồi chức vụ thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }
            var item = db.chucVus.Find(id);
            if (item == null)
            {
                //Thông báo thất bại
                TempData["message"] = new XMessage("danger", "Phục hồi chức vụ thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }
            item.Status = 1;
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            //Hiển thị thông báo thành công
            TempData["message"] = new XMessage("success", "Phục hồi thành công chức vụ " + item.TenChucVu);
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
                        var obj = db.chucVus.Find(Convert.ToInt32(item));
                        if (obj != null)
                        {
                            obj.Status = 1;
                            db.Entry(obj).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                }
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Phục hồi thành công các chức vụ đã lựa chọn");
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_Xoa", chucVu = "Quản lý")]
        public ActionResult PhanQuyen(int id)
        {
            var item = db.chucVus.Find(id);
            return View(item);
        }
        [AdminAuthorize(idChucNang = "MEGATECH_Xoa", chucVu = "Quản lý")]
        [HttpPost]
        public ActionResult PhanQuyen(int chucVu, string maChucNang)
        {
            var phanQuyen = db.phanQuyens.SingleOrDefault(x => x.IDChucVu == chucVu && x.MaChucNang == maChucNang);
            if (phanQuyen != null)
            {
                //Xoá phân quyền
                db.phanQuyens.Remove(phanQuyen);
                db.SaveChanges();
                return Json(new { success = false });
            }
            phanQuyen = new PhanQuyen();
            phanQuyen.IDChucVu = chucVu;
            phanQuyen.MaChucNang = maChucNang;
            db.phanQuyens.Add(phanQuyen);
            db.SaveChanges();
            return Json(new { success = true });
        }
    }
}