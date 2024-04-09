using System;

namespace HoaHoeHoaSoi.Model {
    public class Admin {
        // Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        // Constructor
        public Admin(int id, string name, string address, string phone, string username, string password) {
            Id = id;
            Name = name;
            Address = address;
            Phone = phone;
            Username = username;
            Password = password;
        }

    }
}
