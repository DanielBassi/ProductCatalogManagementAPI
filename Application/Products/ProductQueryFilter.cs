using Domain.Dto;
using MediatR;

namespace Application.Products;

public record ProductQueryFilter(
    Guid? Id = null,
    string? Name = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    bool? IncludeDeleted = false
) : IRequest<IEnumerable<ProductDto>>;
