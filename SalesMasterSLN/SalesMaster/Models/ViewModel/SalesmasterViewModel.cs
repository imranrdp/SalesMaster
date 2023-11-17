namespace SalesMaster.Models.ViewModel
{
    public class SalesmasterViewModel
    {
        public int SalesMasterId { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public virtual ICollection<SalesDetails> SalesDetails { get; set; } = new List<SalesDetails>();

        public int SalesDetailsId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public virtual SalesMasterTable SalesMaster { get; set; }
    }
}
