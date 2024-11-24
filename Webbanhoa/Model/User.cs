using System;
namespace HoaHoeHoaSoi.Model
{
	public class UserInfo
	{
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public DateTime? DOB { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public bool Gender { get; set; }
        public string Mail { get; set; }

        public UserInfo()
        {
        }
    }

    public class UserInfoSession
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
