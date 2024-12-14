using HoaHoeHoaSoi.API.Helpers;
using HoaHoeHoaSoi.API.Models;
using HoaHoeHoaSoi.API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")[1];
                var userId = JwtUtils.ValidateToken(token);
                var user = _dbContext.UserInfos.FirstOrDefault(u => u.Id == userId);

                if (user == null)
                    return Response(400, user, "User not found");

                return Response(200, user);

            }
            catch (Exception ex) 
            {
                return Response(500, string.Empty, ex.Message);
            }
            
            return Ok();
        }
    }
}
