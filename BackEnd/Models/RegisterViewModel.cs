using BackEnd.Models.Entity;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Tài khoản không được trống")]
        [MinLength(6, ErrorMessage = "Tài khoản ít  nhất 6 ký tự")]
        public string username { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được trống")]
        [MinLength(5, ErrorMessage = "Mật khẩu ít nhất 5 ký tự")]
        public string password { get; set; }
        [Required(ErrorMessage = "Họ tên không được trống")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Mật khẩu từ 5 ký tự")]
        public string fullname { get; set; }
        [Required(ErrorMessage = "SĐT không được trống")]
        public string phonenumber { get; set; }
        [Required(ErrorMessage = "Email không được trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string email { get; set; }
        [Required(ErrorMessage = "Địa chỉ không được trống")]
        public string address { get; set; }

        public  User mapRegisterToUser(RegisterViewModel model)
        {
          User user = new User();
            user.username = model.username;
            user.password = model.password;
            user.fullname = model.fullname;
            user.phonenumber = model.phonenumber;
            user.email = model.email;
            user.address = model.address;
            return user;
        }
    }
}
