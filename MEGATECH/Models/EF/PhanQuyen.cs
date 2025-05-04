using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MEGATECH.Models.EF
{
    [Table("PhanQuyen")]
    public class PhanQuyen
    {
        [Key]
        [Column(Order = 0)]
        public int IDChucVu { get; set; }
        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string MaChucNang { get; set; }
        public string? GhiChu { get; set; }
        [ForeignKey(nameof(MaChucNang))]
        public virtual ChucNangQuyen ChucNangQuyen { get; set; }
        [ForeignKey(nameof(IDChucVu))]
        public virtual ChucVu ChucVu { get; set; }

    }
}