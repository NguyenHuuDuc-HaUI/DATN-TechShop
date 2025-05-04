using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MEGATECH.Models.EF
{
    [Table("HoaDon")]
    public class HoaDon : CommonAbstract
    {
        public HoaDon() 
        {
            this.ChiTietHoaDons = new HashSet<ChiTietHoaDon>();
        }
        [Key]
        [StringLength(50)]
        public string MaHoaDon {  get; set; }
        [Required(ErrorMessage = "Mã khách hàng không được để trống")]
        [StringLength(50)]
        public string ID_KhachHang { get; set; }
        [Required(ErrorMessage = "Tên khách hàng không được để trống")]
        [RegularExpression(@"^[^0-9].*$", ErrorMessage = "Tên khách hàng không được bắt đầu bằng chữ số.")]
        [StringLength(500)]
        public string TenKhachHang { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^\d{10,}$", ErrorMessage = "Số điện thoại phải có từ 10 số trở lên.")]
        public string SoDienThoai { get; set; }
        [StringLength(500)]
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string DiaChi {  get; set; }
        [StringLength(500)]
        public string? Email {  get; set; }
        [Required(ErrorMessage = "Căn cước công dân/Chứng minh nhân dân không được để trống")]
        [RegularExpression(@"^\d{9,}$", ErrorMessage = "Căn cước công dân/Chứng minh nhân dân phải có từ 9 số trở lên.")]
        [StringLength(500)]
        public string CCCD { get; set; }
        [Required(ErrorMessage = "Phương thức thanh toán bắt buộc thiết lập")]
        public int PhuongThucThanhToan {  get; set; }
        [Required(ErrorMessage = "Trạng thái bắt buộc thiết lập")]
        public int TrangThai {  get; set; }

        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }
    }
}