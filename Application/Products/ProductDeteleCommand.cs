using MediatR;

namespace Application.Products;

public record ProductDeleteCommand(
    Guid Id) : IRequest<bool>;
