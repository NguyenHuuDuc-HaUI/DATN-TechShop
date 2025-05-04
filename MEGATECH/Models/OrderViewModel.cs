using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MEGATECH.Models
{
    public class OrderViewModel
    {
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public string ID_Customer { get; set; }
        public string CCCD { get; set; }
        public int TypePayment { get; set; }
        public int TypePaymentVN { get; set; }
    }
}