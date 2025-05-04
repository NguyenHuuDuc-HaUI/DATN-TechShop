using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MEGATECH.Models.EF
{
    [Table("NhanVien")]
    public class NhanVien : CommonAbstract
    {
        [Key]
        [StringLength(50)]
        public string ID { get; set; }
        public int? ID_ChucVu { get; set; }
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [RegularExpression("^[^0-9].*$", ErrorMessage = "Họ tên không được bắt đầu bằng số")]
        [StringLength(150)]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        public DateTime NgaySinh { get; set; }
        public string Image { get; set; }
        [Required(ErrorMessage = "Giới tính bắt buộc phải thiết lập")]
        public bool GioiTinh { get; set; }
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [StringLength (500)] 
        public string DiaChi { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^\d{10,}$", ErrorMessage = "Số điện thoại phải có từ 10 số trở lên.")]
        public string SoDienThoai { get; set; }
        [Required(ErrorMessage = "Email không được để trống")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email không hợp lệ.")]
        [StringLength (500)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Căn cước công dân/Chứng minh nhân dân không được để trống")]
        [RegularExpression(@"^\d{9,}$", ErrorMessage = "Căn cước công dân/Chứng minh nhân dân phải có từ 9 số trở lên.")]
        [StringLength(500)]
        public string CCCD { get; set; }
        [Required(ErrorMessage = "Tình trạng hiển thị bắt buộc thiếp lập")]
        public int Status { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [RegularExpression(@"^[A-Za-z]\w{4,}$", ErrorMessage = "Tên đăng nhập phải có độ dài từ 5 ký tự trở lên và không bắt đầu bằng số.")]
        [StringLength(50)]
        public string TenDangNhap { get; set; }

        [RegularExpression(@".{6,}", ErrorMessage = "Mật khẩu phải có độ dài từ 6 ký tự trở lên.")]
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(50)]
        public string MatKhau { get; set; }
        [RegularExpression(@"^[^0-9].*$", ErrorMessage = "Tên hiển thị không được bắt đầu từ số.")]
        [Required(ErrorMessage = "Tên hiển thị không được để trống")]
        [StringLength(150)]
        public string TenHienThi { get; set; }
        public bool IsActiveAccount { get; set; }
        [ForeignKey("ID_ChucVu")]
        public virtual ChucVu ChucVu { get; set; }
    }
}