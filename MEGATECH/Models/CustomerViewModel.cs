using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MEGATECH.Models
{
    public class CustomerViewModel
    {
        public string MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string CCCD { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
    }
    public class CustomerDetailsViewModel
    {
        public string MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string CCCD { get; set; }
        public List<HoaDonViewModel> HoaDons { get; set; }
    }

    public class HoaDonViewModel
    {
        public string MaHoaDon { get; set; }
        public DateTime ThoiGianLap { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public int Status { get; set; }
    }
}