using System.ComponentModel.DataAnnotations;

namespace mvcflowershoplab1.Models
{
    public class Flower
    {
        [Key]
        public int FlowerID { get; set; }
        [Required(ErrorMessage ="Wrong Flower Name")]
        [Display(Name ="Flower Name")]
        public string FlowerName { get; set; }
        [Required(ErrorMessage = "Must key in Flower Type")]
        [Display(Name = "Flower Type")]
        public string FlowerType { get; set; }
        [Required(ErrorMessage = "Must key in Flower Produced Date")]
        [Display(Name = "Flower Produced Date")]
        public DateTime FlowerProducedDate { get; set; }
        [Required(ErrorMessage = "Must key in Flower Price")]
        [Display(Name = "Flower Price")]
        public decimal FlowerPrice { get; set; }

    }
}
