using Domain.Services.Products;
using MediatR;

namespace Application.Products;

public class ProductDeleteCommandHandler(ProductDeleteService service) : IRequestHandler<ProductDeleteCommand, bool>
{
    private readonly ProductDeleteService _service = service ?? throw new ArgumentNullException(nameof(service));

    public async Task<bool> Handle(ProductDeleteCommand request, CancellationToken cancellationToken)
    {
        return await _service.DeleteProductAsync(request.Id, cancellationToken);
    }
}
