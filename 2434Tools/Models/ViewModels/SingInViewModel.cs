using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace _2434Tools.Models.ViewModels
{
    public class SingInViewModel
    {
        [Required]
        [EmailAddress]
        public String Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public String Password { get; set; }
        [Display(Name ="Keep me signed in")]
        public Boolean Remember { get; set; }
    }
}
