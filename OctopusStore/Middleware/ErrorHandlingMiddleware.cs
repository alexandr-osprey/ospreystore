﻿using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace OctopusStore.Middleware
{
    public class ErrorHandlingMiddleware 
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;

            if (exception is EntityNotFoundException) code = HttpStatusCode.NotFound;
            else if (exception is EntityAlreadyExistsException) code = HttpStatusCode.Conflict;
            else if (exception is EntityValidationException) code = HttpStatusCode.Conflict;
            else if (exception is BadRequestException) code = HttpStatusCode.BadRequest;
            else if (exception is AuthenticationException) code = HttpStatusCode.Unauthorized;
            else if (exception is SecurityTokenExpiredException) code = HttpStatusCode.Unauthorized;
            else if (exception is AuthorizationException) code = HttpStatusCode.Forbidden;
            else if (exception is CustomDbException) code = HttpStatusCode.InternalServerError;
            

            var result = JsonConvert.SerializeObject(new { message = exception.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
