using System.Net;
using DN.WebApi.Infrastructure.Common.Exceptions;
using DN.WebApi.Infrastructure.Common.Interfaces;
using DN.WebApi.Infrastructure.Wrapper;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Context;

namespace DN.WebApi.Infrastructure.Middleware;

internal class ExceptionMiddleware : IMiddleware
{
    private readonly ISerializerService _jsonSerializer;

    public ExceptionMiddleware(
        ISerializerService jsonSerializer)
    {
        _jsonSerializer = jsonSerializer;
        
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            string errorId = Guid.NewGuid().ToString();
            LogContext.PushProperty("ErrorId", errorId);
            LogContext.PushProperty("StackTrace", exception.StackTrace);
            var responseModel = await ErrorResult<string>.ReturnErrorAsync(exception.Message);
            responseModel.Source = exception.TargetSite?.DeclaringType?.FullName;
            responseModel.Exception = exception.Message.Trim();
            responseModel.ErrorId = errorId;
            responseModel.SupportMessage = "exceptionmiddleware.supportmessage";
            var response = context.Response;
            response.ContentType = "application/json";
            if (exception is not CustomException && exception.InnerException != null)
            {
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }
            }

            switch (exception)
            {
                case CustomException e:
                    response.StatusCode = responseModel.StatusCode = (int)e.StatusCode;
                    responseModel.Messages = e.ErrorMessages;
                    break;

                case KeyNotFoundException:
                    response.StatusCode = responseModel.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                default:
                    response.StatusCode = responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            Log.Error($"{responseModel.Exception} Request failed with Status Code {context.Response.StatusCode} and Error Id {errorId}.");
            await response.WriteAsync(_jsonSerializer.Serialize(responseModel));
        }
    }
}