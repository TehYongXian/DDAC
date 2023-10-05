using System.ComponentModel.DataAnnotations;

namespace mvcflowershoplab1.Models
{
    public class ReserveInfo
    {
        public string ReserveID { get; set; }
        public string UserName { get; set; }
        public DateTime ReservationTime { get; set; }
        public int BikeID { get; set; }
        public string BikeName { get; set; }
        public string BikeType { get; set; }
        public DateTime BikeProducedDate { get; set; }
        public decimal BikePrice { get; set; }
        public string ImageKey { get; set; }
    }
}
