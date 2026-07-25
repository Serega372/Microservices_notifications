namespace AuthService.Core.Exceptions;

public class RoleNotFoundException : Exception
{
    public RoleNotFoundException()
    {
    }

    public RoleNotFoundException(string message) : base(message)
    {
    }

    public RoleNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}