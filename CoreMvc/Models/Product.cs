using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreMvc.Models
{
    public class Product
    {
        public int Id { get; set; }
        [ForeignKey("Brand")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Brand")]
        public int Brand_Id { get; set; }
        [ForeignKey("BrandModel")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Model")]
        public int BrandModel_Id { get; set; }
        [Required(ErrorMessage = "Please Enter Amount.")]
        public int Amount { get; set; }
        [Required(ErrorMessage = "Please Enter Price.")]
        public decimal Price { get; set; }
        public bool Active { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual BrandModel BrandModel { get; set; }
    }
}
