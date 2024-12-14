using HoaHoeHoaSoi.API.Helpers;
using HoaHoeHoaSoi.API.Models;
using HoaHoeHoaSoi.API.ViewModels;
using HoaHoeHoaSoi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace HoaHoeHoaSoi.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : BaseController
    {
        public UserController(HoaHoeHoaSoiContext dbContext) : base(dbContext)
        {
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password)) {
                return Response(400, string.Empty, "Username or password is empty");
            }

            try
            {
                var user = _dbContext.UserInfos.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);
                if (user == null)
                    return Response(400, string.Empty, "Username or password is not valid");

                return Response(200, JwtUtils.GenerateToken(user));
            }
            catch (Exception ex)
            {
                return Response(500, string.Empty, ex.Message);
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public IActionResult GetProfile()
        {
            try
            {
                var user = GetUserFromToken();

                if (user == null)
                    return Response(400, user, "User not found");

                return Response(200, user);

            }
            catch (Exception ex) 
            {
                return Response(500, string.Empty, ex.Message);
            }
        }

        [HttpPost("sign-up")]
        public IActionResult SignUp([FromBody] UserSignUpModel model)
        {
            try
            {
                var errorMsg = model.Validate();
                if (!string.IsNullOrEmpty(errorMsg))
                    return Response(400, string.Empty, errorMsg);

                if (_dbContext.UserInfos.FirstOrDefault(u => u.Username == model.Username) != null)
                    return Response(400, string.Empty, "Username is already taken");

                _dbContext.UserInfos.Add(new UserInfo
                {
                    Name = model.Name,
                    Username = model.Username,
                    Password = model.Password,
                });
                _dbContext.SaveChanges();

                return Response(200, true);
            }
            catch (Exception ex)
            {
                return Response(500, string.Empty, ex.Message);
            }
        }

        [HttpPut("edit")]
        [Authorize]
        public IActionResult Edit([FromForm] UserEditModel model)
        {
            try
            {
                var user = GetUserFromToken();
                if (user == null)
                    return Response(404, string.Empty, "User not found");

                if(string.IsNullOrEmpty(model.Name))
                    return Response(400, string.Empty, "Name is required");

                DateTime? dob = null;
                if(model.DOB != null)
                    dob = new DateTime(model.DOB.Value, TimeOnly.MinValue);

                string? avatar = null;
                if (model.Avatar != null)
                {
                    avatar = FuncHelpers.ConvertImgToBase64(model.Avatar);
                }

                user.Name = model.Name;
                user.Dob = dob;
                user.Address = model.Address;
                user.Phone = model.Phone;
                user.Avatar = avatar;
                user.Gender = model.Gender;
                user.Mail = model.Mail;

                _dbContext.SaveChanges();
                return Response(200, true);
            }
            catch (Exception ex) 
            {
                return Response(500, string.Empty, ex.Message);
            }
        }

        [HttpPut("change-password")]
        [Authorize]
        public IActionResult ChangePassword(UserChangePasswordModel model)
        {
            try
            {
                var user = GetUserFromToken();
                if (user == null)
                    return Response(404, string.Empty, "User not found");

                var errorMsg = model.Validate();
                if (!string.IsNullOrEmpty(model.OldPassword) && model.OldPassword != user.Password)
                    errorMsg = "Old password is wrong\n" + errorMsg;

                if (!string.IsNullOrEmpty(errorMsg))
                    return Response(400, string.Empty, errorMsg);

                user.Password = model.NewPassword;
                _dbContext.SaveChanges();

                return Response(200, true);
            }
            catch (Exception ex) 
            {
                return Response(500, string.Empty, ex.Message);
            }
        }
    }
}
