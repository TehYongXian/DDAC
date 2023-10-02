namespace mvcflowershoplab1.Models
{
    public class CustomerPurchase
    {
        //partition key
        public string CustomerFullName { get; set; }
        public string CustomerTransactionID { get; set; }
        public decimal PurchaseAmount { get; set; }
        public bool paymentstatus { get; set; }
        //if status is false, meanse no payment date 
        public DateTime paymentDate { get; set; }

    }
}
