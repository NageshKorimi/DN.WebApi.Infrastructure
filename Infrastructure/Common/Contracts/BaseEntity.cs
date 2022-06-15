

using System.ComponentModel.DataAnnotations;

namespace DN.WebApi.Infrastructure.Common.Contracts;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; private set; }
}
    