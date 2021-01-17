using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TN.ViewModels.Catalog.User
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Không được để trống")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải chứa tối thiểu 6 kí tự", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu không trùng khớp")]
        public string ConfirmNewPassword { get; set; }
    }
}
