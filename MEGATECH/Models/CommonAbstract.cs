using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MEGATECH.Models
{
    public class CommonAbstract
    {
        [Display(Name = "Người tạo")]
        public string CreatedBy { get; set; }

        [Display(Name = "Ngày tạo")]
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Ngày điều chỉnh")]
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "Người điều chỉnh")]
        public string? Modifiedby { get; set; }
    }
}