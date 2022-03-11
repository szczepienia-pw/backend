using backend.Dto.Responses;
using backend.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace backend.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.next(context);
            }
            catch (Exception error)
            {
                Object result = null;
                var response = context.Response;
                response.ContentType = "application/json";

                if (error is not BasicException)
                {
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    result = new ErrorResponse(error?.Message);
                }
                else
                {
                    result = (error as BasicException)?.Render(response);
                }

                var serializerSettings = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                await response.WriteAsync(JsonSerializer.Serialize(result, serializerSettings));
            }
        }
    }
}
