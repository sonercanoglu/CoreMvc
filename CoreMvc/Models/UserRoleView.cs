using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreMvc.Models
{
    public class UserRoleView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserRole_Id { get; set; }
        public bool Selected{ get; set; }
    }
}
