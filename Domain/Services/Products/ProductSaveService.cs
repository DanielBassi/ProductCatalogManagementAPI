using Domain.Dto;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Ports;

namespace Domain.Services.Products;

[DomainService]
public class ProductSaveService(IProductRepository productRepository, IUnitOfWork unitOfWork)
{
    private async Task SaveChangesInUnitOfWorkAsync(CancellationToken cancellationToken)
    {
        await unitOfWork.SaveAsync(cancellationToken);
    }

    private async Task<Product> SaveProductInRepositoryAsync(Product product)
    {
        return await productRepository.SaveProductAsync(product);
    }

    public async Task<Product> SaveProductAsync(Product p, CancellationToken cancellationToken)
    {
        await ValidateProduct(p);
        var product = await SaveProductInRepositoryAsync(p);
        await SaveChangesInUnitOfWorkAsync(cancellationToken);
        return product;
    }

    public async Task ValidateProduct(Product p)
    {
        if (p == null) throw new ArgumentNullException(nameof(p));
        var existingProduct = await productRepository.GetProductsByFilterAsync(new ProductFilterDto(null, p.Name));
        if (existingProduct.Any(x => x.Active)) throw new ProductException("Ya existe un producto con el mismo nombre");
    }
}
