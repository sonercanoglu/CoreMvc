using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreMvc.Models
{
    public class UserRole
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please Select User.")]
        [ForeignKey("User")]
        public int User_Id { get; set; }
        [Required(ErrorMessage = "Please Select Role.")]
        [ForeignKey("Role")]
        public int Role_Id { get; set; }

        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
