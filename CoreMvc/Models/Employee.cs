using System;
using System.ComponentModel.DataAnnotations;

namespace CoreMvc.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please Enter Name."), StringLength(50)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please Enter Surname."), StringLength(50)]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Please Enter Birth Date.")]
        public DateTime BirthDate { get; set; }
        [Required(ErrorMessage = "Please Enter Gsm No."), StringLength(20)]
        public string GsmNo { get; set; }
        [Required(ErrorMessage = "Please Enter Email."), StringLength(50)]
        public string Email { get; set; }

        public virtual User User { get; set; }
    }
}
