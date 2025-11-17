namespace Inspekta.API.Exceptions;

public abstract class InspektaBaseException : Exception
{
    protected InspektaBaseException() : base() { }

    protected InspektaBaseException(string message) : base(message) { }

    protected InspektaBaseException(string message, Exception innerException) : base(message, innerException) { }
}
