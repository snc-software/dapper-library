namespace SncSoftware.Extensions.Dapper.Exceptions;

public class MissingPrimaryIdentifierException : Exception
{
    public MissingPrimaryIdentifierException(string type)
    {
        Type = type;
    }

    private string Type { get; }

    public override string Message => $"No primary identifier has been declared for type: {Type}";
}