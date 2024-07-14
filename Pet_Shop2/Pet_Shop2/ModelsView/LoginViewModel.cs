using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pet_Shop2.ModelsView
{
    public class LoginViewModel
    {
        [MaxLength(100)]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage="Vui lòng nhập Email")]
        [Display(Name ="Điện thoại / Email")]
        public string? UserName { get;set; }
        [MinLength(5,ErrorMessage ="Bạn cần đặt mật khẩu tối thiểu 6 kí tự !")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [Display(Name = "Mật khẩu")]
        public string? Password { get; set; }

        public string? Phone { get; set; }
    }
}
