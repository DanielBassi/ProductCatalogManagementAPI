using System.Runtime.Serialization;

namespace Domain.Exceptions;

[Serializable]
public sealed class ProductException : CoreBusinessException
{
    public ProductException()
    {
    }

    public ProductException(string msg) : base(msg)
    {
    }

    public ProductException(string message, Exception inner) : base(message, inner)
    {
    }

    [Obsolete("Obsolete")]
    private ProductException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
