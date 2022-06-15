using System.Net;

namespace DN.WebApi.Infrastructure.Common.Exceptions;

public class EntityCannotBeDeleted : CustomException
{
    public EntityCannotBeDeleted(string message)
    : base(message, null, HttpStatusCode.BadRequest)
    {
    }
}
