using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MEGATECH.Models.EF
{
    [Table("NhaCungCap")]
    public class NhaCungCap : CommonAbstract
    {
        public NhaCungCap() 
        {
            this.Products = new HashSet<Product>();
        }
        [Key]
        [StringLength(50)]
        public string MaNhaCungCap { get; set; }
        [Required(ErrorMessage = "Tên nhà cung cấp không được để trống")]
        [RegularExpression(@"^[^0-9].*$", ErrorMessage = "Tên nhà cung cấp không được bắt đầu từ số.")]
        [StringLength (500)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Alias nhà cung cấp không được để trống")]
        [RegularExpression(@"^[^0-9].*$", ErrorMessage = "Alias nhà cung cấp không được bắt đầu từ số.")]
        [StringLength (500)]
        public string Alias { get; set; }
        public string? Image { get; set; }
        [Required(ErrorMessage = "Số điện thoại nhà cung cấp không được để trống")]
        [RegularExpression(@"^\d{10,}$", ErrorMessage = "Số điện thoại phải có từ 10 số trở lên.")]
        public string SoDienThoai { get; set; }
        [Required(ErrorMessage = "Email nhà cung cấp không được để trống")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Tình trạng hiển thị bắt buộc thiếp lập")]
        public int Status { get; set; }
        [StringLength(150)]
        public string SeoTitle { get; set; }
        [StringLength(250)]
        public string SeoDescription { get; set; }
        [StringLength(150)]
        public string SeoKeywords { get; set; }
        public virtual ICollection<Product> Products { get; set; } 
    }
}