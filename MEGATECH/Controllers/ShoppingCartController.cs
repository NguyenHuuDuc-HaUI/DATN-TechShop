using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MEGATECH.Models;
using MEGATECH.Models.EF;
using MEGATECH.Models.Payments;
using System.Security.Policy;
using System.Text;

namespace MEGATECH.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private MEGATECHDBContext db = new MEGATECHDBContext();
        // GET: ShoppingCart
        [AllowAnonymous]
        public ActionResult Index()
        {

            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;
            }
            return View();
        }
        [AllowAnonymous]
        public ActionResult VnpayReturn()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        var itemOrder = db.hoaDons.FirstOrDefault(x => x.MaHoaDon == orderCode);
                        if (itemOrder != null)
                        {
                            itemOrder.TrangThai = 2;//đã thanh toán
                            if (itemOrder.PhuongThucThanhToan == 3)
                            {
                                itemOrder.TrangThai = 2;
                            }

                            var orderDetails = db.chiTietHoaDons.Where(od => od.OrderID == itemOrder.MaHoaDon).ToList();
                            foreach (var orderDetail in orderDetails)
                            {
                                var product = db.products.Find(orderDetail.ProductID);
                                if (product != null)
                                {
                                    product.SoLuong -= orderDetail.SoLuong;
                                    db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                                }
                            }

                            db.hoaDons.Attach(itemOrder);
                            db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();


                            // Gán các thông tin từ hoá đơn vào ViewBag để sử dụng trong view
                            ViewBag.OrderDate = itemOrder.CreatedDate.ToString("dd");
                            ViewBag.OrderMonth = itemOrder.CreatedDate.ToString("mm");
                            ViewBag.OrderYear = itemOrder.CreatedDate.ToString("yyyy");
                            ViewBag.CustomerID = itemOrder.ID_KhachHang;
                            ViewBag.CustomerName = itemOrder.TenKhachHang;
                            ViewBag.CCCD = itemOrder.CCCD;
                            ViewBag.Address = itemOrder.DiaChi;
                            ViewBag.Phone = itemOrder.SoDienThoai;
                            ViewBag.TypePayment = "";
                            if (itemOrder.PhuongThucThanhToan == 1)
                            {
                                ViewBag.TypePayment = "COD";
                            }
                            else if (itemOrder.PhuongThucThanhToan == 2)
                            {
                                ViewBag.TypePayment = "Chuyển khoản";
                            }
                            else if (itemOrder.PhuongThucThanhToan == 3)
                            {
                                ViewBag.TypePayment = "Mua trực tiếp";
                            }
                            ViewBag.OrderDetails = orderDetails;
                        }
                        //Thanh toan thanh cong
                        ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ của Hoàng Hà.";
                        //log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                        //log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                    }
                    //displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
                    //displayTxnRef.InnerText = "Mã giao dịch thanh toán:" + orderId.ToString();
                    //displayVnpayTranNo.InnerText = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
                    ViewBag.ThanhToanThanhCong = "Số tiền thanh toán: " + vnp_Amount.ToString("C0");
                    ViewBag.MaVanDon = orderCode;
                    //displayBankCode.InnerText = "Ngân hàng thanh toán:" + bankCode;
                }
            }
            //var a = UrlPayment(0, "DH3574");
            return View();
        }

        [AllowAnonymous]
        public ActionResult CheckOut()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;
            }
            ViewBag.Year = DateTime.Now.ToString("yyyy");
            ViewBag.Month = DateTime.Now.ToString("MM");
            ViewBag.Date = DateTime.Now.ToString("dd");
            return View();
        }
        [AllowAnonymous]
        public ActionResult CheckOutSuccess(string orderCode)
        {
            if (orderCode != null)
            {
                // Truy vấn cơ sở dữ liệu để lấy thông tin hoá đơn dựa trên mã hoá đơn
                var order = db.hoaDons.FirstOrDefault(x => x.MaHoaDon == orderCode);
                if (order != null)
                {
                    // Gán các thông tin từ hoá đơn vào ViewBag để sử dụng trong view
                    ViewBag.OrderCode = orderCode;
                    ViewBag.OrderDate = order.CreatedDate.ToString("dd");
                    ViewBag.OrderMonth = order.CreatedDate.ToString("mm");
                    ViewBag.OrderYear = order.CreatedDate.ToString("yyyy");
                    ViewBag.CustomerID = order.ID_KhachHang;
                    ViewBag.CustomerName = order.TenKhachHang;
                    ViewBag.CCCD = order.CCCD;
                    ViewBag.Address = order.DiaChi;
                    ViewBag.Phone = order.SoDienThoai;
                    ViewBag.TypePayment = "";
                    if (order.PhuongThucThanhToan == 1)
                    {
                        ViewBag.TypePayment = "COD";
                    }
                    else if (order.PhuongThucThanhToan == 2)
                    {
                        ViewBag.TypePayment = "Chuyển khoản";
                    }
                    else if (order.PhuongThucThanhToan == 3)
                    {
                        ViewBag.TypePayment = "Mua trực tiếp";
                    }
                    // Truy vấn cơ sở dữ liệu để lấy danh sách sản phẩm của hoá đơn
                    var orderDetails = db.chiTietHoaDons.Where(x => x.OrderID == orderCode).ToList();
                    ViewBag.OrderDetails = orderDetails;
                }
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult CheckOutSuccessByOffline(string orderCode)
        {
            if (orderCode != null)
            {
                var itemOrder = db.hoaDons.FirstOrDefault(x => x.MaHoaDon == orderCode);
                if (itemOrder != null)
                {
                    itemOrder.TrangThai = 2; // Đánh dấu đơn hàng đã thanh toán

                    var orderDetails = db.chiTietHoaDons.Where(od => od.OrderID == itemOrder.MaHoaDon).ToList();
                    foreach (var orderDetail in orderDetails)
                    {
                        var product = db.products.Find(orderDetail.ProductID);
                        if (product != null)
                        {
                            product.SoLuong -= orderDetail.SoLuong; // Giảm số lượng sản phẩm trong kho
                            db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                        }
                    }

                    db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges(); // Lưu các thay đổi vào cơ sở dữ liệu

                    // Gán các thông tin từ hoá đơn vào ViewBag để sử dụng trong view
                    ViewBag.OrderDate = itemOrder.CreatedDate.ToString("dd");
                    ViewBag.OrderMonth = itemOrder.CreatedDate.ToString("mm");
                    ViewBag.OrderYear = itemOrder.CreatedDate.ToString("yyyy");
                    ViewBag.CustomerID = itemOrder.ID_KhachHang;
                    ViewBag.CustomerName = itemOrder.TenKhachHang;
                    ViewBag.CCCD = itemOrder.CCCD;
                    ViewBag.Address = itemOrder.DiaChi;
                    ViewBag.Phone = itemOrder.SoDienThoai;
                    ViewBag.TypePayment = "";
                    if (itemOrder.PhuongThucThanhToan == 1)
                    {
                        ViewBag.TypePayment = "COD";
                    }
                    else if (itemOrder.PhuongThucThanhToan == 2)
                    {
                        ViewBag.TypePayment = "Chuyển khoản";
                    }
                    else if (itemOrder.PhuongThucThanhToan == 3)
                    {
                        ViewBag.TypePayment = "Mua trực tiếp";
                    }
                    // Truy vấn cơ sở dữ liệu để lấy danh sách sản phẩm của hoá đơn
                    ViewBag.OrderDetails = orderDetails;
                }
            }
            ViewBag.OrderCode = orderCode; // Gán mã hoá đơn vào ViewBag
            return View();
        }

        [AllowAnonymous]
        public ActionResult Partial_Item_ThanhToan()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }
        [AllowAnonymous]
        public ActionResult Partial_Item_Cart()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }

        [AllowAnonymous]
        public ActionResult ShowCount()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                return Json(new { Count = cart.Items.Count }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult Partial_CheckOut()
        {
            return PartialView();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut(OrderViewModel req)
        {
            var code = new { Success = false, Code = -1, Url = "", MaHoaDon = "" };
            if (ModelState.IsValid)
            {

                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart != null)
                {
                    HoaDon order = new HoaDon();
                    //////////////////////////////////////////////////////////////////
                    //Tạo ngẫu nhiên mã hoá đơn
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    var random = new Random();
                    // Kiểm tra xem khách hàng đã tồn tại chưa
                    var existingCustomer = db.hoaDons
                        .FirstOrDefault(h => h.TenKhachHang == req.TenKhachHang && h.CCCD == req.CCCD);
                    if (existingCustomer != null)
                    {
                        // Sử dụng mã khách hàng hiện có nếu khách hàng đã tồn tại
                        order.ID_KhachHang = existingCustomer.ID_KhachHang;
                    }
                    else
                    {
                        var maKhachHang = new string(Enumerable.Repeat(chars, 8)
                            .Select(s => s[random.Next(s.Length)]).ToArray());
                        order.ID_KhachHang = maKhachHang;
                    }
                    //////////////////////////////////////////////////////////////////
                    //Tạo ngẫu nhiên mã hoá đơn
                    string currentYear = DateTime.Now.ToString("yy");
                    string currentMonth = DateTime.Now.ToString("MM");
                    string currentDay = DateTime.Now.ToString("dd");
                    string randomString = new string(Enumerable.Repeat(chars, 8)
                        .Select(s => s[random.Next(s.Length)]).ToArray());
                    order.MaHoaDon = currentYear + currentMonth + currentDay + randomString;

                    ///////////////////////////////////////////////////////////////////
                    order.TenKhachHang = req.TenKhachHang;
                    order.SoDienThoai = req.SoDienThoai;
                    order.DiaChi = req.DiaChi;
                    order.Email = req.Email;
                    order.CCCD = req.CCCD;
                    order.TrangThai = 1;//chưa thanh toán / 2/đã thanh toán, 3/Hoàn thành, 4/hủy
                    cart.Items.ForEach(x => order.ChiTietHoaDons.Add(new ChiTietHoaDon
                    {
                        ProductID = x.ProductID,
                        SoLuong = x.Quantity,
                        GiaBan = x.Price
                    }));
                    //order.TongHoaDon = cart.Items.Sum(x => (x.Price * x.Quantity));
                    order.PhuongThucThanhToan = req.TypePayment;
                    order.CreatedDate = DateTime.Now;
                    order.ModifiedDate = DateTime.Now;
                    order.CreatedBy = req.TenKhachHang;
                    db.hoaDons.Add(order);
                    db.SaveChanges();

                    decimal tongHoaDon = order.ChiTietHoaDons.Sum(x => x.SoLuong * x.GiaBan);
                    ////////////////////////////////////////////////////////////////////////////////////////////////////
                    //Gửi hoá đơn đến mail admin
                    var strSanPham2 = "";
                    var thanhtien_admin = decimal.Zero;
                    var TongTien_admin = decimal.Zero;
                    foreach (var sp in cart.Items)
                    {
                        strSanPham2 += "<tr>";
                        strSanPham2 += "<td>" + sp.ProductName + "</td>";
                        strSanPham2 += "<td>" + sp.Quantity + "</td>";
                        strSanPham2 += "<td>" + MEGATECH.Common.Common.FormatNumber(sp.TotalPrice, 0) + "</td>";
                        strSanPham2 += "</tr>";
                        thanhtien_admin += sp.Price * sp.Quantity;
                    }
                    TongTien_admin = thanhtien_admin;
                    string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
                    contentAdmin = contentAdmin.Replace("{{MaDon}}", order.MaHoaDon);
                    contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham2);
                    contentAdmin = contentAdmin.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                    contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", order.TenKhachHang);
                    contentAdmin = contentAdmin.Replace("{{Phone}}", order.SoDienThoai);
                    contentAdmin = contentAdmin.Replace("{{Email}}", req.Email);
                    contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", order.DiaChi);
                    contentAdmin = contentAdmin.Replace("{{ThanhTien}}", MEGATECH.Common.Common.FormatNumber(thanhtien_admin, 0));
                    contentAdmin = contentAdmin.Replace("{{TongTien}}", MEGATECH.Common.Common.FormatNumber(TongTien_admin, 0));
                    MEGATECH.Common.Common.SendMail("MEGATECH", "Đơn hàng #" + order.MaHoaDon, contentAdmin.ToString(), ConfigurationManager.AppSettings["EmailAdmin"]);
                    ////////////////////////////////////////////////////////////////////////////////////////////////////


                    ////////
                    // Cập nhật số lượng sản phẩm trong bảng Product
                    foreach (var item in cart.Items)
                    {
                        var product = db.products.FirstOrDefault(p => p.MaSanPham == item.ProductID);
                        if (product != null)
                        {
                            if (product.SoLuong >= item.Quantity)
                            {
                                product.SoLuong -= item.Quantity; // Trừ số lượng
                                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                            }
                            else
                            {
                                // Nếu số lượng không đủ, thông báo lỗi
                                ModelState.AddModelError("", $"Sản phẩm {product.Title} không đủ số lượng.");
                                return Json(new { Success = false, msg = $"Sản phẩm {product.Title} không đủ số lượng." });
                            }
                        }
                    }
                    db.SaveChanges();
                    ////////

                    cart.ClearCart();
                    code = new { Success = true, Code = req.TypePayment, Url = "", MaHoaDon = order.MaHoaDon };

                    //var url = "";
                    if (req.TypePayment == 2)
                    {
                        var url = UrlPayment(req.TypePaymentVN, order.MaHoaDon);
                        code = new { Success = true, Code = req.TypePayment, Url = url, MaHoaDon = order.MaHoaDon };
                    }
                }
            }
            return Json(code);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult XuatHoaDon(string orderCode)
        {
            // Tìm kiếm đơn hàng dựa trên mã hóa đơn
            var order = db.hoaDons.FirstOrDefault(o => o.MaHoaDon == orderCode);
            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
            }
            //send mail cho khachs hang
            var items = db.chiTietHoaDons
                .Where(od => od.OrderID == orderCode)
                .Select(od => new ShoppingCartItem
                {
                    ProductID = od.ProductID,
                    ProductName = od.Product.Title,
                    Quantity = od.SoLuong,
                    Price = od.GiaBan,
                    TotalPrice = od.GiaBan * od.SoLuong
                })
                .ToList();

            var strSanPham = new StringBuilder();
            var thanhtien = decimal.Zero;
            var TongTien = decimal.Zero;
            var i = 1;
            foreach (var sp in items)
            {
                strSanPham.Append("<tr>");
                strSanPham.Append("<td style=\"text-align:center; width: 40px;\">" + i + "</td>");
                strSanPham.Append("<td style=\"text-align:center;width: 150px;\">" + sp.ProductName + "</td>");
                strSanPham.Append("<td style=\"text-align:center;width: 80px;\">" + sp.Quantity + "</td>");
                strSanPham.Append("<td style=\"text-align:center;\">" + MEGATECH.Common.Common.FormatNumber(sp.Price, 0) + " VNĐ</td>");
                strSanPham.Append("<td style=\"text-align:center;\">" + MEGATECH.Common.Common.FormatNumber(sp.Price * sp.Quantity, 0) + " VNĐ</td>");
                strSanPham.Append("</tr>");
                thanhtien += sp.Price * sp.Quantity;
                i++;
            }
            TongTien = thanhtien; // Tổng tiền thanh toán là tổng thành tiền

            string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/hoadonofficial.html"));
            contentCustomer = contentCustomer.Replace("{{MaDon}}", order.MaHoaDon);
            contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham.ToString());
            contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd"));
            contentCustomer = contentCustomer.Replace("{{ThangDat}}", DateTime.Now.ToString("MM"));
            contentCustomer = contentCustomer.Replace("{{NamDat}}", DateTime.Now.ToString("yyyy"));
            contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", order.TenKhachHang);
            contentCustomer = contentCustomer.Replace("{{CCCD}}", order.CCCD);
            contentCustomer = contentCustomer.Replace("{{Phone}}", order.SoDienThoai);
            contentCustomer = contentCustomer.Replace("{{Email}}", order.Email);
            contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", order.DiaChi);
            contentCustomer = contentCustomer.Replace("{{ThanhTien}}", MEGATECH.Common.Common.FormatNumber(thanhtien, 0));
            contentCustomer = contentCustomer.Replace("{{TongTien}}", MEGATECH.Common.Common.FormatNumber(TongTien, 0));
            if (order.PhuongThucThanhToan == 1)
            {
                contentCustomer = contentCustomer.Replace("{{HinhThucThanhToan}}", "COD");
            }
            else if (order.PhuongThucThanhToan == 2)
            {
                contentCustomer = contentCustomer.Replace("{{HinhThucThanhToan}}", "Chuyển khoản");
            }
            else if (order.PhuongThucThanhToan == 3)
            {
                contentCustomer = contentCustomer.Replace("{{HinhThucThanhToan}}", "Mua trực tiếp tại cửa hàng");
            }

            try
            {
                MEGATECH.Common.Common.SendMail("MEGATECH", "Đơn hàng #" + order.MaHoaDon, contentCustomer.ToString(), order.Email);
                return Json(new { success = true, message = "Hoá đơn đã được gửi thành công đến email " + order.Email });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi gửi hoá đơn: " + ex.Message });
            }
        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult AddToCart(string id, int quantity)
        {
            var code = new { Success = false, msg = "", code = -1, Count = 0 };
            var db = new MEGATECHDBContext();
            var checkProduct = db.products.FirstOrDefault(x => x.MaSanPham == id);
            if (checkProduct != null)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart == null)
                {
                    cart = new ShoppingCart();
                }
                ShoppingCartItem item = new ShoppingCartItem
                {
                    ProductID = checkProduct.MaSanPham,
                    ProductName = checkProduct.Title,
                    ProductImg = checkProduct.Image,
                    CategoryName = checkProduct.ProductCategory.Title,
                    SupplierName = checkProduct.NhaCungCap.Title,
                    TonKho = checkProduct.SoLuong,
                    Alias = checkProduct.Alias,
                    Quantity = quantity
                };
                item.Price = (decimal)checkProduct.GiaBan;
                item.TotalPrice = item.Quantity * item.Price;
                cart.AddToCart(item, quantity);
                Session["Cart"] = cart;
                code = new { Success = true, msg = "Thêm sản phẩm vào giỏ hàng thành công!", code = 1, Count = cart.Items.Count };
            }
            return Json(code);
        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult Update(string id, int quantity)
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                cart.UpdateQuantity(id, quantity);
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Delete(string id)
        {
            var code = new { Success = false, msg = "", code = -1, Count = 0 };

            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                var checkProduct = cart.Items.FirstOrDefault(x => x.ProductID == id);
                if (checkProduct != null)
                {
                    cart.Remove(id);
                    code = new { Success = true, msg = "", code = 1, Count = cart.Items.Count };
                }
            }
            return Json(code);
        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult DeleteAll()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                cart.ClearCart();
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }



        #region Thanh toán vnpay
        public string UrlPayment(int TypePaymentVN, string orderCode)
        {
            var urlPayment = "";
            var order = db.hoaDons.FirstOrDefault(x => x.MaHoaDon == orderCode);
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            // Tính tổng hóa đơn
            decimal tongHoaDon = order.ChiTietHoaDons.Sum(x => x.SoLuong * x.GiaBan);
            var Price = (long)tongHoaDon * 100;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (TypePaymentVN == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + order.MaHoaDon);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.MaHoaDon); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return urlPayment;
        }
        #endregion

    }
}