using HoaHoeHoaSoi.API.Helpers;
using HoaHoeHoaSoi.API.Models;
using HoaHoeHoaSoi.API.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HoaHoeHoaSoi.API.Controllers
{
    public class BaseController : ControllerBase
    {
        protected HoaHoeHoaSoiContext _dbContext;
        public BaseController(HoaHoeHoaSoiContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected UserInfo GetUserFromToken()
        {
            var token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ")[1];
            var userId = JwtUtils.ValidateToken(token);
            return _dbContext.UserInfos.FirstOrDefault(u => u.Id == userId);
        }

        public ObjectResult Response<T>(int statusCode, T result, string errorMsg = "")
        {
            return StatusCode(statusCode, new ResponseModel<T>
            {
                ErrorMsg = errorMsg,
                Result = result
            });
        }

        public ObjectResult Response<T>(int statusCode, T result, int totalItem, int pageNumber, int itemCount, string errorMsg = "")
        {
            return StatusCode(statusCode, new ResponseModelList<T>
            {
                ErrorMsg = errorMsg,
                Result = result,
                TotalItem = totalItem,
                PageNumber = pageNumber,
                ItemCount = itemCount,
            });
        }
    }
}
