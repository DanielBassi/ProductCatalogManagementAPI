using Domain.Dto;
using MediatR;

namespace Application.Products;

public record ProductSaveCommand(
    Guid? Id, string Name, string? Description, decimal Price
) : IRequest<ProductDto>;
