using System.ComponentModel.DataAnnotations;

namespace HoaHoeHoaSoi.API.ViewModels
{
    public class UserVMs
    {
    }

    public class UserLoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserSignUpModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }

        public string Validate()
        {
            var result = string.Empty;

            if (string.IsNullOrEmpty(Name))
                result += "Name is required\n";

            if (string.IsNullOrEmpty(Username))
                result += "Username is required\n";
            else if (Username.Length < 5 || Username.Length > 10)
                result += "Username must be from 5 to 10 characters\n";

            if (string.IsNullOrEmpty(Password))
                result += "Password is required\n";
            else if (Password.Length < 5 || Password.Length > 10)
                result += "Password must be from 5 to 10 characters\n";

            return result;
        }
    }

    public class UserEditModel
    {
        public string? Name { get; set; }
        public DateOnly? DOB { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public IFormFile? Avatar { get; set; }
        public bool? Gender { get; set; }
        public string? Mail { get; set; }
    }

    public class UserChangePasswordModel
    {
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
        public string Validate()
        {
            var result = string.Empty;

            if (string.IsNullOrEmpty(OldPassword))
                result += "Old password is required\n";

            if (string.IsNullOrEmpty(NewPassword))
                result += "New password is required\n";
            else if (NewPassword.Length < 5 || NewPassword.Length > 10)
                result += "New password must be from 5 to 10 characters\n";

            if (NewPassword != ConfirmPassword)
                result += "Passwords do not match";

            return result;
        }
    }
}
