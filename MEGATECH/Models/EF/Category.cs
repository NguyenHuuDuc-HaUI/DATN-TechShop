using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MEGATECH.Models.EF
{
    [Table("Category")]
    public class Category : CommonAbstract
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [RegularExpression(@"^[^0-9].*$", ErrorMessage = "Tên danh mục không được bắt đầu từ số.")]
        [StringLength(150)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Alias danh mục không được để trống")]
        [StringLength(150)]
        public string Alias { get; set; }
        public string Link { get; set; }
        [StringLength(150)]
        public string SeoTitle { get; set; }
        [StringLength(250)]
        public string SeoDescription { get; set; }
        [StringLength(150)]
        public string SeoKeywords { get; set; }
        [Required(ErrorMessage = "Tình trạng hoạt động bắt buộc thiết lập")]
        public bool IsActive { get; set; }
        public int? Position { get; set; }
        [Required(ErrorMessage = "Tình trạng hiển thị bắt buộc thiếp lập")]
        public int Status { get; set; }
    }
}