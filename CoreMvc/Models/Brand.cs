using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreMvc.Models
{
    public class Brand
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please Enter Brand Name."),StringLength(50)]
        public string Name { get; set; }

        public virtual ICollection<BrandModel> BrandModels { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
