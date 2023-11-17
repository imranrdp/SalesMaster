using System.ComponentModel.DataAnnotations.Schema;

namespace SalesMaster.Models
{
    public partial class SalesDetails
    {
        public int Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int SalesMasterId { get; set; }
        public virtual SalesMasterTable SalesMaster { get; set; } 

    }
}