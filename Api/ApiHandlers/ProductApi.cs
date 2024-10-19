using Api.Filters;
using Api.Validators;
using Application.Products;
using Domain.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.ApiHandlers;

public static class ProductApi
{
    public static RouteGroupBuilder MapProduct(this IEndpointRouteBuilder routeHandler)
    {
        //GetById
        routeHandler.MapGet("/{id}",
                async (IMediator mediator, Guid id) =>
                {
                    var product = await mediator.Send(new ProductQuery(id));
                    return product is null ? Results.NotFound() : Results.Ok(product);
                })
            .Produces(StatusCodes.Status200OK, typeof(ProductDto))
            .Produces(StatusCodes.Status404NotFound);

        //Save
        routeHandler.MapPost("/", async (IMediator mediator, [Validate] ProductSaveCommand command) =>
            {
                var product = await mediator.Send(command);
                return Results.Created(new Uri($"/api/product/{product.Id}", UriKind.Relative), product);
            })
            .Produces(StatusCodes.Status201Created);

        // GetByFilter
        routeHandler.MapGet("/", async (IMediator mediator,
                [FromQuery] Guid? id,
                [FromQuery] string? name,
                [FromQuery] decimal? minPrice,
                [FromQuery] decimal? maxPrice,
                [FromQuery] bool? includeDeleted,
                [FromServices] ProductQueryFilterValidator validator) =>
            {
                var productQueryFilter = new ProductQueryFilter(id, name, minPrice, maxPrice, includeDeleted);

                var validationResult = await validator.ValidateAsync(productQueryFilter);

                if (!validationResult.IsValid) return Results.UnprocessableEntity(validationResult.Errors);

                var products = await mediator.Send(productQueryFilter);
                return Results.Ok(products);
            })
            .Produces(StatusCodes.Status200OK, typeof(IEnumerable<ProductDto>));

        // Update
        routeHandler.MapPut("/{id}", async (IMediator mediator, Guid id, [Validate] ProductUpdateCommand command) =>
            {
                if (id != command.Id)
                    return Results.BadRequest("El producto ID no es igual que el producto ID enviado en el body de la petición.");

                var result = await mediator.Send(command);
                return result ? Results.NoContent() : Results.NotFound();
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        // Delete
        routeHandler.MapDelete("/{id}", async (IMediator mediator, Guid id) =>
            {
                var result = await mediator.Send(new ProductDeleteCommand(id));
                return result ? Results.NoContent() : Results.NotFound();
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return (RouteGroupBuilder)routeHandler;
    }
}
