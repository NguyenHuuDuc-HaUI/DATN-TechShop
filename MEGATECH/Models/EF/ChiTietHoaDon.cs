using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MEGATECH.Models.EF
{
    [Table("ChiTietHoaDon")]
    public class ChiTietHoaDon
    {
        public int ID { get; set; }
        public string OrderID { get; set; }
        public string ProductID { get; set; }
        public decimal GiaBan { get; set; }
        public int GiamGia {  get; set; }
        public int SoLuong { get; set; }

        [ForeignKey(nameof(OrderID))]
        public virtual HoaDon HoaDon { get; set; }
        [ForeignKey(nameof(ProductID))]
        public virtual Product Product { get; set; }
    }
}