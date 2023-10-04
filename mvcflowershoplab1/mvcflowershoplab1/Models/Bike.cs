using System.ComponentModel.DataAnnotations;

namespace mvcflowershoplab1.Models
{
    public class Bike
    {
        [Key]
        public int BikeID { get; set; }
        [Required(ErrorMessage ="Wrong Bike Name")]
        [Display(Name ="Bike Name")]
        public string BikeName { get; set; }
        [Required(ErrorMessage = "Must key in Bike Type")]
        [Display(Name = "Bike Type")]
        public string BikeType { get; set; }
        [Required(ErrorMessage = "Must key in Bike Produced Date")]
        [Display(Name = "Bike Produced Date")]
        public DateTime BikeProducedDate { get; set; }
        [Required(ErrorMessage = "Must key in Bike Price")]
        [Display(Name = "Bike Price")]
        public decimal BikePrice { get; set; }
        public string ImageKey { get; set; }

    }
}
