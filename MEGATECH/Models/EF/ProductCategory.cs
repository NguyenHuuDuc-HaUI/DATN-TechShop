using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MEGATECH.Models.EF
{
    [Table("ProductCategory")]
    public class ProductCategory : CommonAbstract
    {
        public ProductCategory()
        {
            this.Products = new HashSet<Product>();
        }
        [Key]
        [StringLength(50)]
        public string MaLoaiSanPham { get; set; }
        [Required(ErrorMessage = "Tên loại sản phẩm không được để trống")]
        [RegularExpression(@"^[^0-9].*$", ErrorMessage = "Tên loại sản phẩm không được bắt đầu bằng chữ số.")]
        [StringLength (500)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Alias loại sản phẩm không được để trống")]
        [StringLength(500)]
        public string Alias { get; set; }
        [StringLength(500)]
        public string? MoTa { get; set; }
        public string? Image { get; set; }
        [Required(ErrorMessage = "Tình trạng hoạt động bắt buộc thiết lập")]
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Tình trạng hiển thị bắt buộc thiếp lập")]
        public int Status { get; set; }
        [StringLength(150)]
        public string SeoTitle { get; set; }
        [StringLength(250)]
        public string SeoDescription { get; set; }
        [StringLength(150)]
        public string SeoKeywords { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}