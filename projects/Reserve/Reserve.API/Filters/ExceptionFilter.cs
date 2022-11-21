using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Reserve.API.Resources;

namespace Reserve.API.Filters
{
    [UsedImplicitly]
    public class ExceptionFilter : Attribute, IExceptionFilter
    {
        private readonly IWebHostEnvironment _environment;
        
        private static readonly Dictionary<string, HttpStatusCode> Exceptions = new Dictionary<string, HttpStatusCode>
        {
            { nameof(ArgumentNullException), HttpStatusCode.NotFound },
            { nameof(ValidationException), HttpStatusCode.BadRequest },
            { nameof(UnauthorizedAccessException), HttpStatusCode.Unauthorized },
            { nameof(DuplicateNameException), HttpStatusCode.Conflict },
            { nameof(OperationCanceledException), HttpStatusCode.BadRequest }
        };
        
        public ExceptionFilter(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        
        public void OnException(ExceptionContext context)
        {
            if (context.Exception == null)
                return;

            var response = new ServerExceptionResource
            {
                Error = context.Exception.Message
            };

            if (_environment.IsDevelopment())
            {
                response.Exception = context.Exception;
                response.CausedBy = context.Exception.HelpLink;
            }

            context.Result = new ObjectResult(response);
            context.HttpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            if (Exceptions.TryGetValue(context.Exception.GetType().Name, out HttpStatusCode value))
                context.HttpContext.Response.StatusCode = (int) value;

            context.Exception = null;
        }
    }
}