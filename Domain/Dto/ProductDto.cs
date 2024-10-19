namespace Domain.Dto;

public record ProductDto(Guid Id, string Name, string? Description, decimal Price);