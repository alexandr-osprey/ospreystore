﻿namespace ApplicationCore.ViewModels
{
    public class Response
    {
        public Response(string message)
        {
            Message = message;
        }
        public string Message { get; set; }
    }
}
