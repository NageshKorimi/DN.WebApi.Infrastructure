using System.Net;

namespace DN.WebApi.Infrastructure.Common.Exceptions;

public class EntityAlreadyExistsException : CustomException
{
    public EntityAlreadyExistsException(string message)
    : base(message, null, HttpStatusCode.BadRequest)
    {
    }
}