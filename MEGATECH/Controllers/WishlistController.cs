//using PagedList;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using MEGATECH.Models;
//using MEGATECH.Models.EF;

//namespace MEGATECH.Controllers
//{
//    [Authorize]
//    public class WishlistController : Controller
//    {
//        // GET: Wishlist
//        public ActionResult Index(int? page)
//        {
//            var pageSize = 10;
//            if (page == null)
//            {
//                page = 1;
//            }
//            IEnumerable<tb_Wishlist> items = db.tb_Wishlist.Where(x => x.UserName == User.Identity.Name).OrderByDescending(x => x.CreatedDate);
//            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
//            items = items.ToPagedList(pageIndex, pageSize);
//            ViewBag.PageSize = pageSize;
//            ViewBag.Page = page;
//            return View(items);
//        }

//        [HttpPost]
//        [AllowAnonymous]
//        public ActionResult PostWishlist(int ProductId)
//        {
//            if (Request.IsAuthenticated == false)
//            {
//                return Json(new { Success = false, Message = "Bạn chưa đăng nhập." });
//            }
//            var checkItem = db.tb_Wishlist.FirstOrDefault(x => x.ProductId == ProductId && x.UserName == User.Identity.Name);
//            if (checkItem != null)
//            {
//                return Json(new { Success = false, Message = "Sản phẩm đã được yêu thích rồi." });
//            }
//            var item = new tb_Wishlist();
//            item.ProductId = ProductId;
//            item.UserName = User.Identity.Name;
//            item.CreatedDate = DateTime.Now;
//            db.tb_Wishlist.Add(item);
//            db.SaveChanges();
//            return Json(new { Success = true });
//        }

//        [HttpPost]
//        [AllowAnonymous]
//        public ActionResult PostDeleteWishlist(int ProductId)
//        {
//            var checkItem = db.tb_Wishlist.FirstOrDefault(x => x.ProductId == ProductId && x.UserName == User.Identity.Name);
//            if (checkItem != null)
//            {
//                var item = db.tb_Wishlist.Find(checkItem.Id);
//                db.Set<tb_Wishlist>().Remove(item);
//                var i = db.SaveChanges();
//                return Json(new { Success = true, Message = "Xóa thành công." });
//            }
//            return Json(new { Success = false, Message = "Xóa thất bại." });
//        }

//        private MEGATECHEntities db = new MEGATECHEntities();
//        protected override void Dispose(bool disposing)
//        {
//            base.Dispose(disposing);
//        }
//    }
//}