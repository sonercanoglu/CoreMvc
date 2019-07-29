using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreMvc.Models
{
    public class UserView
    {
        public int Id { get; set; }
        [ForeignKey("Employee")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Employee")]
        public int Employee_Id { get; set; }
        public string EmployeeName { get; set; }
        [Required(ErrorMessage = "Please Enter Username."), StringLength(50)]
        public string Username { get; set; }
        [Required(ErrorMessage = "Please Enter Password."), StringLength(50)]
        public string Password { get; set; }

        public virtual List<UserRoleView> UserRoleViews { get; set; }
    }
}
