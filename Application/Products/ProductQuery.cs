using Domain.Dto;
using MediatR;

namespace Application.Products;

public record ProductQuery(
    Guid Id
    ) : IRequest<ProductDto?>;
