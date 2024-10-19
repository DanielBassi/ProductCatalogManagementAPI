using MediatR;

namespace Application.Products;

public record ProductUpdateCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price
) : IRequest<bool>;
