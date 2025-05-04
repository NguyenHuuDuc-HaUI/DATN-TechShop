using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MEGATECH.Models.EF
{
    [Table("ChucNangQuyen")]
    public class ChucNangQuyen
    {
        public ChucNangQuyen()
        {
            this.PhanQuyen = new HashSet<PhanQuyen>();
        }
        [Key]
        [StringLength(50)]
        [Required(ErrorMessage = "Mã chức năng không được để trống")]
        public string MaChucNang { get; set; }
        [StringLength(500)]
        [RegularExpression(@"^[^0-9].*$", ErrorMessage = "Tên chức năng không được bắt đầu từ số.")]
        [Required(ErrorMessage = "Tên chức năng không được để trống")]
        public string TenChucNang { get; set; }
        [StringLength(500)]
        public string? MoTa {  get; set; }
        public virtual ICollection<PhanQuyen> PhanQuyen { get; set; }
    }
}