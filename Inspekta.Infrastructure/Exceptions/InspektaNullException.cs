namespace Inspekta.Infrastructure.Exceptions;

public class InspektaNullException : InspektaBaseException
{
    public InspektaNullException() : base() { }

    public InspektaNullException(string message) : base(message) { }

    public InspektaNullException(string message, Exception innerException) : base(message, innerException) { }
}
