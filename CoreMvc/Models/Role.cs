using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreMvc.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [NotMapped]
        public bool Selected{ get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
