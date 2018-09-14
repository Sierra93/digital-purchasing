using System;
using System.ComponentModel.DataAnnotations;

namespace DigitalPurchasing.Models
{
    public class RawColumns
    {
        [Key]
        public Guid RawColumnsId { get; set; } = Guid.NewGuid();

        // columns
        public string Code { get; set; }
        public string Name { get; set; }
        public string Uom { get; set; }
        public string Qty { get; set; }
    }
}