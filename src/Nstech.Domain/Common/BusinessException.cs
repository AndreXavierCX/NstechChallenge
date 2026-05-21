namespace Nstech.Domain.Common;

public sealed class BusinessException : Exception
{
    public string Code { get; }

    public BusinessException(string message, string code = "BUSINESS_ERROR") : base(message)
    {
        Code = code;
    }

    public BusinessException(string message, Exception innerException, string code = "BUSINESS_ERROR") : base(message, innerException)
    {
        Code = code;
    }
}
