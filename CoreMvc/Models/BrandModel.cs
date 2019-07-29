using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreMvc.Models
{
    public class BrandModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please Select Brand.")]
        [ForeignKey("Brand")]
        public int Brand_Id { get; set; }
        [Required(ErrorMessage = "Please Enter Model Name."), StringLength(50)]
        public string Name { get; set; }

        public virtual Brand Brand{ get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
