﻿namespace redis.WebAPi.Model.UserModels
{
    public class ResetPasswordRequest
    {
        public string Username { get; set; }
        public string NewPassword { get; set; }
    }
}
