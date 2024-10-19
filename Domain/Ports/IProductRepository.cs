using Domain.Dto;
using Domain.Entities;

namespace Domain.Ports;

public interface IProductRepository
{
    public Task DeleteProductAsync(Product product);
    public Task<IEnumerable<Product>> GetProductsByFilterAsync(ProductFilterDto f);
    public Task<Product?> GetSingleProductByIdAsync(Guid id);
    public Task<Product> SaveProductAsync(Product product);
    public Task UpdateProductAsync(Product product);
}
