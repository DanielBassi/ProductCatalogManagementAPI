using Domain.Dto;
using Domain.Services.Products;
using MediatR;

namespace Application.Products;

public class ProductUpdateCommandHandler(ProductUpdateService service) : IRequestHandler<ProductUpdateCommand, bool>
{
    private readonly ProductUpdateService _service = service ?? throw new ArgumentNullException(nameof(service));

    public async Task<bool> Handle(ProductUpdateCommand request, CancellationToken cancellationToken)
    {
        return await _service.UpdateProductAsync(
            new ProductDto(request.Id, request.Name, request.Description, request.Price), cancellationToken
        );
    }
}
