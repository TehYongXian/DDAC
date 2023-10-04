using System.ComponentModel.DataAnnotations;

namespace mvcflowershoplab1.Models
{
    public class ReserveInfo
    {
        public string ReserveID { get; set; }
        public string CustomerName { get; set; }
        public int ReservePax { get; set; }
        public DateTime ReserveTime { get; set; }
        public int FlowerID { get; set; }
        public string FlowerName { get; set; }
        public string FlowerType { get; set; }
        public DateTime FlowerProducedDate { get; set; }
        public decimal FlowerPrice { get; set; }
        public string ImageKey { get; set; }
    }
}
