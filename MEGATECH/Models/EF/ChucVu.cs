using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MEGATECH.Models.EF
{
    [Table("ChucVu")]
    public class ChucVu : CommonAbstract
    {
        public ChucVu() 
        {
            this.NhanVien = new HashSet<NhanVien>();
            this.PhanQuyen = new HashSet<PhanQuyen>();
        }
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "Tên chức vụ không được để trống")]
        [RegularExpression("^[^0-9].*$", ErrorMessage = "Tên chức vụ không được bắt đầu bằng số")]
        [StringLength(500)]
        public string TenChucVu { get; set; }
        [StringLength(500)]
        public string? MoTa { get; set; }
        [Required(ErrorMessage = "Tình trạng hiển thị bắt buộc thiếp lập")]
        public int Status { get; set; }
        public virtual ICollection<NhanVien> NhanVien { get; set; }
        public virtual ICollection<PhanQuyen> PhanQuyen { get; set; }
    }
}