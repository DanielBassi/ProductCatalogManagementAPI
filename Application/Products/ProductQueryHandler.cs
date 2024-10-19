using Domain.Dto;
using Domain.Ports;
using MediatR;

namespace Application.Products;

public class ProductQueryHandler(IProductRepository repository) : IRequestHandler<ProductQuery, ProductDto?>
{
    public async Task<ProductDto?> Handle(ProductQuery request, CancellationToken cancellationToken)
    {
        var product = await repository.GetSingleProductByIdAsync(request.Id);
        return product is null ? null : new ProductDto(product!.Id, product.Name, product.Description, product.Price);
    }
}
