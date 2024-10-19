using System.Text.Json;
using Application.Products;
using Domain.Dto;
using Domain.Entities;
using Domain.Ports;
using Infrastructure.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Tests;

public class ProductApiTest
{
    [Fact]
    public async Task DeleteProductSuccess()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<Product>>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var product = new Product("Product 4", "Description 4", 200.00m);
        var productCreated = await repository.AddAsync(product);
        await unitOfWork.SaveAsync(new CancellationTokenSource().Token);

        var client = webApp.CreateClient();
        var deleteResponse = await client.DeleteAsync($"/api/product/{productCreated.Id}");

        deleteResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task GetSingleProductSuccess()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<Product>>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var product = new Product("Product 1", "Description 1", 100.00m);
        var productCreated = await repository.AddAsync(product);
        await unitOfWork.SaveAsync(new CancellationTokenSource().Token);

        var client = webApp.CreateClient();
        var singleProduct = await client.GetFromJsonAsync<ProductDto>($"/api/product/{productCreated.Id}");

        Assert.True(singleProduct is not null);
        Assert.Equal(singleProduct.Id, productCreated.Id);
    }

    [Fact]
    public async Task PostProductFailureByNegativePrice()
    {
        HttpResponseMessage request = default!;
        try
        {
            await using var webApp = new ApiApp();

            ProductSaveCommand productCommand = new(null, "Product 3", "Description 3", -50.00m);
            var client = webApp.CreateClient();
            request = await client.PostAsJsonAsync("/api/product/", productCommand);

            request.EnsureSuccessStatusCode();
            Assert.Fail("There's no way to get here if product price is negative");
        }
        catch (Exception)
        {
            var responseMessage = await request.Content.ReadAsStringAsync();
            Assert.True(request.StatusCode == HttpStatusCode.UnprocessableEntity &&
                        responseMessage.Contains("The price must be greater than or equal to 1"));
        }
    }

    [Fact]
    public async Task PostProductSuccess()
    {
        await using var webApp = new ApiApp();

        ProductSaveCommand productCommand = new(null, "Product 2", "Description 2", 150.00m);
        var client = webApp.CreateClient();
        var request = await client.PostAsJsonAsync("/api/product/", productCommand);

        request.EnsureSuccessStatusCode();

        var deserializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var responseData =
            JsonSerializer.Deserialize<ProductDto>(await request.Content.ReadAsStringAsync(), deserializeOptions);

        Assert.True(responseData is not null);
        Assert.IsType<ProductDto>(responseData);
    }
}
