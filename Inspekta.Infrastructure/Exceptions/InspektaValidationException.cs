namespace Inspekta.Infrastructure.Exceptions;

public class InspektaValidationException : InspektaBaseException
{
    public InspektaValidationException() : base() { }

    public InspektaValidationException(string message) : base(message) { }

    public InspektaValidationException(string message, Exception innerException) : base(message, innerException) { }
}
