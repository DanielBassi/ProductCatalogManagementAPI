using Domain.Dto;
using Domain.Entities;
using Domain.Services.Products;
using MediatR;

namespace Application.Products;

public class ProductSaveCommandHandler(ProductSaveService service) : IRequestHandler<ProductSaveCommand, ProductDto>
{
    private readonly ProductSaveService _service = service ?? throw new ArgumentNullException(nameof(service));

    public async Task<ProductDto> Handle(ProductSaveCommand request, CancellationToken cancellationToken)
    {
        var productSaved = await _service.SaveProductAsync(
            new Product(request.Name, request.Description, request.Price), cancellationToken
        );

        return new ProductDto(productSaved.Id, productSaved.Name, productSaved.Description, productSaved.Price);
    }
}
