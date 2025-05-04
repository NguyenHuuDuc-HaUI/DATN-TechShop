using MEGATECH.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MEGATECH.Models.EF;
using MEGATECH.App_Start;
using System.Data.Entity;

namespace MEGATECH.Areas.Admin.Controllers
{
    public class NhanVienController : Controller
    {
        private MEGATECHDBContext db = new MEGATECHDBContext();
        private List<NhanVien> getList(string viewName, List<NhanVien> filteredList)
        {
            if (filteredList != null && filteredList.Any())
            {
                // Sử dụng danh sách đã lọc nếu có
                return filteredList;
            }

            // Nếu không có danh sách đã lọc, trả về danh sách mặc định
            return db.nhanViens.OrderByDescending(x => x.ID).ToList();
        }

        //Hiển thị danh sách toàn bộ: SELCT * FROM
        public List<NhanVien> getList(string status = "All")
        {
            List<NhanVien> list = null;
            switch (status)
            {
                case "Index":
                    {
                        list = db.nhanViens
                            .Where(m => m.Status != 0)  // Ensure only active employees are listed
                            .ToList();
                        break;
                    }
                case "Trash":
                    {
                        list = db.nhanViens
                            .Where(m => m.Status == 0)  // Ensure only trashed employees are listed
                            .ToList();
                        break;
                    }
                default:
                    {
                        list = db.nhanViens.OrderBy(c => c.FullName).ToList();
                        break;
                    }
            }
            return list;
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        //Tra cứu danh sách
        [AdminAuthorize(idChucNang = "MEGATECH_XemDanhSach", chucVu = "Quản lý")]
        public ActionResult Index(string Searchtext)
        {
            // Khởi tạo danh sách nhân viên và sắp xếp theo ID giảm dần
            var items = db.nhanViens.AsQueryable();

            // Kiểm tra nếu có chuỗi tìm kiếm
            if (!string.IsNullOrEmpty(Searchtext))
            {
                // Lọc danh sách theo các tiêu chí tìm kiếm
                items = items.Where(x =>
                    x.ID.Contains(Searchtext) ||
                    x.FullName.Contains(Searchtext) ||
                    x.SoDienThoai.Contains(Searchtext) ||
                    x.Email.Contains(Searchtext)
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
            ViewBag.ChucVu = new SelectList(db.chucVus.ToList(), "ID", "TenChucVu");
            var gt = new SelectList(new[]
            {
                new { Value = true, Text = "Nam" },
                new { Value = false, Text = "Nữ" }
            }, "Value", "Text");
                ViewBag.Gender = gt;
                return View();
            }
        [AdminAuthorize(idChucNang = "MEGATECH_ThemMoi", chucVu = "Quản lý")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NhanVien model)
        {
            var gt = new SelectList(new[]
{
                        new { Value = true, Text = "Nam" },
                        new { Value = false, Text = "Nữ" }
                    }, "Value", "Text");
            if (ModelState.IsValid)
            {
                const string chars = "0123456789";
                var random = new Random();
                //Mã nhân viên
                var maNhanVien = new string(Enumerable.Repeat(chars, 8)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                model.ID = maNhanVien;
                /////////////////////////////////////////////////////
                model.IsActiveAccount = true;
                model.CreatedBy = "MEGATECH Administrator";
                model.CreatedDate = DateTime.Now;
                model.Status = 1;
                //////////////////////////////////////////////////////
                // Kiểm tra độ tuổi từ 18 đến 40
                var age = DateTime.Now.Year - model.NgaySinh.Year;
                if (model.NgaySinh > DateTime.Now.AddYears(-age)) age--;

                if (age < 18 || age > 40)
                {
                    ModelState.AddModelError("NgaySinh", "Độ tuổi phải nằm trong khoảng từ 18 đến 40. Độ tuổi hiện tại của bạn là: " + age);
                    ViewBag.ChucVu = new SelectList(db.chucVus.ToList(), "ID", "TenChucVu");
                    ViewBag.Gender = gt;
                    return View(model);
                }
                ///////////////////////////////////////////////////////
                //Tồn tại nhân viên
                var existNV = db.nhanViens.FirstOrDefault(x => x.FullName == model.FullName && model.CCCD == model.CCCD);
                if (existNV != null)
                {
                    ModelState.AddModelError("FullName", "Nhân viên đã tồn tại.");
                    // Trả về View với model để hiển thị lỗi
                    ViewBag.ChucVu = new SelectList(db.chucVus.ToList(), "ID", "TenChucVu");
                    ViewBag.Gender = gt;
                    return View(model);
                }
                //Tồn tại tài khoản
                var existTK = db.nhanViens.FirstOrDefault(x => x.TenDangNhap == model.TenDangNhap);
                if (existTK != null)
                {
                    ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại.");
                    // Trả về View với model để hiển thị lỗi
                    ViewBag.ChucVu = new SelectList(db.chucVus.ToList(), "ID", "TenChucVu");
                    ViewBag.Gender = gt;
                    return View(model);
                }
                //////////////////////////////////////////////////////////////////////////////////
                //Kiểm tra đã có nhân viên là quản lý hay chưa
                if (model.ID_ChucVu == 2) // chỉ kiểm tra nếu người dùng đang chọn chức vụ Quản lý
                {
                    var existQuanLy = db.nhanViens.FirstOrDefault(x => x.ID_ChucVu == 2);
                    if (existQuanLy != null)
                    {
                        ModelState.AddModelError("ID_ChucVu", "Chức vụ Quản lý đã có người đảm nhận. Vui lòng kiểm tra lại");
                        ViewBag.ChucVu = new SelectList(db.chucVus.ToList(), "ID", "TenChucVu");
                        ViewBag.Gender = gt;
                        return View(model);
                    }
                }
                db.nhanViens.Add(model);
                db.SaveChanges();
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Thêm mới nhân viên " + model.FullName + " thành công"); ;
                return RedirectToAction("Index");
            }
            ViewBag.ChucVu = new SelectList(db.chucVus.ToList(), "ID", "TenChucVu");
            ViewBag.Gender = gt;
            return View();
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_ChinhSua", chucVu = "Quản lý")]
        public ActionResult Edit(string id)
        {
            var item = db.nhanViens.Find(id);
            ViewBag.ChucVu = new SelectList(db.chucVus.ToList(), "ID", "TenChucVu");
            var gt = new SelectList(new[]
            {
                new { Value = true, Text = "Nam" },
                new { Value = false, Text = "Nữ" }
            }, "Value", "Text");
            ViewBag.Gender = gt;
            return View(item);
        }
        [AdminAuthorize(idChucNang = "MEGATECH_ChinhSua", chucVu = "Quản lý")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NhanVien model)
        {
            var gt = new SelectList(new[]
{
                new { Value = true, Text = "Nam" },
                new { Value = false, Text = "Nữ" }
            }, "Value", "Text");
            if (ModelState.IsValid)
            {
                //// Kiểm tra độ tuổi từ 18 đến 40
                var age = DateTime.Now.Year - model.NgaySinh.Year;
                if (model.NgaySinh > DateTime.Now.AddYears(-age)) age--;

                if (age < 18 || age > 40)
                {
                    ModelState.AddModelError("NgaySinh", "Độ tuổi phải nằm trong khoảng từ 18 đến 40. Độ tuổi bạn lựa chọn là: " + age);
                    ViewBag.ChucVu = new SelectList(db.chucVus.ToList(), "ID", "TenChucVu");
                    ViewBag.Gender = gt;
                    return View(model);
                }
                // Kiểm tra xem đã có nhân viên nào có chức vụ là "Quản lý" chưa
                var quanLyRole = db.chucVus.FirstOrDefault(x => x.TenChucVu == "Quản lý");
                if (quanLyRole != null)
                {
                    var existQuanLy = db.nhanViens.FirstOrDefault(x => x.ID_ChucVu == quanLyRole.ID);
                    if (existQuanLy != null && model.ID_ChucVu == quanLyRole.ID)
                    {
                        ModelState.AddModelError("ID_ChucVu", "Chức vụ 'Quản lý' đã có người đảm nhận.");
                        ViewBag.ChucVu = new SelectList(db.chucVus.ToList(), "ID", "TenChucVu");
                        ViewBag.Gender = gt;
                        return View(model);
                    }
                }
                model.Modifiedby = "MEGATECH Administrator";
                model.ModifiedDate = DateTime.Now;
                db.nhanViens.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Chỉnh sửa thông tin nhân viên " + model.FullName + " thành công");
                return RedirectToAction("Index");
            }
            ViewBag.ChucVu = new SelectList(db.chucVus.ToList(), "ID", "TenChucVu");
            ViewBag.Gender = gt;
            return View();
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        [AdminAuthorize(idChucNang = "MEGATECH_XemChiTiet", chucVu = "Quản lý")]
        public ActionResult Detail(string id)
        {
            var item = db.nhanViens.Find(id);
            return View(item);
        }
        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <returns></returns>
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
                TempData["message"] = new XMessage("danger", "Đưa nhân viên " + id + " vào thùng rác thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }

            var item = db.nhanViens.Find(id);
            if (item == null)
            {
                //Thông báo thất bại
                TempData["message"] = new XMessage("danger", "Đưa nhân viên " + item.FullName + " vào thùng rác thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }
            item.IsActiveAccount = false;
            item.Status = 0;
            item.Modifiedby = "MEGATECH Administrator";
            item.ModifiedDate = DateTime.Now;
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            //Hiển thị thông báo thành công
            TempData["message"] = new XMessage("success", "Đưa thành công nhân viên " + item.FullName + " vào thùng rác");
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
                        var obj = db.nhanViens.Find(item);
                        if (obj != null)
                        {
                            obj.IsActiveAccount = false;
                            obj.Status = 0;
                            obj.Modifiedby = "MEGATECH Administrator";
                            obj.ModifiedDate = DateTime.Now;
                            db.Entry(obj).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                }
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Đưa thành công các nhân viên đã lựa chọn vào thùng rác");
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        /// <summary>
        /// //////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AdminAuthorize(idChucNang = "MEGATECH_Xoa", chucVu = "Quản lý")]
        public ActionResult Delete(string id)
        {
            var item = db.nhanViens.Find(id);
            if (item != null)
            {
                db.nhanViens.Remove(item);
                db.SaveChanges();
                //Hiển thị thông báo
                TempData["message"] = new XMessage("success", "Xoá thành công nhân viên " + item.FullName);
                return Json(new { success = true });
            }
            return Json(new { success = false });
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
                        var obj = db.nhanViens.Find(item);
                        db.nhanViens.Remove(obj);
                    }
                    db.SaveChanges();
                }
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Xoá thành công các nhân viên đã lựa chọn");
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        /// <summary>
        /// //////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AdminAuthorize(idChucNang = "MEGATECH_Xoa", chucVu = "Quản lý")]
        public ActionResult Undo(string id)
        {
            if (id == null)
            {
                //Thông báo thất bại
                TempData["message"] = new XMessage("danger", "Phục hồi nhân viên thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }
            var item = db.nhanViens.Find(id);
            if (item == null)
            {
                //Thông báo thất bại
                TempData["message"] = new XMessage("danger", "Phục hồi nhân viên thất bại");
                //Chuyển hướng trang
                return RedirectToAction("Index");
            }
            item.IsActiveAccount = true;
            item.Status = 1;
            item.Modifiedby = "MEGATECH Administrator";
            item.ModifiedDate = DateTime.Now;
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            //Hiển thị thông báo thành công
            TempData["message"] = new XMessage("success", "Phục hồi thành công nhân viên " + item.FullName);
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
                        var obj = db.nhanViens.Find(item);
                        if (obj != null)
                        {
                            obj.IsActiveAccount = true;
                            obj.Status = 1;
                            obj.Modifiedby = "MEGATECH Administrator";
                            obj.ModifiedDate = DateTime.Now;
                            db.Entry(obj).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                }
                //Hiển thị thông báo thành công
                TempData["message"] = new XMessage("success", "Phục hồi thành công các nhân viên đã lựa chọn");
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}