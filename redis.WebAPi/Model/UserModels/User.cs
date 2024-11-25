﻿namespace redis.WebAPi.Model.UserModels
{
    public class User
    {
        public int Id { get; set; }
        public string ?Username { get; set; }
        public string ?PasswordHash { get; set; }
        public string ?Email { get; set; }
        public string ?Role { get; set; } // For example, ‘user’ or ‘admin’
    }
}