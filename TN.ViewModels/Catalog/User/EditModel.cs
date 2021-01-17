using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TN.ViewModels.Catalog.User
{
    public class EditModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "UserName")]
        [StringLength(50, ErrorMessage = "User name must be at max 50 characters long.")]
        public string UserName { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        public DateTime DoB { get; set; }

    }
}
