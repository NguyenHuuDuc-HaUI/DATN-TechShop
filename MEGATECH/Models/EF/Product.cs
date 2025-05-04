using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MEGATECH.Models.EF
{
    [Table("Product")]
    public class Product : CommonAbstract
    {
        public Product()
        {
            this.ChiTietHoaDons = new HashSet<ChiTietHoaDon>();
        }
        [Key]
        [StringLength(50)]
        public string MaSanPham {  get; set; }
        [Required(ErrorMessage = "Loại sản phẩm không được để trống")]
        public string ProductCategoryID { get; set; }
        [Required(ErrorMessage = "Nhà cung cấp không được để trống")]
        public string SupplierID { get; set; }
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [RegularExpression(@"^[^0-9].*$", ErrorMessage = "Tên sản phẩm không được bắt đầu bằng chữ số.")]
        [StringLength(500)]
        public string Title {  set; get; }
        [Required(ErrorMessage = "Alias sản phẩm không được để trống")]
        [RegularExpression(@"^[^0-9].*$", ErrorMessage = "Alias sản phẩm không được bắt đầu bằng chữ số.")]
        [StringLength(500)]
        public string Alias { get; set; }
        public string? Image { get; set; }
        public string? MoTa { get; set; }
        public string? ChiTiet { get; set; }
        [Required(ErrorMessage = "Giá nhập sản phẩm không được để trống")]
        public decimal GiaNhap { get; set; }
        [Required(ErrorMessage = "Giá niêm yết sản phẩm không được để trống")]
        public decimal GiaNiemYet { get; set; }
        public decimal? GiaBan { get; set; }
        public decimal? GiamGia { get; set; }
        [Required(ErrorMessage = "Số lượng sản phẩm không được để trống")]
        public int SoLuong { get; set; }
        public int ViewCount { get; set; }
        public bool IsHome { get; set; }
        public bool IsSale { get; set; }
        public bool IsNew { get; set; }
        public bool IsHot { get; set; }
        public bool IsActive { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeywords { get; set; }
        [Required(ErrorMessage = "Tình trạng hiển thị bắt buộc thiếp lập")]
        public int Status { get; set; }
        [ForeignKey(nameof(ProductCategoryID))]
        public virtual ProductCategory ProductCategory { get; set; }
        [ForeignKey(nameof(SupplierID))]
        public virtual NhaCungCap NhaCungCap { get; set; }
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }
    }
}